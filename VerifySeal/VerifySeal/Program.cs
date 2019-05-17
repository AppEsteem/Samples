/****************************************************************************************
// <copyright file="Program.cs" company="AppEsteem Corporation">
// Copyright © 2018 All Rights Reserved
// </copyright>
****************************************************************************************/

using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

[assembly:InternalsVisibleTo("VerifySealUnitTests")]

namespace VerifySeal
{
    /// <summary>
    /// Main program entry point.
    /// </summary>
    public sealed class Program
    {
        static bool _verboseFlag = false;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            // Print out the program name and copyright information.
            PrintMessage("\nVerifySeal - Copyright (C) 2018-2019 AppEsteem Corporation. All Rights Reserved.", null, ConsoleColor.DarkYellow);

            // Parse the arguments, exiting with error if the arguments are not valid.
            if (false == ParseArguments(args, out string filepath))
            {
                PrintHelp();
                Environment.Exit(1);
            }

            var _sealIsValid = false;
            string _sealJson = null;
            string _fileVersion = null;
            string _fileName = null;
            string _companyName = null;
            string _thumbprint = null;
            DateTimeOffset _fileCreation = default(DateTimeOffset);

            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // If this is a PE file, output the PE specific information.
                VerifyPE pe = new VerifyPE(reader);
                if (pe.IsPe)
                {
                    // Save the seal JSON and file creation time.
                    _sealJson = pe.SealJson;
                    _fileCreation = pe.FileCreation;
                    _fileName = pe.FileName;
                    _fileVersion = pe.FileVersion;
                    _companyName = pe.CompanyName;

                    // get the certificate thumbprint from the file
                    try
                    {
                        X509Certificate fcert = X509Certificate.CreateFromSignedFile(filepath);
                        _thumbprint = fcert.GetCertHashString();
                    }
                    catch(Exception ex)
                    {
                        PrintMessage("  Cannot retrieve certificate, but continuing.");
                        PrintMessage("      exception: " + ex.ToString());
                        _thumbprint = "";
                    }
                    
                    Console.WriteLine("File '{0}' was created {1:f}.", _fileName, pe.FileCreation);
                    Console.WriteLine(pe.GetFileTypeMessage());

                    // File name and version are required for PE files.
                    if (string.IsNullOrWhiteSpace(_fileName))
                    {
                        PrintMessage("  File does not contain a FileName in the VersionInfo resource.");
                        Environment.Exit(1);
                    }

                    if (string.IsNullOrWhiteSpace(_fileVersion))
                    {
                        PrintMessage("  File does not contain a FileVersion in the VersionInfo resource.");
                        Environment.Exit(1);
                    }
                }

                // TODO: When Mac is supported, check if this is a Mac executable.

                // Check if this is a JSON file.
                VerifyJson json = new VerifyJson(reader);
                if (json.IsJson)
                {
                    _sealJson = json.SealJson;

                    Console.WriteLine("File '{0}' is a JSON file.", Path.GetFileName(filepath));
                }
            }

            // Check the seal JSON. If it is empty, the seal isn't present.
            if (string.IsNullOrWhiteSpace(_sealJson))
            {
                PrintMessage("Seal is not present.", null, ConsoleColor.Red);
            }
            else
            {
                // Validate the seal.
                _sealIsValid = ValidateSeal(_sealJson, _fileCreation, _fileName, _fileVersion, _companyName, _thumbprint);
                if (_sealIsValid)
                    PrintMessage("\nSeal is valid for this file.", null, ConsoleColor.Green);
                else
                    PrintMessage("\nSeal is not valid for this file.", null, ConsoleColor.Red);

                // If the verbose flag is specified, output the detailed seal information.
                if (_verboseFlag)
                    PrintSealInformation(_sealJson);
            }

#if DEBUG
            Console.WriteLine("\nPress a key to continue.");
            Console.ReadKey();
#endif
            if (_sealIsValid)
                Environment.ExitCode = 0;
            else
                Environment.ExitCode = 1;
        }

        /// <summary>
        /// Validates the seal.
        /// </summary>
        /// <param name="json">The JSON seal to validate.</param>
        /// <param name="creation">Time of the file's creation.</param>
        /// <param name="fileName">Name of the file from the version info resource.</param>
        /// <param name="fileVersion">Version of the file from the version info resource.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool ValidateSeal(string json, DateTimeOffset creation, string fileName = null, string fileVersion = null, string companyName = null, string thumbPrint = null)
        {
            // Validate arguments.
            if (string.IsNullOrWhiteSpace(json))
            {
                PrintMessage("  Seal is not present ...", "Error", null, ConsoleColor.Red);
                return false;
            }

            // Assume the seal is valid.
            bool isValid = true;

            try
            {
                // Parse the seal JSON.
                JToken root = JToken.Parse(json);

                // -----------------------------------------------------------------------------
                // Get the header and pull out the significant property values.
                // -----------------------------------------------------------------------------
                var signatureToken = root.SelectToken("header.signature");
                var signature = signatureToken?.ToString();
                if (string.IsNullOrWhiteSpace(signature))
                {
                    PrintMessage("  Seal header does not contain a valid signature ... ", "Error", null, ConsoleColor.Red);
                    isValid &= false;
                }
                else
                    PrintMessage("  Seal header contains a signature ... ", "Ok", null, ConsoleColor.Green);

                // -----------------------------------------------------------------------------
                // Get the certificate string.
                // -----------------------------------------------------------------------------
                var x509Token = root.SelectToken("header.x509Cert");
                var x509 = x509Token?.ToString();
                if (string.IsNullOrWhiteSpace(x509))
                {
                    PrintMessage("  Seal header does not contain a valid certificate ... ", "Error", null, ConsoleColor.Red);
                    isValid &= false;
                }
                else
                    PrintMessage("  Seal header contains a certificate ... ", "Ok", null, ConsoleColor.Green);

                // -----------------------------------------------------------------------------
                // Get the seal string.
                // -----------------------------------------------------------------------------
                var sealToken = root.SelectToken("seal");
                var seal = sealToken?.ToString(Newtonsoft.Json.Formatting.None);
                if (string.IsNullOrWhiteSpace(seal))
                {
                    PrintMessage("  Seal is empty ... ", "Error", null, ConsoleColor.Red);
                    isValid &= false;
                }
                else
                    PrintMessage("  Seal header contains a seal to validate ... ", "Ok", null, ConsoleColor.Green);

                // If the seal isn't valid to this point, we can no longer proceed.
                if (false == isValid)
                    return isValid;

                // Convert into bytes.
                var sigBytes = Convert.FromBase64String(signature);
                var sealBytes = Encoding.UTF8.GetBytes(seal);

                // -----------------------------------------------------------------------------
                // Create the certificate from the certificate string.
                // -----------------------------------------------------------------------------
                X509ChainStatusFlags flag = X509ChainStatusFlags.NoError;
                X509Certificate2 cert = new X509Certificate2(Convert.FromBase64String(x509));
                // This code doesn't seem to work on a Mac, catch the exception and just issue a warning.
                try
                {
                    if (false == cert.Verify())
                    {

                        // Attempt to see if the certificate has expired, if so issue a warning, but not an error.
                        using (X509Chain chain = new X509Chain())
                        {
                            chain.Build(cert);
                            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;

                            foreach (var element in chain.ChainElements)
                            {
                                bool certIsValid = element.Certificate.Verify();
                                if (chain.ChainStatus.Length > 0)
                                {
                                    for (int index = 0; index < element.ChainElementStatus.Length; index++)
                                    {
                                        flag |= element.ChainElementStatus[index].Status;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception) { flag = X509ChainStatusFlags.RevocationStatusUnknown; }

                if ((X509ChainStatusFlags.RevocationStatusUnknown & flag) > 0)
                {
                    PrintMessage("  Unable to verify certificate revokation status ... ", "Warning", null, ConsoleColor.Yellow);
                }
                else if ((X509ChainStatusFlags.NotTimeValid & flag) > 0)
                {
                    PrintMessage("  Certficate chain has an expired certificate ... ", "Warning", null, ConsoleColor.Yellow);
                }
                else if ((~X509ChainStatusFlags.NotTimeValid & flag) > 0)
                {
                    PrintMessage("  Certificate chain is not valid ... ", "Error", null, ConsoleColor.Red);
                    isValid &= false;
                }
                else
                    PrintMessage("  Certificate has a valid chain ... ", "Ok", null, ConsoleColor.Green);

                // -----------------------------------------------------------------------------
                // Check that the certificate was issued to AppEsteem.
                // -----------------------------------------------------------------------------
                if (false == cert.Subject.Contains("AppEsteem Corporation"))
                {
                    PrintMessage("  Certificate was not issued to AppEsteem Corporation ... ", "Error", null, ConsoleColor.Red);
                    isValid &= false;
                }
                else
                    PrintMessage("  Certificate was issued to AppEsteem Corporation ... ", "Ok", null, ConsoleColor.Green);

                // -----------------------------------------------------------------------------
                // Date checking coode.
                // -----------------------------------------------------------------------------
                string sAfter = root.SelectToken("seal.validDates.validForFilesSignedAfter").ToString();
                string sBefore = root.SelectToken("seal.validDates.validForFilesSignedBefore").ToString();
                if ((string.IsNullOrWhiteSpace(sAfter)) || (string.IsNullOrWhiteSpace(sBefore)))
                {
                    PrintMessage("  Seal does not contain a complete validDates property ... ", "Error", null, ConsoleColor.Red);
                    isValid &= false;
                }

                bool bAfter = DateTimeOffset.TryParse(sAfter, out DateTimeOffset after);
                bool bBefore = DateTimeOffset.TryParse(sBefore, out DateTimeOffset before);
                if ((false == bAfter) || (false == bBefore))
                {
                    PrintMessage("  Seal does not contain valid dates for the validDates property ... ", "Error", null, ConsoleColor.Red);
                    isValid &= false;
                }

                // Check that the certificate was valid at the beginning of the seal interval. It is OK for the certificate to expire before the seal interval ends.
                bAfter = DateTimeOffset.TryParse(cert.GetEffectiveDateString(), out DateTimeOffset certEffective);
                bBefore = DateTimeOffset.TryParse(cert.GetExpirationDateString(), out DateTimeOffset certExpiration);
                if ((false == bAfter ) || (false == bBefore))
                {
                    PrintMessage("  Certificate effective date or expiration date could not be parsed ... ", "Error", null, ConsoleColor.Red);
                    isValid &= false;
                }

                // Check that the validForFilesSignedAfter falls during the effective interval of the certificate.
                if (((after > certEffective) && (after <= certExpiration)) || ((before > certEffective) && (before <= certExpiration)))
                {
                    PrintMessage("  Seal valid interval overlaps with the effective dates of the certificate ... ", "Ok", null, ConsoleColor.Green);
                }
                else
                {
                    isValid &= false;
                    PrintMessage(" Seal valid interval does not overlap with the certificate effective dates ... ", "Error", null, ConsoleColor.Red);
                }

                // Check for the file date being between the valid dates if a file creation date was specified.
                if (creation != default(DateTimeOffset))
                {
                    if ((creation >= after) && (creation <= before))
                    {
                        PrintMessage("  File was created within the valid dates of the seal ... ", "Ok", null, ConsoleColor.Green);
                    }
                    else
                    {
                        PrintMessage("  File was created outside of the dates this seal covers ... ", "Error", null, ConsoleColor.Red);
                        isValid &= false;
                    }
                }

                // -----------------------------------------------------------------------------
                // Validate that this file is listed in the list of known files if a file name and version and company name were passed.
                // -----------------------------------------------------------------------------
                JArray contents = root.SelectToken("seal.contents") as JArray;
                if ((null != contents) 
                    && (false == string.IsNullOrWhiteSpace(fileName)) 
                    && (false == string.IsNullOrWhiteSpace(fileVersion))
                    && (companyName != null) // company name could be empty
                    )
                {
                    bool found = false;
                    foreach (var item in contents)
                    {
                        var name = item.SelectToken("name");
                        var version = item.SelectToken("version");
                        var vendor = item.SelectToken("vendor");
                        var thumbprint = item.SelectToken("thumbprint");

                        // Compare to the name and version of this file
                        if ((fileName == name?.ToString()) 
                            && (fileVersion == version?.ToString()) 
                            && ((string.IsNullOrEmpty(companyName) && string.IsNullOrEmpty(vendor?.ToString())) ||(companyName == vendor?.ToString()))
                            && (string.IsNullOrEmpty(thumbPrint) ||(thumbPrint == thumbprint?.ToString()))
                        )
                        {
                            found = true;
                            break;
                        }
                    }

                    if (false == found)
                    {
                        isValid &= false;
                        PrintMessage($"  File '{fileName}' version '{fileVersion}' CompanyName '{companyName}' does not match a file in the seal ... ", "Error", null, ConsoleColor.Red);
                    }
                    else
                        PrintMessage($"  File '{fileName}' version '{fileVersion}' CompanyName '{companyName}' matches a file in the seal ... ", "Ok", null, ConsoleColor.Green);
                }
                else
                {
                    PrintMessage("  No files are specified as part of the seal ... ", "Warning", null, ConsoleColor.Yellow);
                }

                // -----------------------------------------------------------------------------
                // Validate the seal contents by hashing the digest of the seal and comparing 
                // to the signature from the header using the public key.
                // -----------------------------------------------------------------------------
                using (SHA256Managed sha256 = new SHA256Managed())
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    byte[] digest = sha256.ComputeHash(sealBytes);
                    rsa.ImportParameters(cert.GetRSAPublicKey().ExportParameters(false));
                    bool match = rsa.VerifyHash(digest, HashAlgorithmName.SHA256.Name, sigBytes);

                    if (false == match)
                    {
                        isValid &= false;
                        PrintMessage("  Signature does not match seal content ... ", "Error", null, ConsoleColor.Red);
                    }
                    else
                        PrintMessage("  Signature matches seal content ... ", "Ok", null, ConsoleColor.Green);
                }
            }
            catch(Exception ex)
            {
                PrintMessage($"Unhandled exception: {ex.Message} {ex.StackTrace}", null, ConsoleColor.Red);
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Prints the seal information.
        /// </summary>
        /// <param name="json">The json.</param>
        internal static void PrintSealInformation(string json)
        {
            // Validate arguments.
            if (string.IsNullOrWhiteSpace(json))
                return;

            try
            {
                // Parse the seal JSON.
                JToken root = JToken.Parse(json);

                Console.WriteLine("\nSeal Information");

                Console.WriteLine("\n  Seal is valid for files created between {0} and {1}", root.SelectToken("seal.validDates.validForFilesSignedAfter"), root.SelectToken("seal.validDates.validForFilesSignedBefore"));

                Console.WriteLine("\n  Application Information");
                PrintMessage($"    Product Name: {root.SelectToken("seal.applicationIdentification.productName")}");
                PrintMessage($"    Product Version: {root.SelectToken("seal.applicationIdentification.version")}");
                PrintMessage($"    First Certified Version: {root.SelectToken("seal.applicationIdentification.firstCertifiedVersion")}");
                PrintMessage($"    Application Type: {root.SelectToken("seal.applicationIdentification.applicationType")}");
                PrintMessage($"    Certified: {root.SelectToken("seal.attestations.certified")}");

                Console.WriteLine("\n  Attestations");
                PrintMessage($"    Vendor Name: {root.SelectToken("seal.attestations.vendor")}");
                PrintMessage($"    Value Proposition: {root.SelectToken("seal.attestations.valueProposition")}");
                PrintMessage($"    Audience: {PrintJArray(root.SelectToken("seal.attestations.audience"))}");
                PrintMessage($"    Age: {root.SelectToken("seal.attestations.age")}");
                PrintMessage($"    Categories: {PrintJArray(root.SelectToken("seal.attestations.category"))}");
                PrintMessage($"    Monetization: {PrintJArray(root.SelectToken("seal.attestations.monetization"))}");
                PrintMessage($"    Target: {PrintJArray(root.SelectToken("seal.attestations.target"))}");

                Console.WriteLine("\n  Distribution");
                PrintMessage($"    Whitelist Landing Pages: {PrintJArray(root.SelectToken("seal.distribution.whitelist.landingPages"))}");
                PrintMessage($"    Whitelist Download Urls: {PrintJArray(root.SelectToken("seal.distribution.whitelist.downloadUrls"))}");
                PrintMessage($"    Whitelist Installers: {PrintJArray(root.SelectToken("seal.distribution.whitelist.installers"))}");
                PrintMessage($"    Whitelist Affiliates: {PrintJArray(root.SelectToken("seal.distribution.whitelist.affiliates"))}");
                PrintMessage($"    Whitelist Ad Networks: {PrintJArray(root.SelectToken("seal.distribution.whitelist.adNetworks"))}");

                PrintMessage($"    Blacklist Landing Pages: {PrintJArray(root.SelectToken("seal.distribution.blacklist.landingPages"))}");
                PrintMessage($"    Blacklist Download Urls: {PrintJArray(root.SelectToken("seal.distribution.blacklist.downloadUrls"))}");
                PrintMessage($"    Blacklist Installers: {PrintJArray(root.SelectToken("seal.distribution.blacklist.installers"))}");
                PrintMessage($"    Blacklist Affiliates: {PrintJArray(root.SelectToken("seal.distribution.blacklist.affiliates"))}");
                PrintMessage($"    Blacklist Ad Networks: {PrintJArray(root.SelectToken("seal.distribution.blacklist.adNetworks"))}");

                Console.WriteLine("\n  Contents");
                JArray contents = root.SelectToken("seal.contents") as JArray;
                if (null == contents)
                    Console.WriteLine("    None");
                else
                {
                    Console.WriteLine("    {0,-30}{1,-15}{2,-25}{3}", "Name", "Version", "Vendor", "Thumbprint");
                    Console.WriteLine("    --------------------------------------------------------------------------------------------------------------------");
                    foreach(var item in contents)
                    {
                        Console.WriteLine("    {0,-30}{1,-15}{2,-25}{3}", item.SelectToken("name"), item.SelectToken("version"), item.SelectToken("vendor"), item.SelectToken("thumbprint"));
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Prints an array of JSON items.
        /// </summary>
        /// <param name="array">The array containing the JSON items.</param>
        /// <returns>System.String.</returns>
        private static string PrintJArray(JToken token)
        {
            // If no token was selected, return a default string.
            if (null == token)
                return "None";

            if (JTokenType.Array == token.Type)
            {
                JArray array = (JArray)token;

                StringBuilder sb = new StringBuilder();
                foreach (var item in array)
                {
                    sb.AppendFormat("\n      {0}", item.ToString());
                }

                if (0 == sb.Length)
                    return "None";

                return sb.ToString();
            }

            return token.ToString();
        }

        /// <summary>
        /// Prints the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="suffix">The message suffix.</param>
        /// <param name="mainColor">Color of the main message.</param>
        /// <param name="suffixColor">Color of the suffix message.</param>
        internal static void PrintMessage(string message, string suffix = null, ConsoleColor? mainColor = null, ConsoleColor? suffixColor = null)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            // If a suffix was not provided, just output the main message.
            if (string.IsNullOrWhiteSpace(suffix))
            {
                if (mainColor.HasValue)
                {
                    Console.ForegroundColor = mainColor.Value;
                    Console.WriteLine(message);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
            else
            {
                if (mainColor.HasValue)
                {
                    Console.ForegroundColor = mainColor.Value;
                    Console.Write(message);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(message);
                }

                if (suffixColor.HasValue)
                {
                    Console.ForegroundColor = suffixColor.Value;
                    Console.WriteLine(suffix);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(suffix);
                }
            }
        }

        /// <summary>
        /// Parses the passed arguments.
        /// </summary>
        /// <param name="args">String array containing the passed arguments.</param>
        /// <param name="filepath">Out parameter where the file path will be saved.</param>
        /// <returns></returns>
        private static bool ParseArguments(string[] args, out string filepath)
        {
            filepath = null;

            // If no arguments are provided, return false.
            if (args.Length < 1) return false;

            foreach (string arg in args)
            {
                switch(arg)
                {
                    case "/h":
                    case "/H":
                    case "-h":
                    case "-H":
                        return false;

                    case "/v":
                    case "/V":
                    case "-v":
                    case "-V":
                        _verboseFlag = true;
                        break;

                    default:
                        filepath = Path.GetFullPath(arg);
                        break;
                }
            }

            // Return true if the filepath contains a value.
            return (false == string.IsNullOrWhiteSpace(filepath));
        }

        /// <summary>
        /// Prints program usage help.
        /// </summary>
        private static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Verifies the AppEsteem seal contained within an executable.\n");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("  Usage: VerifySeal <filepath> [/h] [/v]\n");
            Console.WriteLine("  Arguments:");
            Console.WriteLine("    <filepath>     Path and name of the executable file to evaluate.");
            Console.WriteLine("    /H             Displays this help message.");
            Console.WriteLine("    /V             Displays seal details0.");

            Console.ResetColor();
        }
    }
}
