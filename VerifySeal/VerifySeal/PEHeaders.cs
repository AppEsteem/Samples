/****************************************************************************************
// <copyright file="PEHeaders.cs" company="AppEsteem Corporation">
// Copyright © 2018 All Rights Reserved
// </copyright>
****************************************************************************************/

using System;
using System.IO;
using System.Text;

namespace VerifySeal
{
    /// <summary>
    /// DOS Image Header.
    /// </summary>
    public sealed class ImageDosHeader
    {
        const UInt16 Magic_DOS = 0x5A4D;

        public UInt16 Signature;
        public UInt16 BytesOnLastPage;                               
        public UInt16 PagesInFile;                                   
        public UInt16 Relocations;                                   
        public UInt16 SizeOfHeader;                                  
        public UInt16 MinExtraParagraphs;                            
        public UInt16 MaxExtraParagraphs;                            
        public UInt16 InitialSS;                                     
        public UInt16 InitialSP;                                     
        public UInt16 Checksum;                                      
        public UInt16 InitialIP;                                     
        public UInt16 InitialCS;                                     
        public UInt16 RelocTableAddress;                             
        public UInt16 OverlayNumber;                                 
        public byte[] Reserved01; 
        public UInt16 OEMIdentifier;
        public UInt16 OEMInformation;
        public byte[] Reserved02;
        public UInt32 NewHeaderAddress;

        public ImageDosHeader(BinaryReader reader)
        {
            if (null == reader) throw new ArgumentNullException(nameof(reader));

            // Ensure we're reading at the start of the file.
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            // Read in each of the member values.
            Signature = reader.ReadUInt16();
            BytesOnLastPage = reader.ReadUInt16();
            PagesInFile = reader.ReadUInt16();
            Relocations = reader.ReadUInt16();
            SizeOfHeader = reader.ReadUInt16();
            MinExtraParagraphs = reader.ReadUInt16();
            MaxExtraParagraphs = reader.ReadUInt16();
            InitialSS = reader.ReadUInt16();
            InitialSP = reader.ReadUInt16();
            Checksum = reader.ReadUInt16();
            InitialIP = reader.ReadUInt16();
            InitialCS = reader.ReadUInt16();
            RelocTableAddress = reader.ReadUInt16();
            Reserved01 = reader.ReadBytes(8);
            OverlayNumber = reader.ReadUInt16();
            OEMIdentifier = reader.ReadUInt16();
            OEMInformation = reader.ReadUInt16();
            Reserved02 = reader.ReadBytes(20);
            NewHeaderAddress = reader.ReadUInt32();
        }

        public bool IsDOS { get { return (Magic_DOS == Signature); } }
    }

    /// <summary>
    /// ImageNTHeader structure. Models the Windows IMAGE_NT_HEADER structure.
    /// </summary>
    public sealed class ImageNTHeader
    {
        public const int SizeOfSignature = 4;
        const UInt32 Magic_NT = 0x00004550;
        const int IMAGE_FILE_MACHINE_I386 = 0x014c;
        const int IMAGE_FILE_MACHINE_AMD64 = 0x8664;
        const int IMAGE_FILE_MACHINE_IA64 = 0x0200;

        public UInt32 Signature;
        public ImageFileHeader FileHeader;
        public ImageOptionalHeader32? OptionalHeader32;
        public ImageOptionalHeader64? OptionalHeader64;

        public ImageNTHeader(BinaryReader reader, UInt32 offset)
        {
            // Position the reader to the start of the new header.
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            Signature = reader.ReadUInt32();
            FileHeader = new ImageFileHeader(reader);
            OptionalHeader32 = null;
            OptionalHeader64 = null;

            // Create the correct Optional Header structure.
            if (Is32Bit)
                OptionalHeader32 = new ImageOptionalHeader32(reader, offset + SizeOfSignature + ImageFileHeader.SizeOf);
            else if (Is64Bit)
                OptionalHeader64 = new ImageOptionalHeader64(reader, offset + SizeOfSignature + ImageFileHeader.SizeOf);
            else
                throw new InvalidProgramException("File is not a 32 or 64 bit PE file.");
        }

        public bool IsNT { get { return Magic_NT == Signature; } }
        public bool Is32Bit { get { return IMAGE_FILE_MACHINE_I386 == FileHeader.Machine; } }
        public bool Is64Bit { get { return ((IMAGE_FILE_MACHINE_AMD64 == FileHeader.Machine) || (IMAGE_FILE_MACHINE_IA64 == FileHeader.Machine)); } }       
        public UInt16 NumberOfSections { get { return FileHeader.NumberOfSections; } }
        public UInt16 SizeOfOptionalHeader { get { return FileHeader.SizeOfOptionalHeader; } }

        public UInt32 SectionAlignment { get { return (OptionalHeader32.HasValue) ? OptionalHeader32.Value.SectionAlignment : OptionalHeader64.Value.SectionAlignment; } }
        public UInt32 FileAlignment { get { return (OptionalHeader32.HasValue) ? OptionalHeader32.Value.FileAlignment : OptionalHeader64.Value.FileAlignment; } }
        public ImageDataDirectory GetDataDirectory(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));

            if (OptionalHeader32.HasValue)
            {
                if (index >= OptionalHeader32.Value.DataDirectory.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return OptionalHeader32.Value.DataDirectory[index];
            }
            else if (OptionalHeader64.HasValue)
            {
                if (index >= OptionalHeader64.Value.DataDirectory.Length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return OptionalHeader64.Value.DataDirectory[index];
            }
            else
                throw new InvalidProgramException();
        }
    }

    /// <summary>
    /// ImageFileHeader structure. Models the Windows IMAGE_FILE_HEADER.
    /// </summary>
    public struct ImageFileHeader
    {
        public const int SizeOf = 20;

        public UInt16 Machine;
        public UInt16 NumberOfSections;
        public UInt32 TimeDateStamp;
        public UInt32 PointerToSymbolTable;
        public UInt32 NumberOfSymbols;
        public UInt16 SizeOfOptionalHeader;
        public UInt16 Characteristics;

        public ImageFileHeader(BinaryReader reader)
        {
            Machine = reader.ReadUInt16();
            NumberOfSections = reader.ReadUInt16();
            TimeDateStamp = reader.ReadUInt32();
            PointerToSymbolTable = reader.ReadUInt32();
            NumberOfSymbols = reader.ReadUInt32();
            SizeOfOptionalHeader = reader.ReadUInt16();
            Characteristics = reader.ReadUInt16();
        }
    }

    /// <summary>
    /// ImageDataDirectory structure, models the Windows IMAGE_DATA_DIRECTORY.
    /// </summary>
    public struct ImageDataDirectory
    {
        public UInt32 VirtualAddress;
        public UInt32 Size;

        public ImageDataDirectory(BinaryReader reader)
        {
            VirtualAddress = reader.ReadUInt32();
            Size = reader.ReadUInt32();
        }
    }

    public struct ImageOptionalHeader32
    {
        // Standard Fields.
        public UInt16 Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public UInt32 SizeOfCode;
        public UInt32 SizeOfInitializedData;
        public UInt32 SizeOfUninitializedData;
        public UInt32 AddressOfEntryPoint;
        public UInt32 BaseOfCode;
        public UInt32 BaseOfData;

        // NT Additional Fields.
        public UInt32 ImageBase;
        public UInt32 SectionAlignment;
        public UInt32 FileAlignment;
        public UInt16 MajorOperatingSystemVersion;
        public UInt16 MinorOperatingSystemVersion;
        public UInt16 MajorImageVersion;
        public UInt16 MinorImageVersion;
        public UInt16 MajorSubsystemVersion;
        public UInt16 MinorSubsystemVersion;
        public UInt32 Win32VersionValue;
        public UInt32 SizeOfImage;
        public UInt32 SizeOfHeaders;
        public UInt32 CheckSum;
        public UInt16 Subsystem;
        public UInt16 DllCharacteristics;
        public UInt32 SizeOfStackReserve;
        public UInt32 SizeOfStackCommit;
        public UInt32 SizeOfHeapReserve;
        public UInt32 SizeOfHeapCommit;
        public UInt32 LoaderFlags;
        public UInt32 NumberOfRvaAndSizes;
        public ImageDataDirectory[] DataDirectory;

        public ImageOptionalHeader32(BinaryReader reader, UInt32 offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            // Standard fields.
            Magic = reader.ReadUInt16();
            MajorLinkerVersion = reader.ReadByte();
            MinorLinkerVersion = reader.ReadByte();
            SizeOfCode = reader.ReadUInt32();
            SizeOfInitializedData = reader.ReadUInt32();
            SizeOfUninitializedData = reader.ReadUInt32();
            AddressOfEntryPoint = reader.ReadUInt32();
            BaseOfCode = reader.ReadUInt32();
            BaseOfData = reader.ReadUInt32();

            // NT additional fields.
            ImageBase = reader.ReadUInt32();
            SectionAlignment = reader.ReadUInt32();
            FileAlignment = reader.ReadUInt32();
            MajorOperatingSystemVersion = reader.ReadUInt16();
            MinorOperatingSystemVersion = reader.ReadUInt16();
            MajorImageVersion = reader.ReadUInt16();
            MinorImageVersion = reader.ReadUInt16();
            MajorSubsystemVersion = reader.ReadUInt16();
            MinorSubsystemVersion = reader.ReadUInt16();
            Win32VersionValue = reader.ReadUInt32();
            SizeOfImage = reader.ReadUInt32();
            SizeOfHeaders = reader.ReadUInt32();
            CheckSum = reader.ReadUInt32();
            Subsystem = reader.ReadUInt16();
            DllCharacteristics = reader.ReadUInt16();
            SizeOfStackReserve = reader.ReadUInt32();
            SizeOfStackCommit = reader.ReadUInt32();
            SizeOfHeapReserve = reader.ReadUInt32();
            SizeOfHeapCommit = reader.ReadUInt32();
            LoaderFlags = reader.ReadUInt32();
            NumberOfRvaAndSizes = reader.ReadUInt32();

            // Load the data directories.
            DataDirectory = new ImageDataDirectory[16];
            DataDirectory[0] = new ImageDataDirectory(reader);
            DataDirectory[1] = new ImageDataDirectory(reader);
            DataDirectory[2] = new ImageDataDirectory(reader);
            DataDirectory[3] = new ImageDataDirectory(reader);
            DataDirectory[4] = new ImageDataDirectory(reader);
            DataDirectory[5] = new ImageDataDirectory(reader);
            DataDirectory[6] = new ImageDataDirectory(reader);
            DataDirectory[7] = new ImageDataDirectory(reader);
            DataDirectory[8] = new ImageDataDirectory(reader);
            DataDirectory[9] = new ImageDataDirectory(reader);
            DataDirectory[10] = new ImageDataDirectory(reader);
            DataDirectory[11] = new ImageDataDirectory(reader);
            DataDirectory[12] = new ImageDataDirectory(reader);
            DataDirectory[13] = new ImageDataDirectory(reader);
            DataDirectory[14] = new ImageDataDirectory(reader);
            DataDirectory[15] = new ImageDataDirectory(reader);
        }
    }

    public struct ImageOptionalHeader64
    {
        // Standard Fields.
        public UInt16 Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public UInt32 SizeOfCode;
        public UInt32 SizeOfInitializedData;
        public UInt32 SizeOfUninitializedData;
        public UInt32 AddressOfEntryPoint;
        public UInt32 BaseOfCode;

        // NT Additional Fields.
        public UInt64 ImageBase;
        public UInt32 SectionAlignment;
        public UInt32 FileAlignment;
        public UInt16 MajorOperatingSystemVersion;
        public UInt16 MinorOperatingSystemVersion;
        public UInt16 MajorImageVersion;
        public UInt16 MinorImageVersion;
        public UInt16 MajorSubsystemVersion;
        public UInt16 MinorSubsystemVersion;
        public UInt32 Win32VersionValue;
        public UInt32 SizeOfImage;
        public UInt32 SizeOfHeaders;
        public UInt32 CheckSum;
        public UInt16 Subsystem;
        public UInt16 DllCharacteristics;
        public UInt64 SizeOfStackReserve;
        public UInt64 SizeOfStackCommit;
        public UInt64 SizeOfHeapReserve;
        public UInt64 SizeOfHeapCommit;
        public UInt32 LoaderFlags;
        public UInt32 NumberOfRvaAndSizes;
        public ImageDataDirectory[] DataDirectory;

        public ImageOptionalHeader64(BinaryReader reader, UInt32 offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            // Standard fields.
            Magic = reader.ReadUInt16();
            MajorLinkerVersion = reader.ReadByte();
            MinorLinkerVersion = reader.ReadByte();
            SizeOfCode = reader.ReadUInt32();
            SizeOfInitializedData = reader.ReadUInt32();
            SizeOfUninitializedData = reader.ReadUInt32();
            AddressOfEntryPoint = reader.ReadUInt32();
            BaseOfCode = reader.ReadUInt32();

            // NY additional fields.
            ImageBase = reader.ReadUInt64();
            SectionAlignment = reader.ReadUInt32();
            FileAlignment = reader.ReadUInt32();
            MajorOperatingSystemVersion = reader.ReadUInt16();
            MinorOperatingSystemVersion = reader.ReadUInt16();
            MajorImageVersion = reader.ReadUInt16();
            MinorImageVersion = reader.ReadUInt16();
            MajorSubsystemVersion = reader.ReadUInt16();
            MinorSubsystemVersion = reader.ReadUInt16();
            Win32VersionValue = reader.ReadUInt32();
            SizeOfImage = reader.ReadUInt32();
            SizeOfHeaders = reader.ReadUInt32();
            CheckSum = reader.ReadUInt32();
            Subsystem = reader.ReadUInt16();
            DllCharacteristics = reader.ReadUInt16();
            SizeOfStackReserve = reader.ReadUInt64();
            SizeOfStackCommit = reader.ReadUInt64();
            SizeOfHeapReserve = reader.ReadUInt64();
            SizeOfHeapCommit = reader.ReadUInt64();
            LoaderFlags = reader.ReadUInt32();
            NumberOfRvaAndSizes = reader.ReadUInt32();

            // Load the data directories.
            DataDirectory = new ImageDataDirectory[16];
            DataDirectory[0] = new ImageDataDirectory(reader);
            DataDirectory[1] = new ImageDataDirectory(reader);
            DataDirectory[2] = new ImageDataDirectory(reader);
            DataDirectory[3] = new ImageDataDirectory(reader);
            DataDirectory[4] = new ImageDataDirectory(reader);
            DataDirectory[5] = new ImageDataDirectory(reader);
            DataDirectory[6] = new ImageDataDirectory(reader);
            DataDirectory[7] = new ImageDataDirectory(reader);
            DataDirectory[8] = new ImageDataDirectory(reader);
            DataDirectory[9] = new ImageDataDirectory(reader);
            DataDirectory[10] = new ImageDataDirectory(reader);
            DataDirectory[11] = new ImageDataDirectory(reader);
            DataDirectory[12] = new ImageDataDirectory(reader);
            DataDirectory[13] = new ImageDataDirectory(reader);
            DataDirectory[14] = new ImageDataDirectory(reader);
            DataDirectory[15] = new ImageDataDirectory(reader);
        }
    }
}
