#include <iostream>
#include <Windows.h>
#include <winternl.h>

int main(int argc, const char* argv[])
{
	// Get parent process PID from the command line
	DWORD parentPid = atoi(argv[1]);

	// Initialise STARTUPINFOEX
	STARTUPINFOEX sie = { sizeof(sie) };

	SIZE_T lpSize;
	InitializeProcThreadAttributeList(NULL, 1, 0, &lpSize);

	// Allocate memory for the attribute list on STARTUPINFOEX
	sie.lpAttributeList = (PPROC_THREAD_ATTRIBUTE_LIST)malloc(lpSize);

	// Call InitializeProcThreadAttributeList again, it should return TRUE this time
	if (!InitializeProcThreadAttributeList(sie.lpAttributeList, 1, 0, &lpSize))
	{
		printf("InitializeProcThreadAttributeList failed. Error code: %d.\n", GetLastError());
		return 0;
	}

	// Get the handle to the process to act as the parent
	HANDLE hParentProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, parentPid);

	// Call UpdateProcThreadAttribute, should return TRUE
	if (!UpdateProcThreadAttribute(sie.lpAttributeList, 0, PROC_THREAD_ATTRIBUTE_PARENT_PROCESS, &hParentProcess, sizeof(HANDLE), NULL, NULL))
	{
		printf("UpdateProcThreadAttribute failed. Error code: %d.\n", GetLastError());
		return 0;
	}

	// Call CreateProcess and pass the EXTENDED_STARTUPINFO_PRESENT flag
	PROCESS_INFORMATION pi;

	if (!CreateProcess(
		L"C:\\Windows\\System32\\notepad.exe",
		NULL,
		0,
		0,
		FALSE,
		EXTENDED_STARTUPINFO_PRESENT,
		NULL,
		L"C:\\Windows\\System32",
		&sie.StartupInfo,
		&pi))
	{
		printf("CreateProcess failed. Error code: %d.\n", GetLastError());
		return 0;
	}

	printf("PID created: %d", pi.dwProcessId);
	return 1;

	DeleteProcThreadAttributeList(sie.lpAttributeList);
}