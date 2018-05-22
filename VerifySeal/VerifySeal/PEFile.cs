/****************************************************************************************
// <copyright file="PEFile.cs" company="AppEsteem Corporation">
// Copyright © 2018 All Rights Reserved
// </copyright>
****************************************************************************************/

using System;
using System.IO;
using System.Text;

namespace VerifySeal
{
    public sealed class PEFile
    {
        public const int IMAGE_DIRECTORY_ENTRY_EXPORT =         0;   // Export Directory
        public const int IMAGE_DIRECTORY_ENTRY_IMPORT =         1;   // Import Directory
        public const int IMAGE_DIRECTORY_ENTRY_RESOURCE =       2;   // Resource Directory
        public const int IMAGE_DIRECTORY_ENTRY_EXCEPTION =      3;   // Exception Directory
        public const int IMAGE_DIRECTORY_ENTRY_SECURITY =       4;   // Security Directory
        public const int IMAGE_DIRECTORY_ENTRY_BASERELOC =      5;   // Base Relocation Table
        public const int IMAGE_DIRECTORY_ENTRY_DEBUG =          6;   // Debug Directory
        public const int IMAGE_DIRECTORY_ENTRY_COPYRIGHT =      7;   // (X86 usage)
        public const int IMAGE_DIRECTORY_ENTRY_ARCHITECTURE =   7;   // Architecture Specific Data
        public const int IMAGE_DIRECTORY_ENTRY_GLOBALPTR =      8;   // RVA of GP
        public const int IMAGE_DIRECTORY_ENTRY_TLS =            9;   // TLS Directory
        public const int IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG =    10;  // Load Configuration Directory
        public const int IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT =   11;  // Bound Import Directory in headers
        public const int IMAGE_DIRECTORY_ENTRY_IAT =            12;  // Import Address Table
        public const int IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT =   13;  // Delay Load Import Descriptors
        public const int IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR = 14;  // COM Runtime descriptor

        public ImageDosHeader DosHeader = null;
        public ImageNTHeader NTHeader = null;
        public SectionHeader[] SectionHeaders = null;
        public CLIHeader CliHeader = null;
        public ImageResourceDirectory? ResourceHeader = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PEFile"/> struct.
        /// </summary>
        /// <param name="fileBytes">The file bytes.</param>
        /// <exception cref="ArgumentException">Parameter is null or zero length. - fileBytes</exception>
        /// <exception cref="InvalidProgramException">
        /// File does not have a valid IMAGE_DOS_HEADER.
        /// or
        /// File does not have a valid IMAGE_NT_HEADER.
        /// </exception>
        public PEFile(BinaryReader reader)
        {
            if (null == reader)
                return;

            // Initialize variables to allow calls to this instance.
            CliHeader = null;
            ResourceHeader = null;

            // Load the DOS header and verify.
            DosHeader = new ImageDosHeader(reader);
            if (false == DosHeader.IsDOS)
                return;

            // Load the NT header and verify.
            NTHeader = new ImageNTHeader(reader, DosHeader.NewHeaderAddress);
            if (false == NTHeader.IsNT)
                return;
           
            // Allocate the array to contain the SectionHeader instances.
            SectionHeaders = new SectionHeader[NTHeader.NumberOfSections];

            // Calculate the offset to read from.
            UInt32 sectionOffset = DosHeader.NewHeaderAddress + ImageNTHeader.SizeOfSignature + ImageFileHeader.SizeOf + NTHeader.SizeOfOptionalHeader;

            // Read each of the sections.
            for (int i = 0; i < SectionHeaders.Length; i++)
            {
                SectionHeaders[i] = new SectionHeader(reader, sectionOffset);
                sectionOffset += SectionHeader.SizeOfSectionHeader;
            }

            // Check for a CLI header.
            int fileOffset = GetDirectoryFileOffset(IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR);
            if (fileOffset > 0)
            {
                CliHeader = new CLIHeader(reader, (UInt32)fileOffset);
            }

            // Load the Resource header.
            fileOffset = GetDirectoryFileOffset(IMAGE_DIRECTORY_ENTRY_RESOURCE);
            if (fileOffset > 0)
            {
                ResourceHeader = new ImageResourceDirectory(reader, (UInt32)fileOffset);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is PE or not.
        /// </summary>
        public bool IsPe => this.DosHeader.IsDOS && this.NTHeader.IsNT;

        /// <summary>
        /// Gets the cli resource offset.
        /// </summary>
        /// <value>The cli resource offset.</value>
        /// <exception cref="System.InvalidProgramException"></exception>
        public UInt32 CliResourceOffset
        {
            get
            {
                if (null == CliHeader)
                    throw new InvalidProgramException();
                return RvaToFileOffset(CliHeader.ResourcesDirectoryAddress);
            }
        }

        /// <summary>
        /// Rvas to file offset.
        /// </summary>
        /// <param name="rva">The file offset of the Relative Virtual Address (RVA). Zero if something goes wrong.</param>
        /// <returns>UInt32.</returns>
        public UInt32 RvaToFileOffset(UInt32 rva)
        {
            SectionHeader? section = FindContainingSection(rva);
            if (section.HasValue)
            {
                var value = section.Value.PointerToRawData + (rva - section.Value.VirtualAddress);
                return value;
            }

            return 0;
        }

        /// <summary>
        /// Finds the containing section.
        /// </summary>
        /// <param name="rva">The rva.</param>
        /// <returns>SectionHeader instance or null if not found.</returns>
        public SectionHeader? FindContainingSection(UInt32 rva)
        {
            SectionHeader? section = null;
            foreach (var s in SectionHeaders)
            {
                if ((s.VirtualAddress <= rva) && (s.VirtualAddress + s.SizeOfRawData >= rva))
                {
                    section = s;
                    break;
                }
            }

            return section;
        }

        /// <summary>
        /// Gets the directory file offset.
        /// </summary>
        /// <param name="dataDirectoryOffset">The data directory offset.</param>
        /// <returns>FileOffset value for the directory, -1 if not found.</returns>
        public int GetDirectoryFileOffset(int dataDirectoryOffset)
        {
            // Get the desired data directory.
            ImageDataDirectory dd = NTHeader.GetDataDirectory(dataDirectoryOffset);

            // Find the section that contains the specified directory.
            SectionHeader? section = FindContainingSection(dd.VirtualAddress);

            // If we didn't find the section, return -1.
            if (false == section.HasValue)
                return -1;

            // Calculate the file offset.
            return (int)(section.Value.PointerToRawData + (dd.VirtualAddress - section.Value.VirtualAddress));
        }

        public string GetStringFromOffset(BinaryReader reader, UInt32 offset, UInt32 size)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            byte[] bytes = reader.ReadBytes((int) size);
            return Encoding.UTF8.GetString(bytes);
        }

        public ImageResourceDataEntry? FindSealDataEntry(BinaryReader reader, ImageResourceDirectory root, UInt32 resourceOffset)
        {
            var rcDataDirectory = root.RCDATADirectory;
            if (rcDataDirectory.HasValue)
            {
                uint rcDataOffset = resourceOffset + rcDataDirectory.Value.Offset;
                var ird = new ImageResourceDirectory(reader, rcDataOffset);

                uint nameOffset = 0;

                foreach (var dd in ird.NamedDirectoryEntries)
                {
                    nameOffset = resourceOffset + dd.NameOffset;
                    var irds = new ImageResourceDirectoryString(reader, nameOffset);
                    if ("AESEAL" == irds.Name)
                    {
                        uint lcidOffset = resourceOffset + dd.Offset;
                        var lciddd = new ImageResourceDirectory(reader, lcidOffset);
                        if (lciddd.NumberOfIdEntries > 0)
                        {
                            var sealOffset = resourceOffset + lciddd.DirectoryEntries[0].Offset;
                            return new ImageResourceDataEntry(reader, sealOffset);
                        }
                    }
                }
            }

            return null;
        }

        public ImageResourceDataEntry? FindVersionDataEntry(BinaryReader reader, ImageResourceDirectory root, UInt32 resourceOffset)
        {
            var rcDataDirectory = root.VersionDirectory;
            if (rcDataDirectory.HasValue)
            {
                uint rcDataOffset = resourceOffset + rcDataDirectory.Value.Offset;
                var ird = new ImageResourceDirectory(reader, rcDataOffset);

                foreach(var dd in ird.DirectoryEntries)
                {
                    if (1 == dd.IdOrName)
                    {
                        uint lcidOffset = resourceOffset + dd.Offset;
                        var lciddd = new ImageResourceDirectory(reader, lcidOffset);
                        if (lciddd.NumberOfIdEntries > 0)
                        {
                            var verOffset = resourceOffset + lciddd.DirectoryEntries[0].Offset;
                            return new ImageResourceDataEntry(reader, verOffset);
                        }
                    }
                }
            }

            return null;
        }
    }
}
