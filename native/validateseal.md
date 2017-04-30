# Validating an AppEsteem seal

## What is a seal

A seal is a block of JSON containing information that AppEsteem is certifying about the application containing the seal. An example of a complete seal is shown below. Take a look at the complete seal, then we'll continue with details of each section.

** IMPORTANT NOTE **

It is very important to note that seals can be used in two ways, having a seal present doesn't mean that AppEsteem has certified that application. Vendors are able to create a seal without it being certified by AppEsteem for testing purposes. An important part of validating a seal is to check the **certification** property within the *attestation* section. AppEsteem certified applications will have the value 'yes'. If the property is 'no' or not present, the application should be be considered certified by AppEsteem.

```json
{
  "header": {
    "signature": "cNKzRy3XiT8Q/BQCugNVuwslmllSZ624miM9q/sKeFnlAkFhEX0Te4vFlgQFqEPj2aCFCmYGpVOvAQXP4pcIZ8/HHci9hgeORXQ1k2L1pd6TobpwRcHj1YiiiKV0JJTbvATFH2C5nD3pL7/R5mN5v7UG+a41wrA/W8M9W4DfgltzKVPauoDCPQq3TxwSNaSJ/LxnsmvFCNl+RjS0uiYKYzgl1ziB1QAIJDCZmhaMKxpUBcWSA7SAEDAVLII78FO/Vtm/eBDZHbp5mOg+nHrblJCfBhncVApB+Otp20QhaIwlG+AY22Xefgcnd3Kj0PM9739OQZ9+wJOAB+TXhedh4w==",
    "x509Cert": "MIIFMjCCBBqgAwIBAgIIOqyFx1PbMFQwDQYJKoZIhvcNAQELBQAwgbQxCzAJBgNVBAYTAlVTMRAwDgYDVQQIEwdBcml6b25hMRMwEQYDVQQHEwpTY290dHNkYWxlMRowGAYDVQQKExFHb0RhZGR5LmNvbSwgSW5jLjEtMCsGA1UECxMkaHR0cDovL2NlcnRzLmdvZGFkZHkuY29tL3JlcG9zaXRvcnkvMTMwMQYDVQQDEypHbyBEYWRkeSBTZWN1cmUgQ2VydGlmaWNhdGUgQXV0aG9yaXR5IC0gRzIwHhcNMTYwNzIwMjIwNDM4WhcNMTcwNzIwMjIwNDM4WjA9MSEwHwYDVQQLExhEb21haW4gQ29udHJvbCBWYWxpZGF0ZWQxGDAWBgNVBAMMDyouYXBwZXN0ZWVtLmNvbTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBALseD/te6Z4YbOj0Iz7LzNi187AydQkykiwMWgPSpgkmCubkfY0rlytkG0gjdLYiOBhrlKCHB8jBl+fgebRaGZejwaIacjJxaK+xqBj1zfiwImyWwXtJed6kvWmKRIQTDHXuiZkodzwvqqHlicFwYGvrCsZDl8kv2QJD7HjwS3qNphk4oerZ7OWog2qdNTs9MAa8aXU/DOIzdQTcC//nrKk6VNxvmQCC2aLMnCekUplyrOkv2EjDsP1mRkWkcByZdafQ3FSxyjEH+irQhrrQiZMb0AMBfpyDaZ4BxRExNrMyQygpiBBEFG9+XPt0rugB4Q7GkkeaBBGB3lFI5Hnj+N0CAwEAAaOCAbwwggG4MAwGA1UdEwEB/wQCMAAwHQYDVR0lBBYwFAYIKwYBBQUHAwEGCCsGAQUFBwMCMA4GA1UdDwEB/wQEAwIFoDA3BgNVHR8EMDAuMCygKqAohiZodHRwOi8vY3JsLmdvZGFkZHkuY29tL2dkaWcyczEtMjcwLmNybDBdBgNVHSAEVjBUMEgGC2CGSAGG/W0BBxcBMDkwNwYIKwYBBQUHAgEWK2h0dHA6Ly9jZXJ0aWZpY2F0ZXMuZ29kYWRkeS5jb20vcmVwb3NpdG9yeS8wCAYGZ4EMAQIBMHYGCCsGAQUFBwEBBGowaDAkBggrBgEFBQcwAYYYaHR0cDovL29jc3AuZ29kYWRkeS5jb20vMEAGCCsGAQUFBzAChjRodHRwOi8vY2VydGlmaWNhdGVzLmdvZGFkZHkuY29tL3JlcG9zaXRvcnkvZ2RpZzIuY3J0MB8GA1UdIwQYMBaAFEDCvSeOzDSDMKIz1/tss/C0LIDOMCkGA1UdEQQiMCCCDyouYXBwZXN0ZWVtLmNvbYINYXBwZXN0ZWVtLmNvbTAdBgNVHQ4EFgQU57qMDbG+clb8NyNO3TkzgJtX6Y0wDQYJKoZIhvcNAQELBQADggEBAAQ2bxnE6DzrAl+MawZKcAKmh5jMe8z7gErlrdXf08HC49dogNug+IWPTEajOWAvWHX75qBzG/mB6kfJW+OVdzFykV9A2b6xFS1e5ekUf3N3BqCcJUebzwWHbJqKiK6GJdSZaM64gg3B+PwxcqEQHpqzf5IkQ+xItv/XEk9B3/+CjY402JEeZwtgH/KUsUV4kF/g1Jus7Q/4NU2pNJCBht6LCbjHrDW0H4BBZvLe5fkHBfHlo3QL72sp0icmn3meXkBX7vwtw7DClFZQylXvLAtPTFr1/FCa0cnPbyHijcoaYV5ALf1TXTXX84Nkal/i1vrgbYgSztm272zS75TyCGA="
  },
  "seal": {
    "applicationIdentification": {
      "appId": "00000000-0000-0000-0000-000000000002",
      "sealId": "161104-PEF-DRVHQ-00001",
      "applicationType": "pe"
    },
    "attestations": {
      "address": "7600 Capital of Texas Highway, Building B, Suite 350, Austin, TX 78731",
      "certification": "yes",
      "valueProposition": "Driver recommendation and update services including performance optimizations.",
      "age": "Child appropriate",
      "audience": [
        "Consumer"
      ],
      "category": [
        "SysTools & Utilities"
      ],
      "monetization": [
        "Paid",
        "Up-sell"
      ],
      "target": [
        "Windows XP",
        "Windows Vista",
        "Windows 7",
        "Windows 8",
        "Windows 10"
      ]
    },
    "validDates": {
      "validForFilesSignedAfter": "2016-11-03T00:00:00+00:00",
      "validForFilesSignedBefore": "2017-11-03T00:00:00+00:00"
    },
    "distribution": {
      "whitelist": {
        "landingPages": [
          "http://download.driversupport.com/lp/*/*",
          "http://www.driversupport.com/"
        ],
        "downloadUrls": [
          "http://cdn.driversupport.com/builds/v10/nsis/driversupport/DriverSupport.exe",
          "https://secure.driversupport.com/direct/driversupport/driversupport.exe"
        ]
      }
    },
    "contents": {
      "files": [
        {
          "name": "DriverSupport.exe",
          "majorVersion": "10",
          "thumbprint": "1a7acd613247312b73a6f91156a4cc4c2a8b19c5",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "DriverSupportApp.exe",
          "majorVersion": "10",
          "thumbprint": "1a7acd613247312b73a6f91156a4cc4c2a8b19c5",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "DriverSupportUpdater.exe",
          "majorVersion": "10",
          "thumbprint": "1a7acd613247312b73a6f91156a4cc4c2a8b19c5",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "Agent.CPU.exe",
          "majorVersion": "10",
          "thumbprint": "1a7acd613247312b73a6f91156a4cc4c2a8b19c5",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "ISUninstall.exe",
          "majorVersion": "1",
          "thumbprint": "5ccc0781ad2dffac626a04f1839fe8807909fb77",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "Uninstall.exe",
          "majorVersion": "1",
          "thumbprint": "5ccc0781ad2dffac626a04f1839fe8807909fb77",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "DriverSupportAO.exe",
          "majorVersion": "1",
          "thumbprint": "5ccc0781ad2dffac626a04f1839fe8807909fb77",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "DriverSupportAOsvc.exe",
          "majorVersion": "1",
          "thumbprint": "5ccc0781ad2dffac626a04f1839fe8807909fb77",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "ipterbg.exe",
          "majorVersion": "1",
          "thumbprint": "5ccc0781ad2dffac626a04f1839fe8807909fb77",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "ipteup.exe",
          "majorVersion": "1",
          "thumbprint": "5ccc0781ad2dffac626a04f1839fe8807909fb77",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "pmtu.exe",
          "majorVersion": "1",
          "thumbprint": "5ccc0781ad2dffac626a04f1839fe8807909fb77",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "sigverify.exe",
          "majorVersion": "1",
          "thumbprint": "5ccc0781ad2dffac626a04f1839fe8807909fb77",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "uninstall.exe",
          "majorVersion": "1",
          "thumbprint": "5ccc0781ad2dffac626a04f1839fe8807909fb77",
          "vendor": "PC Drivers Headquarters, LP"
        },
        {
          "name": "viometer.exe",
          "majorVersion": "1",
          "thumbprint": "5ccc0781ad2dffac626a04f1839fe8807909fb77",
          "vendor": "PC Drivers Headquarters, LP"
        }
      ]
    }
  }
}
```

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

1. A native application will contain a section with the name of '**AESeal**',this section contains the seal in it's binary form.
1. Extract the contents of the section and XXX
1. After obtaining the JSON text, compute the SHA256 value of the contents of the **seal** property, including the starting and ending brackets
1. Use the base64 encoded value of the **X509Cert** property to create a certificate
1. Sign the digest using the RS256 (RSASSA-PKCS-v1_5) and the certificate and compare the resulting signatures. If they match, the value of the seal is the same as what AppEsteem has certified. If they do not match, then the seal is not valid.

