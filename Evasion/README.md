# Evasion

## Project Description

- **CommandLineSpoofing**
  - *Drawback: A process cannot change its own command line arguments.*
- **PPIDSpoofing**
  - *Drawback: Mismatched parent and creator process IDs.*
- **ETWLoader**
  - ETW Patching via EtwEventWrite in C#
- **ETWBlinder**
  - ETW Patching via NtTraceEvent in C#
- **DirectSyscall**
  - Raw Implementation of Direct Syscall in C++ Without D/Invoke & SysWhispers
  - *Drawback: Abnormal for user-land application to execute syscall directly, causing a malformed call stack where code execution does not flow through the userland DLLs when a direct syscall is made, but directly to the kernel.*
- **ManualMapping**
  - Raw Implementation of Manual Mapping in C++ Without D/Invoke
  - *Drawback: Loading of ntdll.dll into single process multiple times.*
- **GhostMapping**
  - Raw Implementation of Manual Mapping via Suspended Process in C++ Without D/Invoke
  - *Drawback: Rare for applications to create a temporary suspended process for legitimate reasons.*

## Resources

### EDR Evasion
- [A story about tampering EDRs](https://redops.at/en/blog/a-story-about-tampering-edrs)
- [Meterpreter vs Modern EDR(s)](https://redops.at/en/blog/meterpreter-vs-modern-edrs-in-2023)
- [Direct Syscalls: A journey from high to low](https://redops.at/en/blog/direct-syscalls-a-journey-from-high-to-low)
- [Direct Syscalls vs Indirect Syscalls](https://redops.at/en/blog/direct-syscalls-vs-indirect-syscalls)
- [A tale of EDR bypass methods](https://s3cur3th1ssh1t.github.io/A-tale-of-EDR-bypass-methods/)
- [Naughty Hooking Detoxifying Memory Before Doing Crime](https://dazzyddos.github.io/posts/Naughty_Hooking_Detoxifying_Memory/#inside-the-hook-manipulating-functions-the-cool-way)
- [A Syscall Journey in the Windows Kernel](https://alice.climent-pommeret.red/posts/a-syscall-journey-in-the-windows-kernel/)
- [Bypassing User-Mode Hooks and Direct Invocation of System Calls for Red Teams](https://www.mdsec.co.uk/2020/12/bypassing-user-mode-hooks-and-direct-invocation-of-system-calls-for-red-teams/)
- [Red Team Tactics: Combining Direct System Calls and sRDI to bypass AV/EDR](https://outflank.nl/blog/2019/06/19/red-team-tactics-combining-direct-system-calls-and-srdi-to-bypass-av-edr/)
- [Blindside: A New Technique for EDR Evasion with Hardware Breakpoints](https://cymulate.com/blog/blindside-a-new-technique-for-edr-evasion-with-hardware-breakpoints)
- [Cat & Mouse - or Chess?](https://s3cur3th1ssh1t.github.io/Cat_Mouse_or_Chess/)

### Memory Evasion
- [Avoiding Memory Scanners](https://www.blackhillsinfosec.com/avoiding-memory-scanners/)
- [In-Memory Disassembly for EDR/AV Unhooking](https://signal-labs.com/analysis-of-edr-hooks-bypasses-amp-our-rust-sample/)
- [EDR bypassing via memory manipulation techniques](https://labs.withsecure.com/publications/edr-bypassing-via-memory-manipulation-techniques)
- [Detecting and Advancing In-Memory .NET Tradecraft](https://www.mdsec.co.uk/2020/06/detecting-and-advancing-in-memory-net-tradecraft/)
- [Reducing The Indicators of Compromise (IOCs) on Beacon and Team Server](https://perspectiverisk.com/reducing-the-indicators-of-compromise-iocs-on-beacon-and-team-server/)
- [SleepyCrypt: Encrypting a running PE image while it sleeps](https://www.solomonsklash.io/SleepyCrypt-shellcode-to-encrypt-a-running-image.html)
- [An Introduction into Sleep Obfuscation](https://dtsec.us/2023-04-24-Sleep/)
- [An Introduction into Stack Spoofing](https://dtsec.us/2023-09-15-StackSpoofin/)
- [Spoofing Call Stacks To Confuse EDRs](https://labs.withsecure.com/publications/spoofing-call-stacks-to-confuse-edrs)
- [Reflective call stack detections and evasions](https://securityintelligence.com/x-force/reflective-call-stack-detections-evasions/)

### Antimalware Scan Interface (AMSI)
- [Memory Patching AMSI Bypass ](https://rastamouse.me/memory-patching-amsi-bypass/)
- [[Malware] Bypass AMSI in local process hooking NtCreateSection](https://waawaa.github.io/es/amsi_bypass-hooking-NtCreateSection/)
- [Bypass AMSI on Windows 11](https://gustavshen.medium.com/bypass-amsi-on-windows-11-75d231b2cac6)

### Event Tracing for Windows (ETW)
- [ETWHash – “He who listens, shall receive”](https://labs.nettitude.com/blog/etwhash-he-who-listens-shall-receive/)

### Videos
- [Evasion in Depth - Techniques Across the Kill-Chain by Mariusz Banach](https://youtu.be/IbA7Ung39o4)
- [Intro to Syscalls for Windows Malware](https://youtu.be/elA_eiqWefw)
- [AV/EDR Evasion: Packer Style](https://youtu.be/Q2vazB6SYfg)
- [Knocking Out Post-Exploitation Kits](https://youtu.be/kVYlYAR2R7E?si=YUdLiXlMdc6Z9FT1)
