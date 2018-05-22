/****************************************************************************************
// <copyright file="VerifyJson.cs" company="AppEsteem Corporation">
// Copyright © 2018 All Rights Reserved
// </copyright>
****************************************************************************************/

using System;
using System.IO;
using System.Text;

namespace VerifySeal
{
    /// <summary>
    /// Used to verify a plan JSON file.
    /// </summary>
    public sealed class VerifyJson
    {
        internal string _seal = null;

        public VerifyJson(BinaryReader reader)
        {
            // Seek the beginning of the file.
            reader.BaseStream.Seek(0, SeekOrigin.Begin);

            // The first byte of the file should be the '{' character.
            if ('{' == reader.ReadChar())
            {
                // Seek the last byte of the file
                reader.BaseStream.Seek(-1, SeekOrigin.End);
                if ('}' == reader.ReadChar())
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

                    // Get the JSON.
                    _seal = Encoding.UTF8.GetString(bytes);
                }
            }
        }

        public bool IsJson => (false == string.IsNullOrWhiteSpace(_seal));

        public string SealJson => _seal;
    }
}
