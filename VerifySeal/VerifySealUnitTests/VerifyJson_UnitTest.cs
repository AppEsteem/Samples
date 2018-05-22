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
    public class VerifyJson_UnitTest
    {
        [TestMethod]
        public void JsonFile_Managedx86_Test()
        {
            string filePath = Path.GetFullPath(@"..\..\..\TestFiles\Managed_x86.exe");

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // If this is a PE file, output the PE specific information.
                VerifyJson json = new VerifyJson(reader);

                Assert.IsFalse(json.IsJson);
                Assert.IsNull(json._seal);
            }
        }

        [TestMethod]
        public void JsonFile_Test()
        {
            string filePath = Path.GetFullPath(@"..\..\..\TestFiles\ValidSeal.json");

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // If this is a PE file, output the PE specific information.
                VerifyJson json = new VerifyJson(reader);

                Assert.IsTrue(json.IsJson);
                Assert.IsNotNull(json._seal);
            }
        }
    }
}
