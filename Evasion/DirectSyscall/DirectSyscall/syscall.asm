.code

NtAllocateVirtualMemory PROC
		mov r10, rcx
		mov eax, 18h
		syscall
		ret
NtAllocateVirtualMemory ENDP

NtWriteVirtualMemory PROC
		mov r10, rcx
		mov eax, 3Ah
		syscall
		ret
NtWriteVirtualMemory ENDP

NtCreateThreadEx PROC
		mov r10, rcx
		mov eax, 0C2h
		syscall
		ret
NtCreateThreadEx ENDP

NtWaitForSingleObject PROC
		mov r10, rcx
		mov eax, 4h
		syscall
		ret
NtWaitForSingleObject ENDP

end