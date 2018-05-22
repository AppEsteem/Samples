/****************************************************************************************
// <copyright file="SectionHeader.cs" company="AppEsteem Corporation">
// Copyright © 2018 All Rights Reserved
// </copyright>
****************************************************************************************/

using System;
using System.IO;
using System.Text;

namespace VerifySeal
{
    /// <summary>
    /// Models an IMAGE_SECTION_HEADER structure.
    /// </summary>
    public struct SectionHeader
    {
        public const int SizeOfSectionHeader = 40;

        public readonly string Name;
        public readonly UInt32 VirtualSize;
        public readonly UInt32 VirtualAddress;
        public readonly UInt32 SizeOfRawData;
        public readonly UInt32 PointerToRawData;
        public readonly UInt32 PointerToRelocations;
        public readonly UInt32 PointerToLineNumbers;
        public readonly UInt16 NumberOfRelocations;
        public readonly UInt16 NumberOfLineNumbers;
        public readonly UInt32 Characteristics;

        public SectionHeader(BinaryReader reader, UInt32 offset)
        {
            // The name is constrained to 8 bytes at offset zero.
            Name = Encoding.ASCII.GetString(reader.ReadBytes(8), 0, 8);
            VirtualSize = reader.ReadUInt32();
            VirtualAddress = reader.ReadUInt32();
            SizeOfRawData = reader.ReadUInt32();
            PointerToRawData = reader.ReadUInt32();
            PointerToRelocations = reader.ReadUInt32();
            PointerToLineNumbers = reader.ReadUInt32();
            NumberOfRelocations = reader.ReadUInt16();
            NumberOfLineNumbers = reader.ReadUInt16();
            Characteristics = reader.ReadUInt32();
        }
    }

}
