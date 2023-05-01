# D/Invoke
Dynamic Invocation for Covert Operations (Avoiding P/Invoke & API Hooks)

## Why D/Invoke?
Rather than statically importing API calls with P/Invoke, Dynamic Invocation can be used to load the DLL at runtime and call the function using a pointer to its location in memory. You may call arbitrary unmanaged code from memory (while passing parameters), allowing you to bypass API hooking in a variety of ways and execute post-exploitation payloads reflectively. This also avoids detections that look for imports of suspicious API calls via the Import Address Table (IAT) in the .NET Assembly’s PE headers.

## Converting D/Invoke to P/Invoke
```csharp
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
public delegate UInt32 NtOpenProcess(
    ref IntPtr ProcessHandle,
    Execute.Win32.Kernel32.ProcessAccessFlags DesiredAccess,
    ref Execute.Native.OBJECT_ATTRIBUTES ObjectAttributes,
    ref Execute.Native.CLIENT_ID ClientId);
```

## Executing Unmanaged Code

1. Want to bypass IAT Hooking for a suspicious function? Use Classic D/Invoke = API Signature + Delegate + Wrapper
2. Want to avoid Inline Hooking? Manually map a fresh copy of the module and use it without any userland hooks in place. Use Manual Mapping.
3. Want to hide in plain sight? Mask the payload with a legitimate, validly signed DLL on disk. Use OverloadMapping.
4. Want to bypass all userland hooking without leaving a PE suspiciously floating in memory? Go native and use a Syscall.

## Bypassing User-land Hooking
`Manual Mapping` - This method loads a full copy of the target library file into memory. Any functions can be exported from it afterwards.
```
DInvoke.Data.PE.PE_MANUAL_MAP mappedDLL = new DInvoke.Data.PE.PE_MANUAL_MAP();
mappedDLL = DInvoke.ManualMap.Map.MapModuleToMemory(@"C:\Windows\System32\ntdll.dll");
```
`OverloadMapping` - In addition to Manual Mapping, the payload stored in memory is backed by a legitimate file on disk. So the payload will appear to be executed from a legitimate, validly signed DLL on disk.
```
DInvoke.Data.PE.PE_MANUAL_MAP mappedDLL = DInvoke.ManualMap.Overload.OverloadModule(@"C:\Windows\System32\ntdll.dll");
```
`Syscalls` - Using this technique, not the whole target library is mapped to memory but only a specified function is extracted from it. This method therefore offers more stealth than Manual Mapping and is effective in bypassing native API hooking.
```
IntPtr pAllocateSysCall = DInvoke.DynamicInvoke.Generic.GetSyscallStub("NtAllocateVirtualMemory");
NtAllocateVirtualMemory fSyscallAllocateMemory = (NtAllocateVirtualMemory)Marshal.GetDelegateForFunctionPointer(pAllocateSysCall, typeof(NtAllocateVirtualMemory));
```

## Best Practices

1. If you open handles, don’t forget to close them again.
2. When done with manually mapped modules, free them from memory to avoid memory scanners.
3. Hide code in locations it would normally exist, such as file-backed sections.

## Documentations
- [PInvoke.net](http://pinvoke.net/index.aspx)
- [DInvoke.net](https://dinvoke.net/)
- [Undocumented NTInternals](http://undocumented.ntinternals.net/index.html)
- [Syscall Table](https://j00ru.vexillium.org/syscalls/nt/64/)

## References
- [Emulating Covert Operations - Dynamic Invocation (Avoiding PInvoke & API Hooks)](https://thewover.github.io/Dynamic-Invoke/)
- [Dynamic Invocation in .NET to bypass hooks](https://blog.nviso.eu/2020/11/20/dynamic-invocation-in-net-to-bypass-hooks/)
- [A tale of EDR bypass methods](https://s3cur3th1ssh1t.github.io/A-tale-of-EDR-bypass-methods/)
- [D/Invoke & GadgetToJScript](https://rastamouse.me/d-invoke-gadgettojscript/)
- [Implementing Dynamic Invocation in C#](https://www.tevora.com/threat-blog/dynamic-invocation-in-csharp/)

