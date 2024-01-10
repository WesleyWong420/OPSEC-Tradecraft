#include <iostream>
#include <Windows.h>
#include <winternl.h>

int main()
{
    // Create the process with fake args
    STARTUPINFO si = { sizeof(si) };
    PROCESS_INFORMATION pi;

    WCHAR fakeArgs[] = L"notepad totally-fake-args-that-is-longer-than-the-real-args.txt";

    if (CreateProcess(
        L"C:\\Windows\\System32\\notepad.exe",
        fakeArgs,
        NULL,
        NULL,
        FALSE,
        CREATE_SUSPENDED,
        NULL,
        L"C:\\",
        &si,
        &pi))
    {
        printf("Process created: %d", pi.dwProcessId);
    }

    typedef NTSTATUS(*QueryInformationProcess)(IN HANDLE, IN PROCESSINFOCLASS, OUT PVOID, IN ULONG, OUT PULONG);

    // Resolve the location of the API from ntdll.dll
    HMODULE ntdll = GetModuleHandle(L"ntdll.dll");
    QueryInformationProcess NtQueryInformationProcess = (QueryInformationProcess)GetProcAddress(ntdll, "NtQueryInformationProcess");

    // Call NtQueryInformationProcess to read the PROCESS_BASIC_INFORMATION
    PROCESS_BASIC_INFORMATION pbi;
    DWORD length;

    NtQueryInformationProcess(
        pi.hProcess,
        ProcessBasicInformation,
        &pbi,
        sizeof(pbi),
        &length);

    // With the PEB base address, we can read the PEB structure itself
    PEB peb;
    SIZE_T bytesRead;

    ReadProcessMemory(
        pi.hProcess,
        pbi.PebBaseAddress,
        &peb,
        sizeof(PEB),
        &bytesRead);

    // Read the Process Parameters
    RTL_USER_PROCESS_PARAMETERS rtlParams;

    ReadProcessMemory(
        pi.hProcess,
        peb.ProcessParameters,
        &rtlParams,
        sizeof(RTL_USER_PROCESS_PARAMETERS),
        &bytesRead);    

    // Craft new args and write them into the command line buffer
    WCHAR newArgs[] = L"notepad C:\\Users\\Wesley\\Desktop\\Note.txt";
    SIZE_T bytesWritten;

    WriteProcessMemory(
        pi.hProcess,
        rtlParams.CommandLine.Buffer,
        newArgs,
        sizeof(newArgs),
        &bytesWritten);

    ResumeThread(pi.hThread);
}