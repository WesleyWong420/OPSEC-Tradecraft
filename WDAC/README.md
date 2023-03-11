# Microsoft Windows Defender Application Control (WDAC)

## References
- [WDAC & LOLBins Recommended Blocklist](https://learn.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/microsoft-recommended-block-rules)
- [Ultimate WDAC Bypass List](https://github.com/bohops/UltimateWDACBypassList)
- [Arbitrary, Unsigned Code Execution Vector in Microsoft.Workflow.Compiler.exe](https://posts.specterops.io/arbitrary-unsigned-code-execution-vector-in-microsoft-workflow-compiler-exe-3d9294bc5efb)
- [Microsoft.Workflow.Compiler.exe Applocker bypass Part 1 Powershell](https://bitsekure.com/2021/03/02/microsoft-workflow-compiler-exe-applocker-bypass-part-1-powershell/)

## Bypass
- Msbuild.exe: `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Msbuild.exe msbuild.xml`
- Microsoft.Workflow.Compiler.exe: `C:\Windows\Microsoft.NET\Framework\v4.0.30319\Microsoft.Workflow.Compiler.exe test.xml test.txt`
