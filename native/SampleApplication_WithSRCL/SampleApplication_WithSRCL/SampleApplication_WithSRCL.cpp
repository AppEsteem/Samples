// SampleApplication_WithSRCL.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <windows.h>
#include <process.h>
#include <tchar.h>


///////////////////////////////////////////////////////////////////////////////////////
// Begin of modifications to the original program in order to display notifications.

#define USE_APPESTEEM_SRCL

#ifdef USE_APPESTEEM_SRCL
// Initialization helper.
// Contains the app id, options and seal. Uses the notification function.
// This include could be replaced by adding it separately to the compilation.
// If added to compilation no modifications whatsoever are needed.
#include "source/srcl_init.cpp" 
#endif

// End of modifications to the original program in order to display notifications.
////////////////////////////////////////////////////////////////////////////////////////


void _tmain(int argc, TCHAR *argv[])
{
	STARTUPINFO si;
	PROCESS_INFORMATION pi;

	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);
	ZeroMemory(&pi, sizeof(pi));

	if (argc != 2)
	{
		printf("Usage: SampleApplication_WithSRCL <Name of Process to Create> e.g. SampleApplication_WithSRCL calc \n");
		return;
	}
	
	Sleep(3000);
	
	// Start the child process. 
	if (!CreateProcess(NULL,   // No module name (use command line)
		argv[1],        // Command line
		NULL,           // Process handle not inheritable
		NULL,           // Thread handle not inheritable
		FALSE,          // Set handle inheritance to FALSE
		0,              // No creation flags
		NULL,           // Use parent's environment block
		NULL,           // Use parent's starting directory 
		&si,            // Pointer to STARTUPINFO structure
		&pi)           // Pointer to PROCESS_INFORMATION structure
		)
	{
		printf("CreateProcess failed (%d).\n", GetLastError());
		return;
	}

	Sleep(30000);
	// Wait until child process exits.
	WaitForSingleObject(pi.hProcess, INFINITE);

	// Close process and thread handles. 

	CloseHandle(pi.hProcess);
	
	CloseHandle(pi.hThread);
		
}
