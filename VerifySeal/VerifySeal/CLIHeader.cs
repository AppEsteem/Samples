/****************************************************************************************
// <copyright file="CLIHeader.cs" company="AppEsteem Corporation">
// Copyright © 2018 All Rights Reserved
// </copyright>
****************************************************************************************/

using System;
using System.IO;
using System.Text;

namespace VerifySeal
{
    /// <summary>
    /// Models the CLI Header. This class cannot be inherited.
    /// </summary>
    public sealed class CLIHeader
    {
        public UInt32 HeaderSize;
        public UInt16 MajorRuntimeVersion;
        public UInt16 MinorRuntimeVersion;
        public UInt32 MetaDataDirectoryAddress;
        public UInt32 MetaDataDirectorySize;
        public UInt32 Flags;
        public UInt32 EntryPointToken;
        public UInt32 ResourcesDirectoryAddress;
        public UInt32 ResourcesDirectorySize;
        public UInt32 StrongNameSignatureAddress;
        public UInt32 StrongNameSignatureSize;
        public UInt32 CodeManagerTableAddress;
        public UInt32 CodeManagerTableSize;
        public UInt32 VTableFixupsAddress;
        public UInt32 VTableFixupsSize;
        public UInt32 ExportAddressTableJumpsAddress;
        public UInt32 ExportAddressTableJumpsSize;
        public UInt32 ManagedNativeHeaderAddress;
        public UInt32 ManagedNativeHeaderSize;

        public MetadataHeader MetadataHeader;

        /// <summary>
        /// Prevents a default instance of the <see cref="CLIHeader"/> class from being created.
        /// </summary>
        private CLIHeader() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CLIHeader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="offset">The offset.</param>
        public CLIHeader(BinaryReader reader, UInt32 offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            this.MetadataHeader = null;

            HeaderSize = reader.ReadUInt32();
            MajorRuntimeVersion = reader.ReadUInt16();
            MinorRuntimeVersion = reader.ReadUInt16();
            MetaDataDirectoryAddress = reader.ReadUInt32();
            MetaDataDirectorySize = reader.ReadUInt32();
            Flags = reader.ReadUInt32();
            EntryPointToken = reader.ReadUInt32();
            ResourcesDirectoryAddress = reader.ReadUInt32();
            ResourcesDirectorySize = reader.ReadUInt32();
            StrongNameSignatureAddress = reader.ReadUInt32();
            StrongNameSignatureSize = reader.ReadUInt32();
            CodeManagerTableAddress = reader.ReadUInt32();
            CodeManagerTableSize = reader.ReadUInt32();
            VTableFixupsAddress = reader.ReadUInt32();
            VTableFixupsSize = reader.ReadUInt32();
            ExportAddressTableJumpsAddress = reader.ReadUInt32();
            ExportAddressTableJumpsSize = reader.ReadUInt32();
            ManagedNativeHeaderAddress = reader.ReadUInt32();
            ManagedNativeHeaderSize = reader.ReadUInt32();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is managed.
        /// </summary>
        /// <value><c>true</c> if this instance is managed; otherwise, <c>false</c>.</value>
        public bool IsManaged
        {
            get
            {
                if ((0 == HeaderSize) && (0 == MajorRuntimeVersion))
                    return false;
                return true;
            }
        }

        /// <summary>
        /// Reads the metadata header and streams.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="file">The file.</param>
        /// <returns>CLIHeader.</returns>
        public CLIHeader ReadMetadataHeaderAndStreams(BinaryReader reader, PEFile file)
        {
            // Get the RVA for the metadata header.
            if (0 == MetaDataDirectoryAddress)
                return this;

            var metadataHeaderFileOffset = file.RvaToFileOffset(MetaDataDirectoryAddress);
            
            // Read the header.
            this.MetadataHeader = new MetadataHeader(reader, metadataHeaderFileOffset);

            return this;
        }
    }

    /// <summary>
    /// Models the CLI MetadataHeader. This class cannot be inherited.
    /// </summary>
    public sealed class MetadataHeader
    {
        public UInt32 Signature;
        public UInt16 MajorVersion;
        public UInt16 MinorVersion;
        public UInt32 Reserved01;
        public UInt32 VersionStringLength;
        public string VersionString;
        public UInt16 Flags;
        public UInt16 NumberOfStreams;

        /// <summary>
        /// Prevents a default instance of the <see cref="MetadataHeader"/> class from being created.
        /// </summary>
        private MetadataHeader() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataHeader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="offset">The offset.</param>
        public MetadataHeader(BinaryReader reader, UInt32 offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            Signature = reader.ReadUInt32();
            MajorVersion = reader.ReadUInt16();
            MinorVersion = reader.ReadUInt16();
            Reserved01 = reader.ReadUInt32();
            VersionStringLength = reader.ReadUInt32();
            VersionString = Encoding.ASCII.GetString(reader.ReadBytes((int)VersionStringLength));
            Flags = reader.ReadUInt16();
            NumberOfStreams = reader.ReadUInt16();
        }
    }
}
