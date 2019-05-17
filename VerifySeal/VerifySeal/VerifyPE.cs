/****************************************************************************************
// <copyright file="VerifyPE.cs" company="AppEsteem Corporation">
// Copyright © 2018 All Rights Reserved
// </copyright>
****************************************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace VerifySeal
{
    /// <summary>
    /// Used to verify a portable executable format.
    /// </summary>
    public sealed class VerifyPE
    {
        internal PEFile _pe = null;
        internal string _seal = null;
        internal VersionInfo _versionInfo = null;

        public VerifyPE(BinaryReader reader)
        {
            // Create the PEFile structure. If it is not a PE, return.
            _pe = new PEFile(reader);
            if (false == _pe.IsPe)
                return;

            // If this is a CLI based PE, then get the seal from the manifest.
            if ((null != _pe.CliHeader) && (_pe.CliHeader.IsManaged))
            {
                _seal = ReadJsonFromManifest(reader);
            }
            else if (_pe.ResourceHeader.HasValue)  // If the ResourceHeader has a value, try and find the seal in the resources.
            {
                var fileOffset = _pe.GetDirectoryFileOffset(PEFile.IMAGE_DIRECTORY_ENTRY_RESOURCE);

                var sealDataEntry = _pe.FindSealDataEntry(reader, _pe.ResourceHeader.Value, (UInt32)fileOffset);
                if (sealDataEntry.HasValue)
                {
                    uint offset = _pe.RvaToFileOffset(sealDataEntry.Value.OffsetToData);
                    _seal = _pe.GetStringFromOffset(reader, offset, sealDataEntry.Value.Size);
                }

                var verDataEntry = _pe.FindVersionDataEntry(reader, _pe.ResourceHeader.Value, (UInt32)fileOffset);
                if (verDataEntry.HasValue)
                {
                    uint offset = _pe.RvaToFileOffset(verDataEntry.Value.OffsetToData);
                    _versionInfo = new VersionInfo(reader, offset, verDataEntry.Value.Size);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is a portable executable.
        /// </summary>
        public bool IsPe { get { return ((null != _pe) && (null != _pe.DosHeader) && (_pe.DosHeader.IsDOS) && (null != _pe.NTHeader) && (_pe.NTHeader.IsNT)); } }

        /// <summary>
        /// Gets the seal json.
        /// </summary>
        public string SealJson => _seal;

        /// <summary>
        /// Gets the file version.
        /// </summary>
        public string FileVersion => _versionInfo?.FileVersion;
        public string CompanyName => _versionInfo?.CompanyName;

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public string FileName => _versionInfo?.FileName;

        /// <summary>
        /// Gets a value indicating whether this instance is managed.
        /// </summary>
        /// <value><c>true</c> if this instance is managed; otherwise, <c>false</c>.</value>
        public bool IsManaged
        {
            get
            {
                if ((null == _pe) || (null == _pe.CliHeader))
                    return false;
                return _pe.CliHeader.IsManaged;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [is32 bit].
        /// </summary>
        /// <value><c>true</c> if [is32 bit]; otherwise, <c>false</c>.</value>
        public bool Is32Bit
        {
            get
            {
                if ((null == _pe) || (null == _pe.NTHeader)) return false;
                return _pe.NTHeader.Is32Bit;
            }
        }

        /// <summary>
        /// Gets the file creation date and time.
        /// </summary>
        public DateTimeOffset FileCreation
        {
            get
            {
                // Use this date and time, this is the offset assumed by the PE File Headers TimeDateStamp field.
                DateTimeOffset fileCreation = new DateTime(1969, 12, 31, 16, 0, 0, DateTimeKind.Utc);

                if ((null != _pe) && (null != _pe.NTHeader) && (_pe.NTHeader.IsNT))
                    return fileCreation.AddSeconds(_pe.NTHeader.FileHeader.TimeDateStamp);
                else
                    return default(DateTimeOffset);
            }
        }

        /// <summary>
        /// Get the file type message.
        /// </summary>
        public string GetFileTypeMessage()
        {
            if (Is32Bit)  // Output if this is a 32 bit native or managed application.
            {
                if (IsManaged)
                    return "File is a 32 bit managed code application.";
                else
                    return "File is a 32 bit native code application.";
            }
            else // Output if this is a 64 bit native or managed application.
            {
                if (IsManaged)
                    return "File is a 64 bit managed code application.";
                else
                    return "File is a 64 bit native code application.";
            }
        }

        /// <summary>
        /// Reads the json from a CLR assembly.
        /// </summary>
        /// <param name="reader">BinaryReader instance.</param>
        /// <returns>String containing the seal JSON.</returns>
        private string ReadJsonFromManifest(BinaryReader reader)
        {
            try
            {
                // Get the entire contents of the file.
                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    // Seek to the beginning of the stream.
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    reader.BaseStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }

                // Load the assemblly.
                Assembly assm = Assembly.Load(bytes);

                // Get the version info.
                var fileOffset = _pe.GetDirectoryFileOffset(PEFile.IMAGE_DIRECTORY_ENTRY_RESOURCE);
                var verDataEntry = _pe.FindVersionDataEntry(reader, _pe.ResourceHeader.Value, (UInt32)fileOffset);
                if (verDataEntry.HasValue)
                {
                    // Calculate the offset and reposition the stream.
                    uint offset = _pe.RvaToFileOffset(verDataEntry.Value.OffsetToData);
                    reader.BaseStream.Seek(offset, SeekOrigin.Begin);

                    byte[] content = reader.ReadBytes((int) verDataEntry.Value.Size);
                    _versionInfo = new VersionInfo(content);
                }

                // Find the seal in the manifest and return it.
                string[] names = assm.GetManifestResourceNames();
                foreach (var name in names)
                {
                    if (name.EndsWith("AESEAL.json", StringComparison.InvariantCultureIgnoreCase))
                    {
                        using (StreamReader sr = new StreamReader(assm.GetManifestResourceStream(name)))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Program.PrintMessage($"Unexpected exception {ex.Message}.", "Error", ConsoleColor.Red, ConsoleColor.Red);
            }

            return null;
        }
    }
}
