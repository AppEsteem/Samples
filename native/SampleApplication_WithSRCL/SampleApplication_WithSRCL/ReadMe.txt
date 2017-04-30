========================================================================
    CONSOLE APPLICATION : SampleApplication_WithSRCL Project Overview
========================================================================

The SampleApplication_WithSRCL creates a process that is supplied as an command line argument of the .exe.
For example : SampleApplication_WithSRCL <Name of the process to create> 
e.g. To create or start the calculator program, it will be triggred as : SampleApplication_WithSRCL calc 

Few more examples : 
		1. To start task manager: SampleApplication_WithSRCL.exe taskmgr
                                     2. To start notepad : SampleApplication_WithSRCL.exe notepad
                                     3. To start event viewer : SampleApplication_WithSRCL.exe eventvwr

Note that some of the processes might need Administrator privileges to start the process.

This file contains a summary of what you will find in each of the files that
make up your SampleApplication_WithSRCL application.


SampleApplication_WithSRCL.vcxproj
    This is the main project file for VC++ projects generated using an Application Wizard.
    It contains information about the version of Visual C++ that generated the file, and
    information about the platforms, configurations, and project features selected with the
    Application Wizard.

SampleApplication_WithSRCL.vcxproj.filters
    This is the filters file for VC++ projects generated using an Application Wizard. 
    It contains information about the association between the files in your project 
    and the filters. This association is used in the IDE to show grouping of files with
    similar extensions under a specific node (for e.g. ".cpp" files are associated with the
    "Source Files" filter).

SampleApplication_WithSRCL.cpp
    This is the main application source file.

/////////////////////////////////////////////////////////////////////////////
Other standard files:

StdAfx.h, StdAfx.cpp
    These files are used to build a precompiled header (PCH) file
    named SampleApplication_WithSRCL.pch and a precompiled types file named StdAfx.obj.

/////////////////////////////////////////////////////////////////////////////
Other notes:

AppWizard uses "TODO:" comments to indicate parts of the source code you
should add to or customize.

/////////////////////////////////////////////////////////////////////////////
