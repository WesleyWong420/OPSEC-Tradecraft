# Ghostpack
A collection of pre-compiled .NET binaries:

- [ADSearch](https://github.com/tomcarver16/ADSearch)
- [Certify](https://github.com/GhostPack/Certify)
- [ForgeCert](https://github.com/GhostPack/ForgeCert)
- [Rubeus](https://github.com/GhostPack/Rubeus)
- [Seatbelt](https://github.com/GhostPack/Seatbelt)
- [SharPersist](https://github.com/h4wkst3r/SharPersist)
- [SharpChromium](https://github.com/djhohnstein/SharpChromium)
- [SharpDPAPI](https://github.com/GhostPack/SharpDPAPI)
- [SharpGPOAbuse](https://github.com/FSecureLABS/SharpGPOAbuse)
- [SharpHound](https://github.com/BloodHoundAD/SharpHound)
- [SharpSpoolTrigger](https://github.com/cube0x0/SharpSystemTriggers)
- [SharpUp](https://github.com/GhostPack/SharpUp)
- [SharpView](https://github.com/tevora-threat/SharpView)
- [SharpWMI](https://github.com/GhostPack/SharpWMI)
- [SpoolSample](https://github.com/leechristensen/SpoolSample)
- [Mimikatz](https://github.com/gentilkiwi/mimikatz)

Optimization for Evasion via Microsoft Visual C++ Compiler (MSVC):

`cl.exe /O2 /Ob2 /Os /Gs- /Zi /EHsc- /GL /Os /GF /Gy /GA main.cpp`

Optimization for Evasion via Rust Compiler (Rustc):

`RUSTFLAGS="-Zlocation-detail=none" cargo +nightly build --release`
