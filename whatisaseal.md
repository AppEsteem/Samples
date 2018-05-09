# What is an AppEsteem Seal?

**IMPORTANT NOTE** - _Please read_
***
It is _very important_ to note that seals can be used in two ways. Having a seal within application doesn't mean that AppEsteem has certified that application. Vendors are able to create a seal without the application being certified by AppEsteem for testing purposes and evaluation. It is critically important when validating a seal to check the **certification** property within the *attestation* section. AppEsteem certified applications will have the value 'yes'. If the property is 'no' or the property is not present, the application **must not** be be considered certified by AppEsteem.

***
## What is a seal

A seal is a block of JSON containing information that AppEsteem is certifying about the application containing the seal. An example of a complete seal is shown below. Take a look at the complete seal, then we'll continue with details of each section.

```json
{
    "header": {
        "copyright": "Copyright © AppEsteem Corporation. All Rights Reserved.",
        "description": "Digitally signed seal for AppEsteem Corporation.",
        "comments": "Generated file, do not edit! If the content is changed in any way, including adding spaces or newline characters, the digital signature will be broken and the seal will be reported as being forged.",
        "signature": "Zw3vAcdk/s97fy8IH7EtF++widI1TS8rbLUg9vh0Kq+qBWSa+i3mR45f4FnbwY8toi6DAhwbdGdBe+Ygk63DYxFEqLjY09buRx8gN/HnS/YPTBJibey89yjTTmKH4+TG52gwgRqK8WEJ5LIK9N1pmwUV7Se/y6BAu/6GXRhipL2kejg3fVZQwe4u7y1R6RkASdHnk4MZN8ol8A8EsZ9q6YiThT90DtK8okO99HrVdWwU9CDr6tTj/wp1cc8Di6WgUHZcqbRoNJtLkS0K4GFz/nmpFTu0OBZrGHMGO4shKvLMH3K1ITpDsvIhXz04/CX98GxjryasS/NG15YisfZfOg==",
        "x509Cert": "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"
    },
    "seal": {
        "serialNumber": "83103788-d435-ff3a-eee5-82096031eabc",
        "applicationIdentification": {
            "appGuid": "fe3ae840-7221-46f4-b6b7-7d5679a39c4c",
            "appId": "sampleapplication-180423",
            "productName": "Sample Application",
            "version": "1.0.0.1",
            "firstCertifiedVersion": "1.0.0.1",
            "applicationType": "Windows Executable"
        },
        "attestations": {
            "vendor": "Radhika-test",
            "certified": "yes",
            "firstCertifiedVersion": "1.0.0.1",
            "valueProposition": "SampApp",
            "age": "12+ appropriate",
            "audience": [
                "",
                "consumer"
            ],
            "category": [
                "",
                "Books & Reference",
                "Family & Kids",
                "Health & Fitness",
                "Medical",
                "Shopping",
                "Travel & Navigation"
            ],
            "monetization": [
                "",
                "paid",
                "up-sell to paid"
            ],
            "target": [
                "",
                "Chrome",
                "Firefox",
                "IE",
                "",
                "Windows Vista",
                "Windows XP",
                "Chrome"
            ],
            "bundlersCarrying": [],
            "bundlersOffering": []
        },
        "validDates": {
            "validForFilesSignedAfter": "2018-04-23",
            "validForFilesSignedBefore": "2019-04-23"
        },
        "distribution": {
            "whitelist": {
                "landingPages": [
                    "www.sampleapp.com"
                ],
                "downloadUrls": [],
                "installers": [
                    "SampleAp.exe"
                ],
                "affiliates": [],
                "adNetworks": []
            }
        },
        "contents": [
            {
                "name": "SampleAp.exe",
                "version": "1.0.0.1",
                "thumbprint": "5290cd0b9eff8ebc15a5fb0530b9e5dc4de9d58a",
                "vendor": "Radhika-test"
            }
        ]
    }
}
```

### Header section

The header section contains the certificate and the resulting signature that validates the contents of the **seal** section. The header also contains a copyright notice, a description and comments about how to work with a seal.

The **x509Cert** property contains the X509 certificate with a public key that can be used to generate a signature of the seal's digest and if it matches the **signature** property, then the contents of the seal are valid. If the signatures don't match, then the contents of the seal should bot be trusted and the seal is not valid. The certificate is a based64 encoded byte array containing the public portion of an AppEsteem signing certificate. The certificate must  be validated to be issued to AppEsteem and to have a valid certificate chain. The certificate was embedded into the seal to allow verification without making a request to AppEsteem to validate the seal or the certificate.

The signature section contains a base64 encoded byte array containing the result of an RS256 signing operation on the digest value of the **seal** property, including the '{' and '}' brackets within the UTF8 encoded string of JSON. AppEsteem uses Azure KeyVault for the [signing operation](https://docs.microsoft.com/rest/api/keyvault/sign).

### Seal section

The seal contains a number of additional properties, each will be covered in detail within this section. Seals are version specific because they describe a set of attributes for a specific version of an application. A seal generated for one version of an application **cannot** be used with another version of an application.

#### serialNumber property

The serial number property uniquely identifies the seal. If two seals share the same object values, they will have the same serial number. The serial number should not be used to compare the contents of seals with one another.

#### applicationIdentification object

This object is used by AppEsteem to identify the application using a combination of a identifiers for the application. All properties are optional, but the seal may not be able to be successfully validated without all of the properties.

* **appGuid** - Uniquely identifiers an application across versions
* **appId** - Human readable identifier for an application
* **productName** - The name of the application as entered by the application vendor
* **version** - The version of the application for which this seal was generated
* **firstCertifiedVersion** - The first certified version of this application
* **dateFirstCertified** - The date this version was first certified
* **applicationType** - The type of application. Valid values are "Windows Executable"

#### attestations object

This section contains information that AppEsteem has validated such as

* **vendor** - Name of the application's vendor
* **certified** - Indicates the certification status of the application. Only applications with a valid seal and a value of '**yes**' for this property are certified
* **firstCertifiedVersion** - The first certified version of this application
* **valueProposition** - Value propositions as states by the vendor
* **age** - Age appropriate rating recommendation
* **audience** - Array of target audiences for the application
* **category** - Array or categories of software this application falls within
* **monetization** - How the application is monetized
* **target** - What operating environments are supported by the application
* **bundlersCarrying** - Array of bundlers carrying the application
* **bundlersOffering** - Array of bundlers offering the application

#### validDates object

The valid date section is important because it informs if the file this seal is embedded within is certified to belong to an application. Currently, AppEsteem seals are valid for one year. To have a valid seal, the creation date of the file containing the seal must fall within the date range specified within this section. For instance, if the **validForFilesSignedAfter** is 1 Jan 2018 and the **validForFilesSignedBefore** is 31 Dec 2018. The file creation date must be within the year of 2018. If the files creation date is outside that range, the file is not certified as part of the application. Because the certificate is distributed as part of the seal, the certificate may expire while the file is still being distributed. The certificate dates must be valid for the time interval of the valid date ranges as well as the date the file was created.

#### distribution object

The distribution section contains an array of the white or black listed landing pages and download Urls this application can be distributed from. If it is being distributed from another location, this application should be considered invalid. *Note* in the example only the **whitelist** property is present, a **blacklist** property may also be present. It also contains an array of **installers**, **affiliates** and **adNetworks**.

#### contents object

This is an array of information about the files that are covered by this seal. If other files are identified as part of this application, but are not within this array, they are not part of what AppEsteem has certified. Each file contains

* **name** - Name of the file as identified by the OriginalFilename or FileName properties respectively
* **version** - The version of the file as identified  by the FileVersion property of the file
* **thumbprint** - Thumbprint of one of the certificates used to sign the file
* **vendor** - Name of the vendor
