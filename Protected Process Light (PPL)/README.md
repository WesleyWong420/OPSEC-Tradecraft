# Protected Process Light (PPL)
Anti-Malware Self-Defense Bypass Techniques

## Windows Defender
| Component    | Description        | Security Context | Protection Level  |
|:------------:|:------------------:|:----------------:|:-----------------:|
| MsMpEng.exe  | Main Engine        | SYSTEM           | Antimalware Light |
| NisSrv.exe   | Network Inspection | Local Service    | Antimalware Light |
| MpCmdRun.exe | User Interface     | Current User     | -                 |

## References
- [Bypassing LSA Protection (aka Protected Process Light) without Mimikatz on Windows 10](https://redcursor.com.au/bypassing-lsa-protection-aka-protected-process-light-without-mimikatz-on-windows-10/)
- [Hooking System Calls in Windows 11 22H2 like Avast Antivirus. Research, analysis and bypass](https://the-deniss.github.io/posts/2022/12/08/hooking-system-calls-in-windows-11-22h2-like-avast-antivirus.html)
- [Red Team Techniques For Evading Bypassing And Disabling MS Advanced Threat Protection And Advanced Threat Analytics](https://www.blackhat.com/docs/eu-17/materials/eu-17-Thompson-Red-Team-Techniques-For-Evading-Bypassing-And-Disabling-MS-Advanced-Threat-Protection-And-Advanced-Threat-Analytics.pdf)
- [Disabling AV With Process Suspension](https://www.trustedsec.com/blog/disabling-av-with-process-suspension/)

## Vidoes
- [How To Bypass AM-PPL & Disable EDRs - A Red Teamer's Story-Stephen Kho & Juan Sacco | Nullcon Berlin](https://www.youtube.com/watch?v=QtObgEfy5Jw)
