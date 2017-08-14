# AppEsteem Sample Repository

This repository contains samples for using the AppEsteem SDK. It has detailed instructions and samples show how to use the different AppEsteem components within  your application.

## Self Regulating Client Library (SRCL)

SRCL can be used within either a native (C++) application or within a managed (C#) application. The instructions for native can be found in the integrate with your code section of this document.

### To obtain the library
Both versions of the library will be made available on [NuGet](https://www.nuget.org/packages?q=AppEsteem). They can be downloaded directly using NuGet if you don't have Visual Studio installed.

### Integrate with your code

There are a few different to ways to do this, managed, native, and chrome

[Managed Sample](managed/managedsample.md)

[Native Sample](native/nativesample.md)

[Chrome Extension Sample](chrome/chromesample.md)

### Validate a seal

While the concepts of validating the seal are the same for both managed and native code, the details are different.

[Managed](managed/validatesealcsharp.md)

[Native](native/validateseal.md)

[Chrome](chrome/validateseal.md)
