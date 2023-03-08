# Developer Guide

This is a guide for any developers who wish to research protections, implement new protections in `BurnOutSharp`, or fix/update existing protection checks. Below you will find numerous sections about how to use the tools and specialized methods included in the `BurnOutSharp` project.

## Getting Started

`BurnOutSharp` contains multiple custom-built and external projects that allow for detecting copy protections, packers, and file formats. At the time of writing, below is the list of projects and what they do:

| Project | Description |
| --- | --- |
| `BurnOutSharp` | Main library that contains all supported file format, packer, and protection checks. It also houses most of the utilities, interfaces, and structures needed when `BurnOutSharp` is used by another project. Most code additions will happen here. |
| `BinaryObjectScanner.ASN1` | Library containing classes and methods associated with Abstract Syntax Notation One and OID parsing. |
| `BinaryObjectScanner.Builder` | Library containing classes that assist in populating the various object models defined in `BinaryObjectScanner.Models`. Builders can work with either byte arrays or streams for input. At the time of writing, the following executable types have builders: **MS-DOS**, **New Executable**, **Portable Executable**. |
| `BinaryObjectScanner.Compression` | Library containing classes that deal with different compression formats. This library is used extensively by the wrappers in `BinaryObjectScanner.Wrappers`. |
| `BinaryObjectScanner.Matching` | Library containing models and logic for generic searching and matching. This library is used extensively by the packer and protection checks in `BurnOutSharp`. |
| `BinaryObjectScanner.Models` | Library containing object models that represent various pieces of known executable formats. At the time of writing, the following executable types have models: **MS-DOS**, **New Executable**, **Linear Executable (partial)**, **Portable Executable**. |
| `BinaryObjectScanner.Utilities` | Library containing helper and extension methods that don't rely on any other libraries. |
| `BinaryObjectScanner.Wrappers` | Library that acts as a custom wrapper around both `BinaryObjectScanner.Builder` and `BinaryObjectScanner.Models` that allows for easier access to executable information. Each of the wrappers may also include additional functionality that would not otherwise be found in the models, e.g. Data and string reading from sections. |
| `psxt001z` | **Ported External Library** Handles detection of PS1 protections. See the README for a link to the repository. |
| `Test` | Testing executable that allows for standalone testing of the library. Includes the ability to scan files for protection as well as output executable information. |

## Researching Protections

Researching copy protections and packers can be a massive undertaking. Some can be as easy as looking for a single string in the file description while others may include searching multiple sections for bytecode that represents the right instructions or an encoded value. Thankfully for researchers, `BurnOutSharp` contains multiple tools to make this process of finding this information much easier than just poking around with a hex editor.

| Tool / Method | Description |
| --- | --- |
| `Test.exe --info [--json] <path>` | The `--info` option on the test executable is a really good way of getting started with investigation. The output of `--info` contains nearly all immediately parsable information from any executable that has a wrapper defined in `BinaryObjectScanner.Wrappers`. In general, the newer the executable format, the more information will be immediately available. For the most basic of protections and packers, this may be as far as you need to go for your research. Additionally, the `--json` flag allows for a formatted JSON output of the information for later parsing. This is only available in .NET 6+ builds. |
| `Test.exe [--debug] <path>` | Running `Test.exe` without any options runs the existing set of packer and protection checks. The output of this will be all detected packers and protections on the given file, with optional debug information where applicable. This is helpful in research because a protection you are investigating may be related to (or obscured by) another existing packer or protection. Having this information will make it easier to filter the results of `Test.exe --info <path>` as well. |
| **Add and debug** | This starts getting into more serious territory. Creating a skeleton for the packer or protection that you want to add and then messing around in code is a great way to start seeing what sort of stuff the library can see that's not normally output. See the table below for extension properties and methods that you may use in addition to the models defined in `BinaryObjectScanner.Models`. |
| **Hex Editor / External Programs** | As an advanced port of call, using a hex editor and external protection scanning programs (sometimes in conjunction) can help you get a better idea of the protection you're looking into. For example, **TheRogueArchivist** used that combination to narrow down the exact check for a very stubborn protection. |

As noted above, `BurnOutSharp` has a few tricks up its sleeve, mainly in the form of `BinaryObjectScanner.Wrappers`. This library was written explicitly to make research and implementation as easy as possible, and as such, allows for a lot of very creative ways of finding protections.

Below are all current extension properties along with a brief description.

| Executable Type | Property | Description |
| --- | --- | --- |
| **MS-DOS** | N/A | MS-DOS executables currently do not have any extension properties. |
| **New Executable (NE)** | N/A | New Executables currently do not have any extension properties. |
| **Portable Executable (PE)** | `HeaderPaddingData` | The data between the end of the PE header and the start of the first section. |
|  | `HeaderPaddingStrings` | All found ASCII and Unicode wide character strings (length >= 3) between the end of the PE header and the start of the first section. |
|  | `OverlayData` | The data between the end of the last section and either the start of the certificate table or the end of the file. |
|  | `OverlayStrings` | All found ASCII and Unicode wide character strings (length >= 3) between the end of the last section and either the start of the certificate table or the end of the file. |
|  | `SectionNames` | The ordered set of section names converted to UTF-8 strings with trailing nulls trimmed. |
|  | `StubExecutableData` | The data representing the MS-DOS executable stub code. For most programs, the stub would only print a message saying it needs Windows. |
|  | `DebugData` | Dictionary containing mappings from debug directory number to either an object representing the data (if parsed) or a byte array (if unparsed). |
|  | `ResourceData` | Dictionary containing mappings from ID to either an object representing the resource (if parsed) or a byte array (if unparsed). |
|  | `BuildGuid`, `BuildSignature`, `Comments`, `CompanyName`, `DebugVersion`, `FileDescription`, `FileVersion`, `InternalName`, `LegalCopyright`, `LegalTrademarks`, `OriginalFilename`, `PrivateBuild`, `ProductGuid`, `ProductName`, `ProductVersion`, `SpecialBuild`, `TradeName` | Version information strings, some of which are not visible in the Windows file property tab. Not all will be available for all files. |
|  | `AssemblyDescription`, `AssemblyVersion` | Assembly manifest (XML) description and version. May not be available for all files. |

Below are all current helper methods along with a brief description.

| Executable Type | Method | Description |
| --- | --- | --- |
| **MS-DOS** | N/A | MS-DOS executables currently do not have any helper methods. |
| **New Executable (NE)** | `ReadArbitraryRange(int, int)` | Reads an arbitrary range of bytes out of the new executable. **This method will be replaced in the future as proper extension properties and methods are created.** |
| **Portable Executable (PE)** | `GetVersionInfoString(string)` | Get a field from the version info string table based on the key, if the version info, string table, and key exist. Most common fields are already accessible as extension properties. See the table above for details. |
|  | `GetAssemblyManifest()` | Get the parsed XML assembly manifest, if it exists. Some common fields are already accessible as extension properties. See the table above for details. |
|  | `FindCodeViewDebugTableByPath(string)` | Find all CodeView-formatted debug tables that match a given path/filename, if they exist. |
|  | `FindGenericDebugTableByValue(string)` | Find an unparsed or custom debug table where the ASCII, Unicode, or UTF-8 representations contain a given value, if they exist. |
|  | `FindDialogByTitle(string)` | Find all dialog box resources that match a given title, if they exist. |
|  | `FindDialogBoxByItemTitle(string)` | Find all dialog box reaources that contain a dialog item that matches a given title, if they exist. |
|  | `FindStringTableByEntry(string)` | Find all string table resources that contain a given value, if they exist. |
|  | `FindResourceByNamedType(string)` | Find all resources whose type heirarchy contains a given value, if they exist. |
|  | `FindGenericResource(string)` | Find an unparsed or custom resource where the ASCII, Unicode, or UTF-8 representations contain a given value, if they exist. |
|  | `ContainsSection(string, bool)` | Checks if a given section name exists at least once in the table. |
|  | `GetFirstSection(string, bool)` | Get the first section header whose name matches the provided value, if it exists. |
|  | `GetLastSection(string, bool)` | Get the last section header whose name matches the provided value, if it exists. |
|  | `GetSection(int)` | Get the section header whose index matches the provided value, if it exists. |
|  | `GetFirstSectionData(string, bool)` | Get the first section raw data whose name matches the provided value, if it exists. |
|  | `GetLastSectionData(string, bool)` | Get the last section raw data whose name matches the provided value, if it exists. |
|  | `GetSectionData(int)` | Get the section raw data whose index matches the provided value, if it exists. |
|  | `GetFirstSectionStrings(string, bool)` | Get the first section found ASCII and Unicode wide character strings (length >= 5) whose name matches the provided value, if it exists. |
|  | `GetLastSectionStrings(string, bool)` | Get the last section found ASCII and Unicode wide character strings (length >= 5) whose name matches the provided value, if it exists. |
|  | `GetSectionStrings(int)` | Get the section found ASCII and Unicode wide character strings (length >= 5) whose index matches the provided value, if it exists. |
|  | `FindEntryPointSectionIndex()` | Get the section header index for the section that contains the entry point, if it exists. |
|  | `GetTableData(int)` | Get the table raw data whose index matches the provided value, if it exists. |
|  | `GetTableStrings(int)` | Get the table found ASCII and Unicode wide character strings (length >= 5) whose index matches the provided value, if it exists. |

## Adding a New Checker / Format

Adding a new checker or format should happen in a few distinct steps:

1. Create a skeleton class representing the new checker or format

    - If it is a new supported file type (such as an archive format), create the file in `BurnOutSharp/FileType/`. By default, you will need to implement `BurnOutSharp.Interfaces.IScannable`. Do not implement any other interfaces. Please consider asking project maintainers before doing this work, especially if there are external dependencies.

    - If it is a new supported executable packer, compressor, or installer format, create the file in `BurnOutSharp/PackerType/`. By default, you will need to implement `BurnOutSharp.Interfaces.IScannable` as well as at least one of `BurnOutSharp.Interfaces.INewExecutableCheck` and/or `BurnOutSharp.Interfaces.IPortableExecutableCheck`. It is exceptionally rare to need to implement `BurnOutSharp.Interfaces.IPathCheck`.

    - If it is a new supported DRM scheme, copy protection, or obfuscator, create the file in `BurnOutSharp/ProtectionType/`. By default, you will need to implement at least one of `BurnOutSharp.Interfaces.INewExecutableCheck`, `BurnOutSharp.Interfaces.IPortableExecutableCheck`, and/or `BurnOutSharp.Interfaces.IPathCheck`. It is exceptionally rare to need to implement `BurnOutSharp.Interfaces.IScannable`.

    - In addition to the above, there is a debug-only interface called `BurnOutSharp.Interfaces.IContentCheck`. Though there are examples of this being used in code, it is highly recommended to avoid this in a final implementation.

2. Look at other, similar classes for guidelines on how any given set of checks should be implemented. Test early and often, including using debugging tools. Err on the side of over-commenting. Do not try to be clever with your code; readable code is royalty.

3. Unless otherwise directed to by a maintainer, the only way to get changes in is through a pull request on GitHub. We do not accept patches in the form of patchfiles or archives. Please note that the maintainers may need an increased amount of time to review for obscure or hard-to-find protections.

## Updating an Existing Checker / Format

In general, if you want to update an existing checker or format, you will want to follow these steps:

1. Ensure that the change you want to make is not already in the latest source. This may sound obvious, but sometimes the check that you want to add may exist in a different form than what you were expecting _or_ it is included in a different checker entirely. Examples of this would include nearly any string finding, as there are many ways of handling this in code.

2. Ensure that your check does not interfere with any existing checks (unless it is meant to replace one of them). If your check will always be hit before the other check, consider replacing the other check. If your check is a broader version of another check, try to place it after. Interference can also mean changing shared code, such as version finding or core functionality.

3. Once you have done the above, follow the code standards that are in the file you're working in (a code standards file will be created later). Please look at other checkers for hints on where your new or updated check should live within the file, or where your new method should go compared to the others.

See the **Adding a New Checker / Format** section above for more details.