# P/Invoke
.NET provides a mechanism called Platform Invoke (commonly known as P/Invoke) that allows .NET applications to access data and APIs in unmanaged libraries (DLLs). By using P/Invoke, a C# developer may easily make calls to the standard Windows APIs. Offensive tool developers have taken advantage of this to craft .NET Assemblies (EXEs/DLLs) that leverage the power of both the managed and unmanaged Windows APIs to perform post-exploitation tradecraft.

## Usage
```
[DllImport("kernel32.dll")]
public static extern IntPtr OpenProcess(
        ProcessAccessFlags dwDesiredAccess,
        bool bInheritHandle,
        UInt32 dwProcessId
);
```

## Limitations
Any reference to a Windows API call made through P/Invoke will result in a corresponding entry in the .NET Assembly’s Import Table. When your .NET Assembly is loaded, its Import Address Table will be updated with the addresses of the functions that you are calling. This is known as a “static” reference because the application does not need to actively locate the function before calling it. As an example, if you use P/Invoke to call `kernel32!CreateRemoteThread` then your executable’s IAT will include a static reference to that function, telling everybody that it wants to perform the suspicious behavior of injecting code into a different process. Malware analysts and automated security tools commonly inspect the IAT of executables to learn about their behavior.
