========================================================================
    CONSOLE APPLICATION : SampleApplication Project Overview
========================================================================
The SampleApplication creates a process that is supplied as an command line argument of the .exe.
For example : SampleApplication.exe <Name of the process to create> 
e.g. To create or start the calculator program, it will be triggred as : SampleApplication.exe calc 

Few more examples : 
		1. To start task manager: SampleApplication.exe taskmgr
                                     2. To start notepad : SampleApplication.exe notepad
                                     3. To start event viewer : SampleApplication.exe eventvwr

Note that some of the processes might need Administrator privileges to start the process.

This file contains a summary of what you will find in each of the files that
make up your SampleApplication application.


SampleApplication.vcxproj
    This is the main project file for VC++ projects generated using an Application Wizard.
    It contains information about the version of Visual C++ that generated the file, and
    information about the platforms, configurations, and project features selected with the
    Application Wizard.

SampleApplication.vcxproj.filters
    This is the filters file for VC++ projects generated using an Application Wizard. 
    It contains information about the association between the files in your project 
    and the filters. This association is used in the IDE to show grouping of files with
    similar extensions under a specific node (for e.g. ".cpp" files are associated with the
    "Source Files" filter).

SampleApplication.cpp
    This is the main application source file.

/////////////////////////////////////////////////////////////////////////////
Other standard files:

StdAfx.h, StdAfx.cpp
    These files are used to build a precompiled header (PCH) file
    named SampleApplication.pch and a precompiled types file named StdAfx.obj.

/////////////////////////////////////////////////////////////////////////////
Other notes:

AppWizard uses "TODO:" comments to indicate parts of the source code you
should add to or customize.

/////////////////////////////////////////////////////////////////////////////
