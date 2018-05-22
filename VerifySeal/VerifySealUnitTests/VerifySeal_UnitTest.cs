/****************************************************************************************
// <copyright file="VerifyJson_UnitTest.cs" company="AppEsteem Corporation">
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
    public class VerifySeal_UnitTest
    {
        [TestMethod]
        public void ValidSeal_Test()
        {
            string filePath = Path.GetFullPath(@"..\..\..\TestFiles\ValidSeal.json");

            VerifyJson verifyJson = null;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // If this is a PE file, output the PE specific information.
                verifyJson = new VerifyJson(reader);

                Assert.IsTrue(verifyJson.IsJson);
                Assert.IsNotNull(verifyJson.SealJson);
            }

            Assert.IsNotNull(verifyJson);
            Assert.IsTrue(Program.ValidateSeal(verifyJson.SealJson, default(DateTimeOffset)));
        }

        [TestMethod]
        public void SealWithNoSignature_Test()
        {
            string filePath = Path.GetFullPath(@"..\..\..\TestFiles\SealWithNoSignature.json");

            VerifyJson verifyJson = null;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // If this is a PE file, output the PE specific information.
                verifyJson = new VerifyJson(reader);

                Assert.IsTrue(verifyJson.IsJson);
                Assert.IsNotNull(verifyJson.SealJson);
            }

            Assert.IsNotNull(verifyJson);
            Assert.IsFalse(Program.ValidateSeal(verifyJson.SealJson, default(DateTimeOffset), "filename", "fileversion"));
        }
    }
}
