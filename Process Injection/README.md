# Process Injection
Process Injection Techniques & Shellcode Loaders

## Process Injection
> [Ten process injection techniques: A technical survey of common and trending process injection techniques](https://www.elastic.co/blog/ten-process-injection-techniques-technical-survey-common-and-trending-process)
- Classic Shellcode Injection
- Classic PE Injection
- Classic DLL Injection
- Reflective DLL Injection
- Shellcode Reflective DLL injection (sRDI)
- Thread Hijacking
- Process Hollowing
- Transacted Hollowing
- Process Ghosting
- Process Herpaderping
- Process Doppelganging
- Process Reimaging
- Module Stomping
- Function Stomping
- Asychronous Procedure Call (APC) Injection
- Inter-Process Mapped View
- Early Bird
- Atom Bombing
- Hell's Gate

## Shellcode Techniques
- OpenProcess
- CreateProcess
- CreateProcessWithPipe
- CreateThread
- CreateThreadNative
- CreateRemoteThread
- CreateRemoteThreadNative
- CreateFiber
- EtwpCreateEtwThreadEx
- RtlCreateUserThread
- NtQueueApcThread
- UuidFromStringA & EnumSystemLocalA

## OPSEC
- [Exploring Process Injection OPSEC – Part 1](https://rastamouse.me/exploring-process-injection-opsec-part-1/)
- [Exploring Process Injection OPSEC – Part 2](https://rastamouse.me/exploring-process-injection-opsec-part-2/)
- [A story about tampering EDRs](https://redops.at/en/blog/a-story-about-tampering-edrs)
- [Meterpreter vs Modern EDR(s)](https://redops.at/en/blog/meterpreter-vs-modern-edrs-in-2023)
- [Process injection in 2023, evading leading EDRs](https://vanmieghem.io/process-injection-evading-edr-in-2023/)

## Evasion
- [Avoiding Memory Scanners](https://www.blackhillsinfosec.com/avoiding-memory-scanners/)
- [Alternative use cases for SystemFunction032](https://s3cur3th1ssh1t.github.io/SystemFunction032_Shellcode/)
- [Direct Syscalls: A journey from high to low](https://redops.at/en/blog/direct-syscalls-a-journey-from-high-to-low)
- [Bypassing User-Mode Hooks and Direct Invocation of System Calls for Red Teams](https://www.mdsec.co.uk/2020/12/bypassing-user-mode-hooks-and-direct-invocation-of-system-calls-for-red-teams/)

## Donut
- [Donut - Injecting .NET Assemblies as Shellcode](https://thewover.github.io/Introducing-Donut/)
- [Donut v0.9.1 "Apple Fritter" - Dual-Mode Shellcode, AMSI, and More](https://thewover.github.io/Apple-Fritter/)
- [Donut v0.9.2 "Bear Claw" - JScript/VBScript/XSL/PE Shellcode and Python Bindings](https://thewover.github.io/Bear-Claw/)
- [Donut v1.0 "Cruller" - ETW Bypasses, Module Overloading, and Much More](https://thewover.github.io/Cruller/)

## Sample Code
- [Code injection via memory sections and ZwQueueApcThread](https://cocomelonc.github.io/tutorial/2022/01/17/malware-injection-14.html)
- [Red Team C Code Repo](https://github.com/Mr-Un1k0d3r/RedTeamCCode)
- [Shellcode Injection Techniques](https://github.com/plackyhacker/Shellcode-Injection-Techniques)
- [Rust D/Invoke Dropper](https://github.com/Nariod/Tartocitron)
- [Advanced Process Injection Workshop by CyberWarFare Labs](https://github.com/RedTeamOperations/Advanced-Process-Injection-Workshop)

## Vidoes
- [Defeating EDRs using Dynamic invocation by Jean-Francois Maes](https://youtu.be/LXfhyTpQ7TM)
- [Fun with Shellcode (Loaders)](https://youtu.be/HNGuM5LpOEw)
- [DC29 - Adversary village - Workshop: From zero to hero: creating a reflective loader in C#](https://youtu.be/E6LOQQiNjj0)
- [Roll for Stealth Intro to AV EDR Evasion | Mike Saunders | WWHF Deadwood 2022](https://youtu.be/TvPE5EAObHw)
