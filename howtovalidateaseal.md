# How to validate a seal

This article explains how to validate a seal embedded within an application files. This is done by AppEsteem security partners to validate a file on a user's device is part of an AppEsteem certified application. The *VerifySeal* application contains uses all of these steps to verify that a file is part of a valid certified application. The code for *VerifySeal* is published in AppEsteem's public GitHib in the [Samples repository](https://www.github.com/appesteem/samples).

## Prerequisites

* Understand the contents of a seal. See the article [What is an Appesteem Seal](whatisaseal.md) to learn more
* Obtain the JSON seal from an application's file. For both managed and native applications, the seal is stored as a resource named AESeal.json embedded within the file. The **verifySeal** tool shows how to read a resource using portable managed code. This enables the tool to read both managed and native application, but there is quite a bit of code required to read a resource.

## Validating a Seal

A seal can be embedded in one or more files that make up an application. Each file must be validated independently based on the seal that is embedded within the file. Once the seal is obtained from the file, a number of steps must be taken to validate that the seal is valid and that the file that contained the seal is part of what AppEsteem has certified as part of the application.

* The seal contains a valid header section with a signature and a X509 certificate
* The certificate is a valid X509 certificate issued to AppEsteem Corporation
* The certificate was valid for the date the file was created and for the date interval specified within the seal
* The file name and file version are present in the seal and one of the signing certificates thumbprint matches the seal
* The contents of the seal match the digital signature provided in the header

The **verifySeal** tool is used by application vendors to verify their files correctly embed the seal. If this tool indicates that the seal is not correctly validated, the vendor should not distributed the file as it may be flagged as not certified by AppEsteem's security partners. The sample C# code below is taken from the **verifySeal** tool. C++ pseudo-code is provided where possible to help security partners that have native security products validate a seal.

### _**Verify the seal contains a valid header section**_

Within the header section of a seal, there **must** be a **signature** property and a **x509cert** property. The **signature** property contains a base64 encoded byte array containing the digital signature of the contents of the seal. There **must** also be a **x509cert** property as part of the header. The **x509cert** property contains a base64 encoded byte array containing a certificate used to created the digital signature.

To create a certificate from the bytes use the following C++ pseudo-code example:

```C++
     CryptStringToBinaryA(szCertificate, nCertificateLength, CRYPT_STRING_BASE64, certificate_bytes, &certificate_size, NULL, NULL);
     PCCERT_CONTEXT cert_context = CertCreateCertificateContext(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, certificate_bytes, certificate_size);
```

and if using managed code:

```csharp
     X509Certificate2 cert = new X509Certificate2(Convert.FromBase64String(x509));
```

### _**Validate the certificate**_

Next the certificate needs to be validated that is from a trusted root and that it was issued to AppEsteem Corporation. To validate the certificate is trusted and it is not self-signed, use the following pseudo-code example:

   ```C++
        CERT_CHAIN_PARA ChainPara = { 0 };
        ChainPara.cbSize = sizeof(ChainPara);
        PCCERT_CHAIN_CONTEXT pChainContext = NULL;
        CertGetCertificateChain(NULL, cert_context, NULL, NULL, &ChainPara, CERT_CHAIN_REVOCATION_CHECK_CHAIN_EXCLUDE_ROOT, NULL, &pChainContext);
        CERT_CHAIN_POLICY_PARA ChainPolicy = { 0 };
        ChainPolicy.cbSize = sizeof(ChainPolicy);
        ChainPolicy.dwFlags = CERT_CHAIN_POLICY_IGNORE_NOT_TIME_NESTED_FLAG;
        ChainPolicy.pvExtraPolicyPara = NULL; // base policy
        CERT_CHAIN_POLICY_STATUS PolicyStatus = { 0 };
        PolicyStatus.cbSize = sizeof(PolicyStatus);
        CertVerifyCertificateChainPolicy(CERT_CHAIN_POLICY_BASE, pChainContext, &ChainPolicy, &PolicyStatus);
        bool valid_chain = PolicyStatus.dwError == S_OK || PolicyStatus.dwError == CRYPT_E_NO_REVOCATION_CHECK || PolicyStatus.dwError == CRYPT_E_REVOCATION_OFFLINE;
   ```

   Verify the certificate was issued to AppEsteem Corporation

   ```C++
        DWORD str_type = CERT_X500_NAME_STR;
        DWORD size = ::CertGetNameStringW(cert_context, CERT_NAME_RDN_TYPE, CERT_NAME_DISABLE_IE4_UTF8_FLAG, &str_type, NULL, 0);
        LPWSTR subject = (LPWSTR)LocalAlloc(0, size * sizeof(WCHAR));
        size = ::CertGetNameStringW(cert_context, CERT_NAME_RDN_TYPE, CERT_NAME_DISABLE_IE4_UTF8_FLAG, &str_type, subject, size);

        // subject must contain "AppEsteem Corporation"
        FindInLPWSTR(subject, "AppEsteem Corporation");
   ```

### _**Validate the certificate, seal and file dates**_

There a few different ways that date values must be verified

* The seal properties **validForFilesSignedAfter** and **validForFilesSignedBefore** _**must**_ have a valid date specified in the format of YYYY-MM-DD
* The file creation date _**must**_ fall between the **validForFilesSignedAfter** and **validForFilesSignedBefore** properties contained within the seal. If the file creation date is outside of these dates, that means that the seal was either not yet valid or expired when the file was created
* The seal's valid interval _**must**_ overlap the effective period for the certificate. This allows for a new certificate to be used for a seal that was issued before the certificate effective date.

```csharp
    string sAfter = root.SelectToken("seal.validDates.validForFilesSignedAfter").ToString();
    string sBefore = root.SelectToken("seal.validDates.validForFilesSignedBefore").ToString();
    if ((string.IsNullOrWhiteSpace(sAfter)) || (string.IsNullOrWhiteSpace(sBefore)))
    {
        // Invalid - dates not specified.
    }

    bool bAfter = DateTimeOffset.TryParse(sAfter, out DateTimeOffset after);
    bool bBefore = DateTimeOffset.TryParse(sBefore, out DateTimeOffset before);
    if ((false == bAfter) || (false == bBefore))
    {
        // Invalid - Seal does not contain valid dates for the validDates property.
    }

    // Check that the certificate was valid at the beginning of the seal interval. It is OK for the certificate to expire before the seal interval ends.
    bAfter = DateTimeOffset.TryParse(cert.GetEffectiveDateString(), out DateTimeOffset certEffective);
    bBefore = DateTimeOffset.TryParse(cert.GetExpirationDateString(), out DateTimeOffset certExpiration);
    if ((false == bAfter ) || (false == bBefore))
    {
        // Invalid - Certificate effective date or expiration date could not be parsed.
    }

    // Check that the valid seal interval overlaps the effective interval of the certificate.
    if ((after < certEffective) || (after > certExpiration))
    {
        // Invalid - ValidForFilesSignedAfter does not fall between the effective dates of the certificate.
    }

    // Check for the file date being between the valid dates if a file creation date was specified.
    if (creation != default(DateTimeOffset))
    {
        if ((creation >= after) && (creation <= before))
        {
            // Valid - File was created within the valid dates of the seal.
        }
        else
        {
            // Invalid - File was created outside of the dates this seal covers.
        }
    }
```

### _**Validate the file name and version are present in the seal**_

The file containing the seal must be represented in the seal and match both the file name and the file version. If the file is not present, the seal must not be trusted to certifying the file. The file name used is either the original file name property or the file name property that is embedded as a version resource within the executable file. The file version from the version resource is used to obtain the version of the file. If these properties are missing, then the file cannot be part of a certified application.

```csharp
    JArray contents = root.SelectToken("seal.contents") as JArray;
    if ((null != contents) && (false == string.IsNullOrWhiteSpace(fileName)) && (false == string.IsNullOrWhiteSpace(fileVersion)))
    {
        bool found = false;
        foreach (var item in contents)
        {
            var name = item.SelectToken("name");
            var version = item.SelectToken("version");

            // Compare to the name and version of this file
            if ((fileName == name?.ToString()) && (fileVersion == version?.ToString()))
            {
                found = true;
                break;
            }
        }

        if (false == found)
        {
            isValid &= false;
            PrintMessage($"  File '{fileName}' version '{fileVersion}' does not match a file in the seal ... ", "Error", null, ConsoleColor.Red);
        }
        else
            PrintMessage($"  File '{fileName}' version '{fileVersion}' matches a file in the seal ... ", "Ok", null, ConsoleColor.Green);
    }
    else
    {
        PrintMessage("  No files are specified as part of the seal ... ", "Warning", null, ConsoleColor.Yellow);
    }    
```

### **Validate that the signatures matches the seal content**

First compute the SHA256 value of the content of the **seal** property, including the starting and ending brackets

   ```C++
        HCRYPTPROV hProv;
        CryptAcquireContext(&hProv, NULL, NULL, PROV_RSA_AES, CRYPT_VERIFYCONTEXT);
        HCRYPTHASH hHash;
        CryptCreateHash(hProv, CALG_SHA_256, 0, 0, &hHash);
        CryptHashData(hHash, seal_text, seal_text_length, 0);
   ```

Next use the public key in the certificate to validate that the signature in the header (RSASSA-PKCS-v1_5 format) is that of the calculated digest. If it is, the value of the seal is the same as what AppEsteem has certified.

   ```C++
        DWORD size = 0;
        CryptBinaryToStringA(cert_context->pCertInfo->SubjectPublicKeyInfo.PublicKey.pbData + 9, cert_context->pCertInfo->SubjectPublicKeyInfo.PublicKey.cbData - 9 - 2, CRYPT_STRING_BASE64 | CRYPT_STRING_NOCRLF, NULL, &size);
        CHAR key[str_size];
        CryptBinaryToStringA(cert_context->pCertInfo->SubjectPublicKeyInfo.PublicKey.pbData + 9, cert_context->pCertInfo->SubjectPublicKeyInfo.PublicKey.cbData - 9 - 2, CRYPT_STRING_BASE64 | CRYPT_STRING_NOCRLF, str, &size);
        key[size - 6] = '=';
        key[size - 5] = '=';
        key[size - 4] = '=';
        key[size - 3] = 0;

        unsigned char der_key[4096 / 8];
        DWORD der_key_size = sizeof(der_key);
        CryptStringToBinaryA(key, size-3, CRYPT_STRING_BASE64, der_key, &der_key_size, NULL, NULL);
        std::reverse(der_key, der_key + der_key_size);
        unsigned char der_signature[4096 / 8];
        DWORD der_signature_size = sizeof(der_signature);
        CryptStringToBinaryA(signature, signature_length, CRYPT_STRING_BASE64, der_signature, &der_signature_size, NULL, NULL);
        std::reverse(der_signature, der_signature + der_signature_size);

        RSAKEY rsa_pub_key;
        rsa_pub_key.header.bType = PUBLICKEYBLOB;
        rsa_pub_key.header.bVersion = CUR_BLOB_VERSION;
        rsa_pub_key.header.reserved = 0;
        rsa_pub_key.header.aiKeyAlg = CALG_RSA_KEYX;
        rsa_pub_key.pub_key.magic = 0x31415352;
        rsa_pub_key.pub_key.bitlen = der_key_size * 8;
        rsa_pub_key.pub_key.pubexp = 65537;
        memcpy(rsa_pub_key.n, der_key, der_key_size);
        HCRYPTPROV hProv;
        CryptAcquireContext(&hProv, NULL, NULL, PROV_RSA_AES, CRYPT_VERIFYCONTEXT);
        HCRYPTKEY hPubKey;
        CryptImportKey(hProv, (BYTE*)&rsa_pub_key, sizeof(BLOBHEADER) + sizeof(RSAPUBKEY) + der_key_size, 0, 0, &hPubKey);
        if (CryptVerifySignature(hHash, der_signature, der_signature_size, hPubKey, NULL, 0))
        {
            // signature valid
        }
   ```

and if using managed code

```csharp
    using (SHA256Managed sha256 = new SHA256Managed())
    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
    {
        byte[] digest = sha256.ComputeHash(sealBytes);
        rsa.ImportParameters(cert.GetRSAPublicKey().ExportParameters(false));
        bool match = rsa.VerifyHash(digest, HashAlgorithmName.SHA256.Name, sigBytes);
    }
```

## See Also

* [What is a seal](whatisaseal.md) - Explains the seal and it's properties
* [Register a company](registercompany.md) - Explains how to register your company with AppEsteem
* [Register an application](registerapplication.md) - Explains how to register your application with AppEsteem