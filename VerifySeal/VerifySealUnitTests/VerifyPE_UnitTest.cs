/****************************************************************************************
// <copyright file="VerifyPE_UnitTest.cs" company="AppEsteem Corporation">
// Copyright © 2018 All Rights Reserved
// </copyright>
****************************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using VerifySeal;

namespace VerifySealUnitTests
{
    [TestClass]
    public class VerifyPE_UnitTest
    {
        [TestMethod]
        public void Managedx86_Test()
        {
            string filePath = Path.GetFullPath(@"..\..\..\TestFiles\Managed_x86.exe");

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // If this is a PE file, output the PE specific information.
                VerifyPE pe = new VerifyPE(reader);

                Assert.IsTrue(pe.IsPe);
                Assert.IsTrue(pe.Is32Bit);
                Assert.IsTrue(pe.IsManaged);
                Assert.AreNotEqual(default(DateTimeOffset), pe.FileCreation);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pe.SealJson));
            }
        }

        [TestMethod]
        public void Managedx64_Test()
        {
            string filePath = Path.GetFullPath(@"..\..\..\TestFiles\Managed_x64.exe");

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // If this is a PE file, output the PE specific information.
                VerifyPE pe = new VerifyPE(reader);

                Assert.IsTrue(pe.IsPe);
                Assert.IsFalse(pe.Is32Bit);
                Assert.IsTrue(pe.IsManaged);
                Assert.AreNotEqual(default(DateTimeOffset), pe.FileCreation);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pe.SealJson));
            }
        }

        [TestMethod]
        public void Nativex86_Test()
        {
            string filePath = Path.GetFullPath(@"..\..\..\TestFiles\Native_x86.exe");

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // If this is a PE file, output the PE specific information.
                VerifyPE pe = new VerifyPE(reader);

                Assert.IsTrue(pe.IsPe);
                Assert.IsTrue(pe.Is32Bit);
                Assert.IsFalse(pe.IsManaged);
                Assert.AreNotEqual(default(DateTimeOffset), pe.FileCreation);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pe.SealJson));

                Program.PrintSealInformation(pe.SealJson);
                Assert.IsTrue(Program.ValidateSeal(pe.SealJson, default(DateTimeOffset), "IEInstaller.exe", " 4. 2. 0. 10"));
            }
        }

        [TestMethod]
        public void DelphiSample_Test()
        {
            string filePath = Path.GetFullPath(@"..\..\..\TestFiles\DelphiSample.exe");

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // If this is a PE file, output the PE specific information.
                VerifyPE pe = new VerifyPE(reader);

                Assert.IsTrue(pe.IsPe);
                Assert.IsTrue(pe.Is32Bit);
                Assert.IsFalse(pe.IsManaged);
                Assert.AreNotEqual(default(DateTimeOffset), pe.FileCreation);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pe.SealJson));

                Program.PrintSealInformation(pe.SealJson);
                Assert.IsTrue(Program.ValidateSeal(pe.SealJson, default(DateTimeOffset), "DelphiSample.exe", " 1.0.0.1"));  // This fails with the current version of DelphiSample.exe because the seal file name doesn't match.
            }
        }

        [TestMethod]
        public void Nativex64_Test()
        {
            string filePath = Path.GetFullPath(@"..\..\..\TestFiles\Native_x64.exe");

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // If this is a PE file, output the PE specific information.
                VerifyPE pe = new VerifyPE(reader);

                // Verify the DOS header values.
                Assert.AreEqual(0x5A4D, pe._pe.DosHeader.Signature);
                Assert.AreEqual(0x0090, pe._pe.DosHeader.BytesOnLastPage);
                Assert.AreEqual(0x0003, pe._pe.DosHeader.PagesInFile);
                Assert.AreEqual(0x0000, pe._pe.DosHeader.Relocations);
                Assert.AreEqual(0x0004, pe._pe.DosHeader.SizeOfHeader);
                Assert.AreEqual(0x0000, pe._pe.DosHeader.MinExtraParagraphs);
                Assert.AreEqual(0xFFFF, pe._pe.DosHeader.MaxExtraParagraphs);
                Assert.AreEqual(0x0000, pe._pe.DosHeader.InitialSS);
                Assert.AreEqual(0x00B8, pe._pe.DosHeader.InitialSP);
                Assert.AreEqual(0x0000, pe._pe.DosHeader.Checksum);
                Assert.AreEqual(0x0000, pe._pe.DosHeader.InitialIP);
                Assert.AreEqual(0x0000, pe._pe.DosHeader.InitialCS);
                Assert.AreEqual(0x0040, pe._pe.DosHeader.RelocTableAddress);
                Assert.AreEqual(0x0000, pe._pe.DosHeader.OverlayNumber);
                Assert.AreEqual(0x0000, pe._pe.DosHeader.OEMIdentifier);
                Assert.AreEqual(0x0000, pe._pe.DosHeader.OEMInformation);
                Assert.AreEqual<UInt32>(0x0100, pe._pe.DosHeader.NewHeaderAddress);

                // Verify the NT Header values.
                Assert.AreEqual<UInt32>(0x00004550, pe._pe.NTHeader.Signature);

                // Verify the File Header values.
                Assert.AreEqual(0x8664, pe._pe.NTHeader.FileHeader.Machine);
                Assert.AreEqual(0x000A, pe._pe.NTHeader.FileHeader.NumberOfSections);
                Assert.AreEqual<UInt32>(0x5ACEE413, pe._pe.NTHeader.FileHeader.TimeDateStamp);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.FileHeader.PointerToSymbolTable);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.FileHeader.NumberOfSymbols);
                Assert.AreEqual(0x00F0, pe._pe.NTHeader.FileHeader.SizeOfOptionalHeader);
                Assert.AreEqual(0x0022, pe._pe.NTHeader.FileHeader.Characteristics);

                // Verify the Optional Header values.
                Assert.IsFalse(pe._pe.NTHeader.OptionalHeader32.HasValue);
                Assert.IsTrue(pe._pe.NTHeader.OptionalHeader64.HasValue);
                Assert.AreEqual(0x020B, pe._pe.NTHeader.OptionalHeader64.Value.Magic);
                Assert.AreEqual(0x0E, pe._pe.NTHeader.OptionalHeader64.Value.MajorLinkerVersion);
                Assert.AreEqual(0x00, pe._pe.NTHeader.OptionalHeader64.Value.MinorLinkerVersion);
                Assert.AreEqual<UInt32>(0x00007A00, pe._pe.NTHeader.OptionalHeader64.Value.SizeOfCode);
                Assert.AreEqual<UInt32>(0x00009400, pe._pe.NTHeader.OptionalHeader64.Value.SizeOfInitializedData);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.SizeOfUninitializedData);
                Assert.AreEqual<UInt32>(0x0001100F, pe._pe.NTHeader.OptionalHeader64.Value.AddressOfEntryPoint);
                Assert.AreEqual<UInt32>(0x00001000, pe._pe.NTHeader.OptionalHeader64.Value.BaseOfCode);
                Assert.AreEqual<UInt64>(0x0000000140000000, pe._pe.NTHeader.OptionalHeader64.Value.ImageBase);
                Assert.AreEqual<UInt32>(0x00001000, pe._pe.NTHeader.OptionalHeader64.Value.SectionAlignment);
                Assert.AreEqual<UInt32>(0x00000200, pe._pe.NTHeader.OptionalHeader64.Value.FileAlignment);
                Assert.AreEqual(0x0006, pe._pe.NTHeader.OptionalHeader64.Value.MajorOperatingSystemVersion);
                Assert.AreEqual(0x0000, pe._pe.NTHeader.OptionalHeader64.Value.MinorOperatingSystemVersion);
                Assert.AreEqual(0x0000, pe._pe.NTHeader.OptionalHeader64.Value.MinorImageVersion);
                Assert.AreEqual(0x0000, pe._pe.NTHeader.OptionalHeader64.Value.MinorImageVersion);
                Assert.AreEqual(0x0006, pe._pe.NTHeader.OptionalHeader64.Value.MajorSubsystemVersion);
                Assert.AreEqual(0x0000, pe._pe.NTHeader.OptionalHeader64.Value.MinorSubsystemVersion);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.Win32VersionValue);
                Assert.AreEqual<UInt32>(0x00027000, pe._pe.NTHeader.OptionalHeader64.Value.SizeOfImage);
                Assert.AreEqual<UInt32>(0x00000400, pe._pe.NTHeader.OptionalHeader64.Value.SizeOfHeaders);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.CheckSum);
                Assert.AreEqual(0x0003, pe._pe.NTHeader.OptionalHeader64.Value.Subsystem);
                Assert.AreEqual(0x8160, pe._pe.NTHeader.OptionalHeader64.Value.DllCharacteristics);
                Assert.AreEqual<UInt64>(0x0000000000100000, pe._pe.NTHeader.OptionalHeader64.Value.SizeOfStackReserve);
                Assert.AreEqual<UInt64>(0x0000000000001000, pe._pe.NTHeader.OptionalHeader64.Value.SizeOfStackCommit);
                Assert.AreEqual<UInt64>(0x0000000000100000, pe._pe.NTHeader.OptionalHeader64.Value.SizeOfHeapReserve);
                Assert.AreEqual<UInt64>(0x0000000000001000, pe._pe.NTHeader.OptionalHeader64.Value.SizeOfHeapCommit);
                Assert.AreEqual<UInt32>(0x0000, pe._pe.NTHeader.OptionalHeader64.Value.LoaderFlags);
                Assert.AreEqual<UInt32>(0x0010, pe._pe.NTHeader.OptionalHeader64.Value.NumberOfRvaAndSizes);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[0].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[0].Size);
                Assert.AreEqual<UInt32>(0x000203B8, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[1].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000050, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[1].Size);
                Assert.AreEqual<UInt32>(0x00023000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[2].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00002219, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[2].Size);
                Assert.AreEqual<UInt32>(0x0001D000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[3].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00001D40, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[3].Size);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[4].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[4].Size);
                Assert.AreEqual<UInt32>(0x00026000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[5].VirtualAddress);
                Assert.AreEqual<UInt32>(0x0000005C, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[5].Size);
                Assert.AreEqual<UInt32>(0x0001A7E0, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[6].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000038, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[6].Size);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[7].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[7].Size);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[8].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[8].Size);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[9].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[9].Size);
                Assert.AreEqual<UInt32>(0x0001A820, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[10].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000094, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[10].Size);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[11].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[11].Size);
                Assert.AreEqual<UInt32>(0x00020000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[12].VirtualAddress);
                Assert.AreEqual<UInt32>(0x000003B8, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[12].Size);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[13].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[13].Size);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[14].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[14].Size);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[15].VirtualAddress);
                Assert.AreEqual<UInt32>(0x00000000, pe._pe.NTHeader.OptionalHeader64.Value.DataDirectory[15].Size);

                Assert.IsTrue(pe.IsPe);
                Assert.IsFalse(pe.Is32Bit);
                Assert.IsFalse(pe.IsManaged);
                Assert.AreNotEqual(default(DateTimeOffset), pe.FileCreation);
                Assert.IsFalse(string.IsNullOrWhiteSpace(pe.SealJson));
            }
        }


    }
}
