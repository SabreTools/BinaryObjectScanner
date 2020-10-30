# BurnOutSharp

[![Build status](https://ci.appveyor.com/api/projects/status/gmdft5bk1h8a1c31?svg=true)](https://ci.appveyor.com/project/mnadareski/burnoutsharp)

C# port of the protection scanning ability of [BurnOut](http://burnout.sourceforge.net/) plus numerous updates and additions. This currently compiles as a library so it can be used in any C# application. For an example of usage, see [DICUI](https://github.com/reignstumble/DICUI).

In addition to the original BurnOut code, the following libraries (or ports thereof) are used for file handling:

- [HLExtract](https://github.com/Rupan/HLLib) - Various Valve archive format extraction
- [libmspack4n](https://github.com/activescott/libmspack4n) - Microsoft CAB extraction
- [psxt001z](https://github.com/Dremora/psxt001z) - PS1 LibCrypt detection
- [SharpCompress](https://github.com/adamhathcock/sharpcompress) - 7zip/GZip/RAR/PKZIP extraction
- [StormLib](https://github.com/ladislav-zezula/StormLib) - MPQ extraction
- [StormLibSharp](https://github.com/robpaveza/stormlibsharp) - MPQ extraction
- [UnshieldSharp](https://github.com/mnadareski/UnshieldSharp) - InstallShield CAB extraction
- [WiseUnpacker](https://github.com/mnadareski/WiseUnpacker) - Wise Installer extraction

## Protections Detected

Below is a list of the protections that can be detected using this code:

- 3PLock
- 321Studios Online Activation
- AACS
- ActiveMARK / ActiveMARK 5
- Alpha-DVD
- Alpha-ROM
- Armadillo
- Bitpool
- ByteShield
- Cactus Data Shield
- CD-Cops
- CD-Lock
- CD-Protector
- CD-X
- CDSHiELD SE
- Cenga ProtectDVD
- Code Lock
- CopyKiller
- DiscGuard
- DVD-Cops
- DVD-Movie-PROTECT
- DVD Crypt
- EA Protections (Including Cucko, EA CDKey, and EA DRM)
- EXE Stealth
- Freelock
- Games for Windows - Live (partial)
- Hexalock Autolock
- Impulse Reactor
- IndyVCD
- Inno Setup
- JoWooD X-Prot (v1/v2)
- Key2Audio XS
- Key-Lock (Dongle)
- LibCrypt (Separate subfile scan only)
- LaserLock
- MediaCloQ
- MediaMax CD3
- NSIS
- Origin (partial)
- ProtectDisc
- Protect DVD-Video
- PlayStation Anti-modchip (En/Jp, not "Red Hand")
- Ring PROTECH
- SafeCast
- SafeDisc (all versions)
- SafeLock
- SecuROM (all versions)
- SmartE
- SolidShield (mostly complete)
- SoftLock
- StarForce
- Steam (partial)
- SVK Protector
- Sysiphus / Sysiphus DVD
- TAGES (mostly complete)
- TZCopyProtector
- Uplay (partial)
- UPX
- VOB ProtectCD/DVD
- Winlock
- WISE Installer
- WTM CD Protect
- WTM Copy Protection
- XCP
- Xtreme-Protector
- Zzxzz

## Contributions

Contributions to the project are welcome. Please follow the current coding styles and please do not add any keys or legally dubious things to the code. Thank you to all of the testers, particularly from the DICUI project who helped get this rolling.

