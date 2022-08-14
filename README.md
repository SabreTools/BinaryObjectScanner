# BurnOutSharp

[![Build status](https://ci.appveyor.com/api/projects/status/gmdft5bk1h8a1c31?svg=true)](https://ci.appveyor.com/project/mnadareski/burnoutsharp)

C# port of the protection scanning ability of [BurnOut](http://burnout.sourceforge.net/) plus numerous updates and additions. This currently compiles as a library so it can be used in any C# application. For an example of usage, see [MPF](https://github.com/SabreTools/MPF).

In addition to the original BurnOut code, the following libraries (or ports thereof) are used for file handling:

- [HLLibSharp](https://github.com/mnadareski/HLLibSharp) - Various Valve archive format extraction
- [LibMSPackSharp](https://github.com/mnadareski/LibMSPackSharp) - Microsoft CAB extraction [Currently unused]
- [openmcdf](https://github.com/ironfede/openmcdf) - MSI extraction
- [psxt001z](https://github.com/Dremora/psxt001z) - PS1 LibCrypt detection
- [SharpCompress](https://github.com/adamhathcock/sharpcompress) - 7zip/GZip/RAR/PKZIP extraction
- [StormLibSharp](https://github.com/robpaveza/stormlibsharp) - MPQ extraction
- [UnshieldSharp](https://github.com/mnadareski/UnshieldSharp) - InstallShield CAB extraction
- [WiseUnpacker](https://github.com/mnadareski/WiseUnpacker) - Wise Installer extraction
- [WixToolset.Dtf](https://github.com/wixtoolset/Dtf) - Microsoft CAB extraction

Please note that due to current library limitations, the functionality of StormLibSharp is locked to Windows only.

## Protections Detected

Below is a list of protections detected by BurnOutSharp. The two columns explain what sort of checks are performed to determine how the protection is detected. Generally speaking, it's better to have a content check than a path check.

| Protection Name | Content Check | Path Check | Notes |
| --------------- | ------------- | ---------- | ----- |
| 3PLock | True | False | |
| 321Studios Online Activation | True | False | |
| AACS | False | True | BluRay and HD-DVD variants detected |
| ActiveMARK | True | False | Version 5 unconfirmed²; version finding incomplete |
| AegiSoft License Manager | True | True | |
| Alpha-DVD | False | True | Unconfirmed¹ |
| Alpha-ROM | True | False | Unconfirmed¹ |
| BD+ | False | True | |
| Bitpool | False | True | |
| ByteShield | False | True | Unconfirmed¹ |
| Cactus Data Shield | True | True | |
| CD-Cops / DVD-Cops | True | True | Partially unconfirmed² |
| CD-Lock | True | True | Unconfirmed¹ |
| CD-Protector | False | True | Unconfirmed¹ |
| CD-X | False | True | Unconfirmed¹ |
| CDSHiELD SE | True | False | |
| Cenga ProtectDVD | True | True | |
| CodeLock / CodeLok / CopyLok | True | False | Partially unconfirmed² |
| CopyKiller | True | True | Unconfirmed¹ |
| Cucko (EA Custom) | True | False | Does not detect all known cases |
| Denuvo | True | False | |
| Dinamic Multimedia Protection/LockBlocks | False | True | LockBlocks needs manual confirmation of the presence of 2 rings |
| DiscGuard | True | True | Partially unconfirmed² |
| DVD-Movie-PROTECT | False | True | Unconfirmed¹ |
| DVD Crypt | False | True | Unconfirmed¹ |
| EA Protections | True | False | Including EA CDKey and EA DRM. |
| ~~Executable-Based CD Check~~ | True | False | Disabled due to overly-broad checks |
| Executable-Based Online Registration | True | False | Possibly too broad |
| Freelock | False | True | Unconfirmed¹ |
| Games for Windows - Live | True | True | |
| Hexalock AutoLock | True | True | |
| Impulse Reactor / Stardock Product Activation | True | True | |
| IndyVCD | False | True | Unconfirmed¹ |
| ITENIUM Trial & Buy Protection | True | False | |
| JoWood X-Prot (v1/v2) / Xtreme-Protector | True | False | |
| ~~Key2Audio~~ | True | True | Existing checks found to actually be indicators of OpenMG, not key2Audio specifically. |
| Key-Lock (Dongle) | True | False | Unconfirmed¹ |
| LabelGate CD | True | True | Currently only LabelGate CD2 is detected. |
| LibCrypt | True | False | Separate subfile scan only |
| LaserLok | True | True | |
| MediaCloQ | True | True | |
| MediaMax CD3 | True | True | |
| OpenMG | True | True | |
| Origin | True | True | |
| phenoProtect | False | False | Text file check only |
| ProtectDISC / VOB ProtectCD/DVD | True | False | |
| Protect DVD-Video | False | True | Unconfirmed¹ |
| PlayStation Anti-modchip | True | False | En/Jp, not "Red Hand"; PSX executables only |
| Ring PROTECH / ProRing | True | True | Unconfirmed¹ |
| SafeDisc / SafeCast | True | True | Can't distinguish between some versions of SafeDisc and SafeCast |
| SafeLock | False | True | Unconfirmed¹ |
| SecuROM | True | True | v8.x and White Label detected partially² |
| SmartE | True | True | |
| SoftLock | False | True | Unconfirmed¹ |
| SolidShield | True | True | Some Wrapper v1 not detected² |
| StarForce | True | False | Partially unconfirmed², commented out issue with `protect.exe` false positives |
| Steam | True | True | |
| SVKP (Slovak Protector) | True | False | |
| Sysiphus / Sysiphus DVD | True | False | |
| TAGES | True | True | Partially unconfirmed² |
| Tivola Ring Protection | False | True | |
| TZCopyProtector | False | True | Unconfirmed¹ |
| Uplay | True | True | |
| Windows Media Data Session DRM | True | True | |
| Winlock | False | True | Unconfirmed¹ |
| WTM CD Protect | True | True | |
| XCP | True | True | |
| Zzxzz | False | True | |

**Notes**

¹ - This means that I have not obtained one or more samples to ensure that either the original check from BurnOut or information found online is correct.

² - This is the same as ¹, but only for a subset of the checks.

## Unimplemented Protections

Below is a list of protections that have been identified but have not yet been implemented. Assistance on these would be greatly appreciated. See the source code for more details, where available.

- Alcatraz
- Alpha-Audio
- CrypKey
- DBB
- DiscAudit
- FADE
- MusicGuard
- Roxxe
- SAFEAUDIO
- The Bongle
- The Copy Protected CD
- Themida/WinLicense/Code Virtualizer

## Executable Packers Detected

Below is a list of executable packers detected by BurnOutSharp. The three columns explain what sort of checks are performed to determine how the protection is detected as well as if the contents can be extracted.

| Protection Name | Content Check | Path Check | Extractable |
| --------------- | ------------- | ---------- | ----------- |
| Advanced Installer / Caphyon Advanced Installer | Yes | No | No |
| Armadillo | Yes | No | No |
| ASPack | Yes | No | No |
| AutoPlay Media Studio | Yes | No | No |
| CExe | Yes | No | No |
| dotFuscator | Yes | No | No |
| EXE Stealth | Yes | No | No |
| Gentee Installer | Yes | No | No |
| Inno Setup | Yes | No | No |
| InstallAnywhere | Yes | No | No |
| Installer VISE | Yes | No | No |
| Intel Installation Framework | Yes | No | No |
| Microsoft CAB SFX | Yes | No | No |
| NSIS | Yes | No | No |
| PECompact | Yes | No | No |
| PEtite | Yes | No | No |
| Setup Factory | Yes | No | No |
| Shrinker | Yes | No | No |
| UPX and UPX (NOS Variant) | Yes | No | No |
| WinRAR SFX | Yes | No | Yes |
| WinZip SFX | Yes | No | Yes |
| WISE Installer | Yes | No | Yes |

## Unimplemented Packers

Below is a list of packers that have been identified but have not yet been implemented. Assistance on these would be greatly appreciated. See the source code for more details, where available.

- N/A

## Archive Formats

Below is a list of archive or archive-like formats that can be extracted and have contents scanned using this code:

- 7zip
- BFPK
- BZIP2
- GZIP
- InstallShield Archive V3 (Z)
- InstallShield CAB
- Microsoft CAB
- MPQ
- Microsoft Installer (MSI)
- PKZIP and derived files
- RAR
- TAR
- ~~Valve archive formats~~ Disabled for the forseeable future
- XZ

## Contributions

Contributions to the project are welcome. Please follow the current coding styles and please do not add any keys or legally dubious things to the code. Thank you to all of the testers, particularly from the MPF project who helped get this rolling.

## Special Thanks

I want to give a special thanks to [SilasLaspada](https://github.com/SilasLaspada) who has gone above and beyond helping both fix existing checks as well as add new ones.
