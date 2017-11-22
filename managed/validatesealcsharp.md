# Validating an AppEsteem seal

**IMPORTANT NOTE** - _Please read_
***
It is _very important_ to note that seals can be used in two ways. Having a seal within application doesn't mean that AppEsteem has certified that application. Vendors are able to create a seal without the application being certified by AppEsteem for testing purposes and evaluation. It is critically important when validating a seal to check the **certification** property within the *attestation* section. AppEsteem certified applications will have the value 'yes'. If the property is 'no' or the property is not present, the application **must not** be be considered certified by AppEsteem.

***
## What is a seal

A seal is a block of JSON containing information that AppEsteem is certifying about the application containing the seal. An example of a complete seal is shown below. Take a look at the complete seal, then we'll continue with details of each section.

```json
{
  "copyright": "Copyright © AppEsteem Corporation. All Rights Reserved.",
  "description": "Digitally signed seal for Self Regulating Client Library.",
  "comments": "Generated file, do not edit! If the content of the 'signedSeal' property is changed in any way, including adding spaces or newline characters, the digital signature will be broken and the seal will be reported as being forged.",
  "signedSeal": "{\"header\":{\"signature\":\"cNKzRy3XiT8Q/BQCugNVuwslmllSZ624miM9q/sKeFnlAkFhEX0Te4vFlgQFqEPj2aCFCmYGpVOvAQXP4pcIZ8/HHci9hgeORXQ1k2L1pd6TobpwRcHj1YiiiKV0JJTbvATFH2C5nD3pL7/R5mN5v7UG+a41wrA/W8M9W4DfgltzKVPauoDCPQq3TxwSNaSJ/LxnsmvFCNl+RjS0uiYKYzgl1ziB1QAIJDCZmhaMKxpUBcWSA7SAEDAVLII78FO/Vtm/eBDZHbp5mOg+nHrblJCfBhncVApB+Otp20QhaIwlG+AY22Xefgcnd3Kj0PM9739OQZ9+wJOAB+TXhedh4w==\",\"x509Cert\":\"MIIFMjCCBBqgAwIBAgIIOqyFx1PbMFQwDQYJKoZIhvcNAQELBQAwgbQxCzAJBgNVBAYTAlVTMRAwDgYDVQQIEwdBcml6b25hMRMwEQYDVQQHEwpTY290dHNkYWxlMRowGAYDVQQKExFHb0RhZGR5LmNvbSwgSW5jLjEtMCsGA1UECxMkaHR0cDovL2NlcnRzLmdvZGFkZHkuY29tL3JlcG9zaXRvcnkvMTMwMQYDVQQDEypHbyBEYWRkeSBTZWN1cmUgQ2VydGlmaWNhdGUgQXV0aG9yaXR5IC0gRzIwHhcNMTYwNzIwMjIwNDM4WhcNMTcwNzIwMjIwNDM4WjA9MSEwHwYDVQQLExhEb21haW4gQ29udHJvbCBWYWxpZGF0ZWQxGDAWBgNVBAMMDyouYXBwZXN0ZWVtLmNvbTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALseD/te6Z4YbOj0Iz7LzNi187AydQkykiwMWgPSpgkmCubkfY0rlytkG0gjdLYiOBhrlKCHB8jBl+fgebRaGZejwaIacjJxaK+xqBj1zfiwImyWwXtJed6kvWmKRIQTDHXuiZkodzwvqqHlicFwYGvrCsZDl8kv2QJD7HjwS3qNphk4oerZ7OWog2qdNTs9MAa8aXU/DOIzdQTcC//nrKk6VNxvmQCC2aLMnCekUplyrOkv2EjDsP1mRkWkcByZdafQ3FSxyjEH+irQhrrQiZMb0AMBfpyDaZ4BxRExNrMyQygpiBBEFG9+XPt0rugB4Q7GkkeaBBGB3lFI5Hnj+N0CAwEAAaOCAbwwggG4MAwGA1UdEwEB/wQCMAAwHQYDVR0lBBYwFAYIKwYBBQUHAwEGCCsGAQUFBwMCMA4GA1UdDwEB/wQEAwIFoDA3BgNVHR8EMDAuMCygKqAohiZodHRwOi8vY3JsLmdvZGFkZHkuY29tL2dkaWcyczEtMjcwLmNybDBdBgNVHSAEVjBUMEgGC2CGSAGG/W0BBxcBMDkwNwYIKwYBBQUHAgEWK2h0dHA6Ly9jZXJ0aWZpY2F0ZXMuZ29kYWRkeS5jb20vcmVwb3NpdG9yeS8wCAYGZ4EMAQIBMHYGCCsGAQUFBwEBBGowaDAkBggrBgEFBQcwAYYYaHR0cDovL29jc3AuZ29kYWRkeS5jb20vMEAGCCsGAQUFBzAChjRodHRwOi8vY2VydGlmaWNhdGVzLmdvZGFkZHkuY29tL3JlcG9zaXRvcnkvZ2RpZzIuY3J0MB8GA1UdIwQYMBaAFEDCvSeOzDSDMKIz1/tss/C0LIDOMCkGA1UdEQQiMCCCDyouYXBwZXN0ZWVtLmNvbYINYXBwZXN0ZWVtLmNvbTAdBgNVHQ4EFgQU57qMDbG+clb8NyNO3TkzgJtX6Y0wDQYJKoZIhvcNAQELBQADggEBAAQ2bxnE6DzrAl+MawZKcAKmh5jMe8z7gErlrdXf08HC49dogNug+IWPTEajOWAvWHX75qBzG/mB6kfJW+OVdzFykV9A2b6xFS1e5ekUf3N3BqCcJUebzwWHbJqKiK6GJdSZaM64gg3B+PwxcqEQHpqzf5IkQ+xItv/XEk9B3/+CjY402JEeZwtgH/KUsUV4kF/g1Jus7Q/4NU2pNJCBht6LCbjHrDW0H4BBZvLe5fkHBfHlo3QL72sp0icmn3meXkBX7vwtw7DClFZQylXvLAtPTFr1/FCa0cnPbyHijcoaYV5ALf1TXTXX84Nkal/i1vrgbYgSztm272zS75TyCGA=\"},\"seal\":{\"applicationIdentification\":{\"appId\":\"00000000-0000-0000-0000-000000000002\",\"sealId\":\"161104-PEF-DRVHQ-00001\",\"applicationType\":\"pe\"},\"attestations\":{\"address\":\"7600 Capital of Texas Highway, Building B, Suite 350, Austin, TX 78731\",\"certification\":\"yes\",\"valueProposition\":\"Driver recommendation and update services including performance optimizations.\",\"age\":\"Child appropriate\",\"audience\":[\"Consumer\"],\"category\":[\"SysTools & Utilities\"],\"monetization\":[\"Paid\",\"Up-sell\"],\"target\":[\"Windows XP\",\"Windows Vista\",\"Windows 7\",\"Windows 8\",\"Windows 10\"]},\"validDates\":{\"validForFilesSignedAfter\":\"2016-11-03T00:00:00+00:00\",\"validForFilesSignedBefore\":\"2017-11-03T00:00:00+00:00\"},\"distribution\":{\"whitelist\":{\"landingPages\":[\"http://download.driversupport.com/lp/*/*\",\"http://www.driversupport.com/\"],\"downloadUrls\":[\"http://cdn.driversupport.com/builds/v10/nsis/driversupport/DriverSupport.exe\",\"https://secure.driversupport.com/direct/driversupport/driversupport.exe\"]}},\"contents\":{\"files\":[{\"name\":\"DriverSupport.exe\",\"majorVersion\":\"10\",\"thumbprint\":\"1a7acd613247312b73a6f91156a4cc4c2a8b19c5\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"DriverSupportApp.exe\",\"majorVersion\":\"10\",\"thumbprint\":\"1a7acd613247312b73a6f91156a4cc4c2a8b19c5\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"DriverSupportUpdater.exe\",\"majorVersion\":\"10\",\"thumbprint\":\"1a7acd613247312b73a6f91156a4cc4c2a8b19c5\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"Agent.CPU.exe\",\"majorVersion\":\"10\",\"thumbprint\":\"1a7acd613247312b73a6f91156a4cc4c2a8b19c5\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"ISUninstall.exe\",\"majorVersion\":\"1\",\"thumbprint\":\"5ccc0781ad2dffac626a04f1839fe8807909fb77\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"Uninstall.exe\",\"majorVersion\":\"1\",\"thumbprint\":\"5ccc0781ad2dffac626a04f1839fe8807909fb77\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"DriverSupportAO.exe\",\"majorVersion\":\"1\",\"thumbprint\":\"5ccc0781ad2dffac626a04f1839fe8807909fb77\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"DriverSupportAOsvc.exe\",\"majorVersion\":\"1\",\"thumbprint\":\"5ccc0781ad2dffac626a04f1839fe8807909fb77\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"ipterbg.exe\",\"majorVersion\":\"1\",\"thumbprint\":\"5ccc0781ad2dffac626a04f1839fe8807909fb77\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"ipteup.exe\",\"majorVersion\":\"1\",\"thumbprint\":\"5ccc0781ad2dffac626a04f1839fe8807909fb77\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"pmtu.exe\",\"majorVersion\":\"1\",\"thumbprint\":\"5ccc0781ad2dffac626a04f1839fe8807909fb77\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"sigverify.exe\",\"majorVersion\":\"1\",\"thumbprint\":\"5ccc0781ad2dffac626a04f1839fe8807909fb77\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"uninstall.exe\",\"majorVersion\":\"1\",\"thumbprint\":\"5ccc0781ad2dffac626a04f1839fe8807909fb77\",\"vendor\":\"PC Drivers Headquarters, LP\"},{\"name\":\"viometer.exe\",\"majorVersion\":\"1\",\"thumbprint\":\"5ccc0781ad2dffac626a04f1839fe8807909fb77\",\"vendor\":\"PC Drivers Headquarters, LP\"}]}}
}
```
The important part to validate is the JSON contained within "signedSeal" as described below.

### Header section

The header section contains the certificate and the resulting signature that validates the contents of the **seal** section. The **x509Cert** property contains the X509 certificate that can be used to generate a signature of the seal's digest and if it matches the **signature** property, then the contents of the seal are valid. If the signatures don't match, then the seal is not valid. The certificate is a based64 encoded byte array containing the public portion of an AppEsteem signing certificate and the certificate must also be validated to be from AppEsteem and to have a valid certificate chain. The certificate was embedded into the seal to allow verification without making a request to AppEsteem to validate the seal or the certificate. Today, we use a single certificate, but we plan on using more than one signing certificate.

The signature section contains a base64 encoded byte array containing the result of an RS256 signing operation on the digest value of the **seal** property, including the '{' and '}' brackets within the UTF8 encoded string of JSON. AppEsteem uses Azure KeyVault for the [signing operation](https://docs.microsoft.com/rest/api/keyvault/sign).

### Seal section

The seal contains a number of additional properties, each will be covered in detail within this section.

#### validDates

The valid date section is important because it informs if this application and seal combination are still valid. Currently, AppEsteem seals are valid for one year. Within this time, a vendor can release many minor versions of the same application as long as it doesn't deviate from what is described within the seal. To have a valid seal, the creation date of the application must fall within the date contained within this section. For instance, if the **validForFilesSignedAfter** is 1 Jan 2017 and the **validForFilesSignedBefore** is 31 Dec 2017. The file creation date must be within the year of 2017. If it is in 2018, the seal should be considered invalid.

#### distribution

The distribution section contains a list of the white or black listed landing pages and download Urls this application can be distributed from. If it is being distributed from another location, this application should be considered invalid. *Note* in the example only the whitelist property is present, a *blacklist* property may also be present.

#### applicationIdentification

This section is used by AppEsteem to identify the application using the unique combination of an application identifier (appId) and a seal identifer (sealId). These identifier can also be seen on the customer portal as unique application identifiers. The applicationType indicates the type of application, for native application this will contain 'pe'.

#### attestations

This section contains information that AppEsteem has validated such as

* **address** - address of the application's vendor
* **age** - Age appropriate rating recommendation
* **audience** - Target audience for the application, 'consumer' and 'enterprise'
* **certification** - Indicates the certification status of the application. Only applications with a valid seal and a value of 'yes' for this property are certified.
* **category** - Category of software this application falls within
* **monetization** - How the application is monetized
* **target** - What operating systems are supported by the application
* **valueProposition** - Value propositions as states by the vendor

#### contents

This section contains a property named **files** that is an array of information about the files that are covered by this seal. If other files are identified as part of this application, but are not within this array, they are not part of what AppEsteem has certified. Each file contains

* **name** - Name of the file
* **majorVersion** - Major version of the file
* **thumbprint** - Thumbprint of one of the certificates used to sign the file
* **vendor** - Name of the vendor

## Validating the seal

A .Net C# application will contain a .Net resource named \<ProjectName\>.\<FolderName\>.AESeal.json that contains the JSON seal and nothing else. Both \<ProjectName\> and \<FolderName\> depend on the specific application. Any resource ending with ".AESeal.json" should be tested for a seal. After obtaining the JSON text you have to verify the following:
1. The certificate in the header has a trusted certificate chain.
2. The certificate in the header was issued to AppEsteem.
3. The signature in the header is that of the of the seal section:
---

1. Verify that the certificate in the header has a trusted certificate chain:

   a. Use the base64 encoded value of the **X509Cert** property to obtain the signing certificate (ASN encoded).
   Pseudocode example:

   ```C++
     CryptStringToBinaryA(szCertificate, nCertificateLength, CRYPT_STRING_BASE64, certificate_bytes, &certificate_size, NULL, NULL);
     PCCERT_CONTEXT cert_context = CertCreateCertificateContext(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, certificate_bytes, certificate_size);
   ```

   b. Validate the certificate is trusted and it is not self signed.
   Pseudocode example:

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

2. Verify the certificate was issued to AppEsteem.

   a. Validate that the certificate has the common name of "AppEsteem Corporation" and the serial number is one of those [listed](https://www.appesteem.com).
   Pseudocode example:

   ```C++
        DWORD str_type = CERT_X500_NAME_STR;
        DWORD size = ::CertGetNameStringW(cert_context, CERT_NAME_RDN_TYPE, CERT_NAME_DISABLE_IE4_UTF8_FLAG, &str_type, NULL, 0);
        LPWSTR subject = (LPWSTR)LocalAlloc(0, size * sizeof(WCHAR));
        size = ::CertGetNameStringW(cert_context, CERT_NAME_RDN_TYPE, CERT_NAME_DISABLE_IE4_UTF8_FLAG, &str_type, subject, size);
        // subject must contain "AppEsteem Corporation"

        std::string serial;
        serial.reserve(2 * cert_context->pCertInfo->SerialNumber.cbData);
        for (DWORD i = cert_context->pCertInfo->SerialNumber.cbData; i != 0; i--)
        {
            char buf[3];
            BYTE b = *(cert_context->pCertInfo->SerialNumber.pbData + i - 1);
            sprintf_s(buf, sizeof(buf), "%02x", b);
            serial += buf;
        }
        // serial must be listed at TODO
   ```

3. Verify the signature in the header is that of the of the seal section:

   a. Compute the SHA256 value of the contents of the **seal** property, including the starting and ending brackets.
   Pseudocode example:

   ```C++
        HCRYPTPROV hProv;
        CryptAcquireContext(&hProv, NULL, NULL, PROV_RSA_AES, CRYPT_VERIFYCONTEXT);
        HCRYPTHASH hHash;
        CryptCreateHash(hProv, CALG_SHA_256, 0, 0, &hHash);
        CryptHashData(hHash, seal_text, seal_text_length, 0);
   ```

   b. Use the public key in the certificate to validate that the signature in the header (RSASSA-PKCS-v1_5 format) is that of the calculated digest. If it is, the value of the seal is the same as what AppEsteem has certified. If they do not match, then the seal is not valid.
   Pseudocode example:

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
