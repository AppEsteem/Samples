/****************************************************************************************
// <copyright file="ResourceHeader.cs" company="AppEsteem Corporation">
// Copyright © 2018 All Rights Reserved
// </copyright>
****************************************************************************************/

using System;
using System.IO;
using System.Text;

namespace VerifySeal
{
    public struct ImageResourceDirectory
    {
        public UInt32 Characteristics;
        public UInt32 TimeDateStamp;
        public UInt16 MajorVersion;
        public UInt16 MinorVersion;
        public UInt16 NumberOfNamedEntries;
        public UInt16 NumberOfIdEntries;

        public ImageResourceDirectoryEntry[] NamedDirectoryEntries;
        public ImageResourceDirectoryEntry[] DirectoryEntries;

        public ImageResourceDirectory(BinaryReader reader, UInt32 offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            Characteristics = reader.ReadUInt32();
            TimeDateStamp = reader.ReadUInt32();
            MajorVersion = reader.ReadUInt16();
            MinorVersion = reader.ReadUInt16();
            NumberOfNamedEntries = reader.ReadUInt16();
            NumberOfIdEntries = reader.ReadUInt16();

            NamedDirectoryEntries = new ImageResourceDirectoryEntry[NumberOfNamedEntries];
            DirectoryEntries = new ImageResourceDirectoryEntry[NumberOfIdEntries];

            for (int i = 0; i < NumberOfNamedEntries; i++)
                NamedDirectoryEntries[i] = new ImageResourceDirectoryEntry(reader);

            for (int i = 0; i < NumberOfIdEntries; i++)
                DirectoryEntries[i] = new ImageResourceDirectoryEntry(reader);
        }

        /// <summary>
        /// Gets the RCDATA directory.
        /// </summary>
        public ImageResourceDirectoryEntry? RCDATADirectory
        {
            get
            {
                for(int i=0; i < DirectoryEntries.Length; i++)
                {
                    if ((false == DirectoryEntries[i].IsName) && (10 == DirectoryEntries[i].IdOrName))
                        return DirectoryEntries[i];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the version directory.
        /// </summary>
        /// <value>The version directory.</value>
        public ImageResourceDirectoryEntry? VersionDirectory
        {
            get
            {
                for(int i=0; i< DirectoryEntries.Length; i++)
                {
                    if ((false == DirectoryEntries[i].IsName) && (16 == DirectoryEntries[i].IdOrName))
                        return DirectoryEntries[i];
                }

                return null;
            }
        }
    }

    public struct ImageResourceDirectoryEntry
    {
        public UInt32 IdOrName;
        public UInt32 OffsetToData;

        public ImageResourceDirectoryEntry(BinaryReader reader)
        {
            IdOrName = reader.ReadUInt32();
            OffsetToData = reader.ReadUInt32();
        }

        public UInt32 NameOffset { get { return (IdOrName & 0x7FFFFFFF); } }
        public UInt32 Id { get { return IdOrName; } }
        public UInt32 Offset { get { return (OffsetToData & 0x7FFFFFFF); } }
        public bool IsName { get { return (IdOrName & 0x80000000) > 0; } }
        public bool IsOffsetToDirectory { get { return (Offset &  0x80000000) > 0; } }
    }

    public struct ImageResourceDirectoryString
    {
        public UInt16 Length;
        public byte[] NameBytes;

        public ImageResourceDirectoryString(BinaryReader reader, UInt32 offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            Length = reader.ReadUInt16();
            NameBytes = reader.ReadBytes(Length * 2);   // UTF-16 characters are each two bytes.
        }

        public string Name
        {
            get
            {
                if ((null == NameBytes) || (0 == NameBytes.Length))
                    return string.Empty;

                return Encoding.Unicode.GetString(NameBytes, 0, Length * 2);    // UTF-16 characters are each two bytes.
            }
        }
    }

    public struct ImageResourceDataEntry
    {
        public UInt32 OffsetToData;
        public UInt32 Size;
        public UInt32 CodePage;
        public UInt32 Reserved;

        public ImageResourceDataEntry(BinaryReader reader, UInt32 offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            OffsetToData = reader.ReadUInt32();
            Size = reader.ReadUInt32();
            CodePage = reader.ReadUInt32();
            Reserved = reader.ReadUInt32();
        }
    }
}
