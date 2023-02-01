# D/Invoke
Dynamic Invocation for Covert Operations (Avoiding P/Invoke & API Hooks)

## Executing Unmanaged Code

1. Want to bypass IAT Hooking for a suspicious function? Classic Way = API Signature + Delegate + Wrapper
2. Want to avoid Inline Hooking? Manually map a fresh copy of the module and use it without any userland hooks in place. Manual Mapping (Fresh ntdll.dll)
3. Want to bypass all userland hooking without leaving a PE suspiciously floating in memory? Go native and use a syscall. (GetSyscallStub)
4. Overload Mapping

### Best Practices

1. If you open handles, don’t forget to close them again.
2. When done with manually mapped modules, free them from memory to avoid memory scanners.
3. Hide code in locations it would normally exist, such as file-backed sections.
