# Binary Object Scanner

[![Build and Test](https://github.com/SabreTools/BinaryObjectScanner/actions/workflows/build_and_test.yml/badge.svg)](https://github.com/SabreTools/BinaryObjectScanner/actions/workflows/build_and_test.yml)

C# protection, packer, and archive scanning library. This currently compiles as a library so it can be used in any C# application. A reference application called `ProtectionScan` is also included to demonstrate the abilities of the library. For an example of a program implementing the library, see [MPF](https://github.com/SabreTools/MPF).

The following projects have influenced this library:

- [BurnOut](http://burnout.sourceforge.net/) - Project that this library was initially based on. This project is fully unaffiliated with the original BurnOut and its authors.

Please visit our sibling project, [DRML](https://github.com/TheRogueArchivist/DRML), the DRM Library for a more in-depth look at some of the protections detected.

## Releases

For the most recent stable build, download the latest release here: [Releases Page](https://github.com/SabreTools/BinaryObjectScanner/releases)

For the latest WIP build here: [Rolling Release](https://github.com/SabreTools/BinaryObjectScanner/releases/tag/rolling)

**NOTE**: If you used the extraction functionality in previous releases, that has been separated into a standalone program called **[ExtractionTool](https://github.com/SabreTools/SabreTools.Serialization)**.

**NOTE**: If you used the information printing functionality in previous releases, that has been separated into a standalone program called **[InfoPrint](https://github.com/SabreTools/SabreTools.Serialization)**.

## Compatibility Notes

Binary Object Scanner strives to have both full compatibility for scanning across .NET versions as well as across OSes. Unfortunately, this is not always the case. Please see the below list for known compatibility issues.

- **7-zip archive** - Extraction is only supported on .NET Framework 4.6.2 and higher due to `SharpCompress` support limitations
- **bzip2 archive** - Extraction is only supported on .NET Framework 4.6.2 and higher due to `SharpCompress` support limitations
- **MoPaQ** - Extraction is only supported in Windows x86 and x64 builds running .NET Framework 4.5.2 and higher due to native DLL requirements
- **PKZIP and derived files (ZIP, etc.)** - Extraction is only supported on .NET Framework 4.6.2 and higher due to `SharpCompress` support limitations
- **RAR archive** - Extraction is only supported on .NET Framework 4.6.2 and higher due to `SharpCompress` support limitations
- **xz archive** - Extraction is only supported on .NET Framework 4.6.2 and higher due to `SharpCompress` support limitations

## Protections Detected

Below is a list of protections detected by BinaryObjectScanner. The two columns explain what sort of checks are performed to determine how the protection is detected. Generally speaking, it's better to have a content check than a path check.

| Protection Name | Content Check | Path Check | Notes |
| --------------- | ------------- | ---------- | ----- |
| 3PLock | True | False | |
| 321Studios Online Activation | True | False | |
| ActiveMARK | True | False | Version 5 unconfirmed²; version finding incomplete |
| AegiSoft License Manager | True | True | |
| Alpha-DVD | False | True | Unconfirmed¹ |
| Alpha-ROM | True | False | |
| Armadillo | True | False | |
| Bitpool | False | True | |
| ByteShield | True | True | |
| C-Dilla License Management Solution / CD-Secure / CD-Compress | True | True | |
| Cactus Data Shield | True | True | |
| CD-Cops / DVD-Cops | True | True | Partially unconfirmed² |
| CD-Guard | True | True | |
| CD-Lock | True | True | |
| CD-Protector | False | True | |
| CD-X | False | True | Unconfirmed¹ |
| CDSHiELD SE | True | False | |
| Cenga ProtectDVD | True | True | |
| Channelware | True | True | Version finding and detection of later versions unimplemented |
| ChosenBytes CodeLock | True | True | Partially unconfirmed² |
| copy-X | True | True | |
| CopyKiller | True | True | |
| CopyLok/CodeLok | True | False | |
| CrypKey | True | True | |
| Cucko (EA Custom) | True | False | Does not detect all known cases |
| Denuvo Anti-Cheat/Anti-Tamper| True | True | |
| DigiGuard | True | True | |
| Dinamic Multimedia Protection/LockBlocks | False | True | LockBlocks needs manual confirmation of the presence of 2 rings |
| DiscGuard | True | True | Partially unconfirmed² |
| DVD-Movie-PROTECT | False | True | Unconfirmed¹ |
| DVD Crypt | False | True | Unconfirmed¹ |
| EA Anti Cheat | True | True | |
| EA Protections | True | False | Including EA CDKey and EA DRM. |
| Easy Anti-Cheat | True | True | |
| Engine32 | True | False | |
| ~~Executable-Based CD Check~~ | True | False | Disabled due to overly-broad checks |
| Executable-Based Online Registration | True | False | Possibly too broad |
| Freelock | False | True | |
| Games for Windows - Live | True | True | |
| Gefest Protection System | True | False | |
| Hexalock AutoLock | True | True | |
| Hudson huPPPX | True | True | Basically unknown protection |
| Impulse Reactor / Stardock Product Activation | True | True | |
| IndyVCD | False | True | Unconfirmed¹ |
| INTENIUM Trial & Buy Protection | True | False | |
| JoWood X-Prot (v1/v2) / Xtreme-Protector | True | False | |
| Kalypso Launcher | True | True | |
| ~~Key2Audio~~ | True | True | Existing checks found to actually be indicators of OpenMG, not key2Audio specifically. |
| Key-Lock (Dongle) | True | False | Unconfirmed¹ |
| LabelGate CD | True | True | Currently only LabelGate CD2 is detected. |
| LibCrypt | True | False | Separate subfile scan only |
| LaserLok | True | True | |
| MediaCloQ | True | True | |
| MediaMax CD3 | True | True | |
| MGI Registration | True | False | |
| NEAC Protect | True | True | |
| nProtect GameGuard | True | True | |
| nProtect KeyCrypt | True | True | |
| OpenMG | True | True | |
| Origin | True | True | |
| phenoProtect | False | False | Text file check only |
| PlayJ | True | True | |
| ProtectDISC / VOB ProtectCD/DVD | True | True | |
| Protect DVD-Video | False | True | Unconfirmed¹ |
| PlayStation Anti-modchip | True | False | En/Jp, not "Red Hand"; PSX executables only |
| Rainbow Sentinel | True | True | |
| RealArcade | True | True | |
| Ring PROTECH / ProRing | True | True | Partially unconfirmed² |
| RipGuard | True | True | Partially unconfirmed² |
| Roxxe | True | False | |
| SafeDisc / SafeCast | True | True | Can't distinguish between some versions of SafeDisc and SafeCast |
| SafeLock | False | True | |
| SecuROM | True | True | v8.x and White Label detected partially² |
| SmartE | True | True | |
| SoftLock | True | True | |
| SolidShield | True | True | Some Wrapper v1 not detected² |
| StarForce | True | False | Partially unconfirmed², commented out issue with `protect.exe` false positives |
| Steam | True | True | |
| SVKP (Slovak Protector) | True | True | |
| Sysiphus / Sysiphus DVD | True | False | |
| TAGES | True | True | Partially unconfirmed² |
| Themida/WinLicense/Code Virtualizer | True | False | Only certain products/versions currently detected |
| ~~Tivola Ring Protection~~ | False | True | Existing checks found to actually be indicators of copy-X, rather than some Tivola-specific ring protection. |
| TZCopyProtection | False | True | Partially unconfirmed² |
| Ubisoft Orbit | False | True | |
| Uniloc SoftAnchor | True | True | Version finding is not implemented |
| Uplay | True | True | |
| Windows Media Data Session DRM | True | True | |
| Winlock | False | True | |
| WTM CD Protect | True | True | |
| XCP | True | True | |
| yuPlay Launcher | False | True | |
| Zzxzz | False | True | |

### Notes

¹ - This means that I have not obtained one or more samples to ensure that either the original check from BurnOut or information found online is correct.

² - This is the same as ¹, but only for a subset of the checks.

## Executable Packers Detected

Below is a list of executable packers detected by BinaryObjectScanner. The three columns explain what sort of checks are performed to determine how the protection is detected as well as if the contents can be extracted.

| Protection Name | Content Check | Path Check | Extractable | Notes |
| --------------- | ------------- | ---------- | ----------- | ----- |
| .NET Reactor | Yes | No | No | |
| 7-zip SFX | Yes | No | Yes | |
| Advanced Installer / Caphyon Advanced Installer | Yes | No | No | |
| ASPack | Yes | No | No | |
| AutoPlay Media Studio | Yes | No | No | |
| CExe | Yes | No | Yes | |
| dotFuscator | Yes | No | No | |
| Embedded File | Yes | No | Yes | Not technically a packer |
| EXE Stealth | Yes | No | No | |
| Gentee Installer | Yes | No | No | Includes "Create Install 2003" |
| Ghost Installer | Yes | No | No | |
| GkWare SFX | Yes | No | No | |
| GP-Install | Yes | No | No | |
| HyperTech CrackProof | Yes | No | No | |
| Inno Setup | Yes | No | No | |
| InstallAnywhere | Yes | No | No | |
| Installer VISE | Yes | No | No | |
| Intel Installation Framework | Yes | No | No | |
| Microsoft CAB SFX | Yes | No | No | |
| MPRESS | Yes | No | No | |
| NeoLite | Yes | No | No | Only confirmed to detect version 2.X |
| NSIS | Yes | No | No | |
| PECompact | Yes | No | No | |
| PEtite | Yes | No | No | |
| PKLITE32 | Yes | No | No | |
| Reflexive Arcade Installer | Yes | Yes | No | |
| Setup Factory | Yes | No | No | |
| Shrinker | Yes | No | No | |
| Smart Install Maker | Yes | No | No | |
| Spoon Installer | Yes | No | No | |
| UPX and UPX (NOS Variant) | Yes | No | No | |
| WinRAR SFX | Yes | No | Yes | |
| WinZip SFX | Yes | No | Yes | |
| WISE Installer | Yes | No | Yes | |
| Zylom Wrapper | Yes | No | No | |

## Game Engines Detected

Below is a list of game engines detected by BinaryObjectScanner. The two columns explain what sort of checks are performed to determine how the protection is detected. Generally speaking, it's better to have a content check than a path check.

| Protection Name | Content Check | Path Check | Notes |
| --------------- | ------------- | ---------- | ----- |
| RenderWare | Yes | No | |

## Container Formats

Below is a list of container formats that are supported in some way:

| Format Name | Reported | Extracted | Notes |
| --- | --- | --- | --- |
| 7-zip archive | No | Yes | Via `SharpCompress` |
| AACS Media Key Block | Yes | N/A | BluRay and HD-DVD variants detected |
| BD+ SVM | Yes | N/A | |
| BFPK custom archive format | No | Yes | |
| bzip2 archive | No | Yes | Via `SharpCompress` |
| Compound File Binary (CFB) | No | Yes | |
| gzip archive | No | Yes | |
| Half-Life Game Cache File (GCF) | No | Yes | |
| Half-Life Level (BSP) | No | Yes | |
| Half-Life Package File (PAK) | No | Yes | |
| Half-Life Texture Package File (WAD3) | No | Yes | |
| Half-Life 2 Level (VBSP) | No | Yes | |
| INI configuration file | No | N/A | Used in other detections currently |
| InstallShield Archive V3 (Z) | No | Yes | |
| InstallShield CAB | No | Yes | |
| Linear Executable | No* | No | No existing checks |
| Link Data Security encrypted file | Yes | No | |
| Microsoft cabinet file | No | Yes* | Does not support LZX or Quantum compression |
| Microsoft LZ-compressed files | No | Yes | |
| MoPaQ game data archive (MPQ) | No | Yes* | Via `StormLibSharp` |
| MS-DOS Executable | No* | No | No existing checks |
| New Exectuable | Yes | Yes | Subset of extractions implemented |
| NovaLogic Game Archive Format (PFF) | No | Yes | |
| PKZIP and derived files (ZIP, etc.) | No | Yes | Via `SharpCompress` |
| PlayJ audio file (PLJ) | Yes | No | Undocumented file format |
| Portable Executable | Yes | Yes | Subset of extractions implemented |
| Quantum archive (Q) | No | No | Extraction disabled due to compatibility issues |
| RAR archive (RAR) | No | Yes | Via `SharpCompress` |
| RealArcade Installer (.rgs) | Yes | No | |
| RealArcade Mezzanine (.mez) | Yes | No | |
| SGA game archive | No | Yes | |
| StarForce Filesystem file (SFFS) | Yes | No | |
| Tape archive (TAR) | No | Yes | |
| Valve Package File (VPK) | No | Yes | |
| XBox Package File (XZP) | No | Yes | |
| xz archive (XZ) | No | Yes | Via `SharpCompress` |

## Contributions

Contributions to the project are welcome. Please follow the current coding styles and please do not add any keys or legally dubious things to the code. Thank you to all of the testers, particularly from the MPF project who helped get this rolling.

## Special Thanks

I want to give a special thanks to [TheRogueArchivist](https://github.com/TheRogueArchivist) and [HeroponRikiBestest](https://github.com/HeroponRikiBestest) who have gone above and beyond helping both fix existing checks as well as add new ones.
