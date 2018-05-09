# How to Use VerifySeal Tool

## What is the VerifySeal Tool?

The **verifySeal** tool is a utility provided by AppEsteem to our customers to validate that an AppEsteem provided seal is correctly embedded within their application's files. It can also verify the JSON seal file, though without the context of a file containing the seal, it cannot verify the seal is correctly embedded.

## How to Use VerifySeal Tool

Using **VerifySeal** is pretty easy, it has few options

Usage

    VerifySeal.exe <filepath> [/h] [/v]

Argument | Description
------------- | --------------
filepath | Path and name of the executable or JSON file to evaluate
/h | Displays the help message
/v | Displays detailed information about the seal

## Sample VerifySeal Tool Output

When the tool is run against a file containing an embedded seal, successful out will be similar to:

```text
  Seal header contains a signature ... Ok
  Seal header contains a certificate ... Ok
  Seal header contains a seal to validate ... Ok
  Certificate has a valid chain ... Ok
  Certificate was issued to AppEsteem Corporation ... Ok
  Seal valid interval overlaps with the effective dates of the certificate ... Ok
  File 'SampleAp.exe' version '1.0.0.1' matches a file in the seal ... Ok
  Signature matches seal content ... Ok
```


If the tool is run against the JSON seal file, successful output will be similar to:

```text
  Seal header contains a signature ... Ok
  Seal header contains a certificate ... Ok
  Seal header contains a seal to validate ... Ok
  Certificate has a valid chain ... Ok
  Certificate was issued to AppEsteem Corporation ... Ok
  Seal valid interval overlaps with the effective dates of the certificate ... Ok
  No files are specified as part of the seal ... Warning
  Signature matches seal content ... Ok
```

If the /v flag is specified, additional information about the seal will be output similar to:

```text
Seal Information

  Seal is valid for files created between 2018-04-23 and 2019-04-23

  Application Information
    Product Name: Sample Application
    Product Version: 1.0.0.1
    First Certified Version: 1.0.0.1
    Application Type: Windows Executable
    Certified: yes

  Attestations
    Vendor Name: Radhika-test
    Value Proposition: SampApp
    Audience:
      consumer
    Age: Adults only
    Categories:
      Books & Reference
      Family & Kids
      Health & Fitness
      Medical
      Shopping
      Travel & Navigation
    Monetization:
      paid
      up-sell to paid
    Target:
      Chrome
      Firefox
      IE
      Windows XP
      Windows Vista
      Chrome

  Distribution
    Whitelist Landing Pages:
      www.sampleapp.com
    Whitelist Installers: 
      SampleAp.exe
    Whitelist Affiliates: None
    Whitelist Ad Networks: None
    Blacklist Landing Pages: None
    Blacklist Download Urls: None
    Blacklist Installers: None
    Blacklist Affiliates: None
    Blacklist Ad Networks: None

  Contents
    Name                          Version       Vendor                Thumbprint
    ----------------------------------------------------------------------------------------------------------
    SampleAp.exe                  4. 2. 0. 13   Radhika-test          5290cd0b9eff8ebc15a5fb0530b9e5dc4de9d58a
```
