# VerifySeal Tool

## What is the VerifySeal Tool?

The **verifySeal** tool is a utility provided by AppEsteem to our customers to validate that an AppEsteem provided seal is correctly embedded within their application's files. It can also verify the JSON seal file, though without the context of a file containing the seal, it cannot verify the seal is correctly embedded.

See [How to Use the VerifySeal](..\verifyseal.md) tool for information on how to use the tool.

## How to build the VerifySeal Tool

*VerifySeal* is a Visual Studio 2017 solution that can be built using Visual Studio.

1. Open *VerifySeal.sln* using Visual Studio 2017. There will be two projects within the solution, the **VerifySeal** project containing the source code for the tool and **VerifySealUnitTests** that perform some limited testing of the tool
2. Build the solution and run the tests. (as of 5/21/2018 the DeplhiSample_Test is expected to fail)
3. Right click on the **VerifySeal** project and choose *Publish*. The *Publish* tab will appear within Visual Studio
4. Choose the target you want to publish, x86, x64 or osx-64 depending on the platform you are targeting for running the tool
5. In the **output window** the output path for the target files will be displayed, copy the destination path to where the executable file is located. All of the files in the directory may be needed depending on the platform and what is installed. The executable file will run within the folder

## Issues

If you find an issue, please create an issue within GitHub or submit a pull request with the fix

## Known Issues

- On the Mac the .Net Core 2.0 code for verifying a x509 certificate fails. When this fails, a warning will be output indicating that the certificate could not be validated
