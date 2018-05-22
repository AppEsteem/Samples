/****************************************************************************************
// <copyright file="VerifyPE.cs" company="AppEsteem Corporation">
// Copyright © 2018 All Rights Reserved
// </copyright>
****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace VerifySeal
{
    /// <summary>
    /// Models the VS_VERSIONINFO, VS_FIXEDFILEINFO structures.
    /// </summary>
    public sealed class VersionInfo
    {
        public readonly string FileName;
        public readonly string FileVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfo"/> class.
        /// Used when constructing a VersionInfo from a managed executable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        public VersionInfo(FileVersionInfo info)
        {
            if (null == info) throw new ArgumentNullException(nameof(info));

            FileName = (string.IsNullOrWhiteSpace(info.OriginalFilename)) ? info.FileName : info.OriginalFilename;
            FileVersion = info.FileVersion;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionInfo"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        public VersionInfo(BinaryReader reader, uint offset, uint size)
        {
            // Reposition the stream.
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            UInt16 wLength = reader.ReadUInt16();
            UInt16 wValueLength = reader.ReadUInt16();
            UInt16 wType = reader.ReadUInt16();

            // Read past the UTF-16 string "VS_VERSION_INFO".
            byte[] szKey = reader.ReadBytes(15 * 2);
            ReadPastZeroBytes(reader);

            // If the value length is greater than zero, then there is a VS_FIXEDFILEINFO structure.
            if (wValueLength > 0)
            {
                UInt32 dwSignature = reader.ReadUInt32();
                UInt32 dwStructVersion = reader.ReadUInt32();
                UInt16 dwFileVersionMS2 = reader.ReadUInt16();
                UInt16 dwFileVersionMS1 = reader.ReadUInt16();
                UInt16 dwFileVersionLS2 = reader.ReadUInt16();
                UInt16 dwFileVersionLS1 = reader.ReadUInt16();
                UInt32 dwProductVersionMS = reader.ReadUInt32();
                UInt32 dwProductVersionLS = reader.ReadUInt32();
                UInt32 dwFileFlagsMask = reader.ReadUInt32();
                UInt32 dwFileFlags = reader.ReadUInt32();
                UInt32 dwFileOS = reader.ReadUInt32();
                UInt32 dwFileType = reader.ReadUInt32();
                UInt32 dwFileSubtype = reader.ReadUInt32();
                UInt32 dwFileDateMS = reader.ReadUInt32();
                UInt32 dwFileDateLS = reader.ReadUInt32();

                // Create a string based version from the binary one.
                FileVersion = $"{dwFileVersionMS1}.{dwFileVersionMS2}.{dwFileVersionLS1}.{dwFileVersionLS2}";
            }
            ReadPastZeroBytes(reader);

            // Read the size of the StringTable entry, then read all of the bytes in the StringTable.
            wLength = reader.ReadUInt16();
            byte[] contents = reader.ReadBytes(wLength - 2);

            FileName = SearchForStringValue(contents, "OriginalFilename");

        }

        public VersionInfo(byte[] content)
        {
            FileVersion = SearchForStringValue(content, "FileVersion");
            FileName = SearchForStringValue(content, "OriginalFilename");
        }

        /// <summary>
        /// Reads the past zero bytes.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private void ReadPastZeroBytes(BinaryReader reader)
        {
            while(0 == reader.PeekChar())
            {
                reader.ReadByte();
            }
        }

        /// <summary>
        /// Searches for string value in the byte array.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        private string SearchForStringValue(byte[] contents, string key)
        {
            // Check the passed arguments.
            if (null == contents) return string.Empty;
            if (string.IsNullOrWhiteSpace(key)) return string.Empty;

            // Convert the string into a byte array to match.
            byte[] match = Encoding.Unicode.GetBytes(key);
            if (contents.Length < match.Length) return string.Empty;

            int offset = 0;
            int matchOffset = 0;

            // Find the match.
            for(int i=0; i < contents.Length; i++)
            {
                // If the content match, advance the match offset.
                if (contents[i] == match[matchOffset])
                {
                    matchOffset++;
                    if (matchOffset == match.Length)
                    {
                        offset = i;
                        break;
                    }
                }
                else
                    matchOffset = 0;
            }

            // If the offset isn't zero, then it contains the end position of the match within the content array.
            if (offset != 0)
            {
                // Advance past any null padding bytes.
                while (0 == contents[offset])
                    offset++;

                // Find the length of the string value, it's null terminated. It's a UTF-16 string, so skip the second byte as it will be null.
                int length = 0;
                while (0 != contents[offset + length])
                    length += 2;

                // Convert the next bytes to a string.
                return Encoding.Unicode.GetString(contents, offset, length);
            }

            return string.Empty;
        }
    }
}
