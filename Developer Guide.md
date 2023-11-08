# Developer Guide

This is a guide for any developers who wish to research protections, implement new protections in `BinaryObjectScanner`, or fix/update existing protection checks. Below you will find numerous sections about how to use the tools and specialized methods included in the `BinaryObjectScanner` project.

## Getting Started

`BinaryObjectScanner` contains multiple namespaces and external projects that allow for detecting copy protections, packers, and file formats. At the time of writing, below is the list of projects and what they do:

| Project | Description |
| --- | --- |
| `BinaryObjectScanner` | Main library that contains all supported file formats. It also houses most of the utilities and structures needed when `BinaryObjectScanner` is used by another project. Some code additions will happen here. |
| `BinaryObjectScanner.FileType` | Namespace containing file type definitions specific to scanning. |
| `BinaryObjectScanner.GameEngine` | Namespace containing game engine scanning definitions. |
| `BinaryObjectScanner.Interfaces` | Namespace containing interface definitions for scanning and detection. |
| `BinaryObjectScanner.Packer` | Namespace containing packer scanning definitions. |
| `BinaryObjectScanner.Protection` | Namespace containing protection scanning definitions. |
| `BinaryObjectScanner.Utilities` | Library containing helper and extension methods that don't rely on any other libraries. |
| `Test` | Testing executable that allows for standalone testing of the library. Includes the ability to scan files for protection as well as output executable information. |

## Researching Protections

Researching copy protections and packers can be a massive undertaking. Some can be as easy as looking for a single string in the file description while others may include searching multiple sections for bytecode that represents the right instructions or an encoded value. Thankfully for researchers, `BinaryObjectScanner` contains multiple tools to make this process of finding this information much easier than just poking around with a hex editor.

| Tool / Method | Description |
| --- | --- |
| `Test.exe --info [--json] <path>` | The `--info` option on the test executable is a really good way of getting started with investigation. The output of `--info` contains nearly all immediately parsable information from any executable that has a wrapper defined in `BinaryObjectScanner.Wrappers`. In general, the newer the executable format, the more information will be immediately available. For the most basic of protections and packers, this may be as far as you need to go for your research. Additionally, the `--json` flag allows for a formatted JSON output of the information for later parsing. This is only available in .NET 6+ builds. |
| `Test.exe [--debug] <path>` | Running `Test.exe` without any options runs the existing set of packer and protection checks. The output of this will be all detected packers and protections on the given file, with optional debug information where applicable. This is helpful in research because a protection you are investigating may be related to (or obscured by) another existing packer or protection. Having this information will make it easier to filter the results of `Test.exe --info <path>` as well. |
| **Add and debug** | This starts getting into more serious territory. Creating a skeleton for the packer or protection that you want to add and then messing around in code is a great way to start seeing what sort of stuff the library can see that's not normally output. See the table below for extension properties and methods that you may use in addition to the models defined in `BinaryObjectScanner.Models`. |
| **Hex Editor / External Programs** | As an advanced port of call, using a hex editor and external protection scanning programs (sometimes in conjunction) can help you get a better idea of the protection you're looking into. For example, **TheRogueArchivist** used that combination to narrow down the exact check for a very stubborn protection. |

## Adding a New Checker / Format

Adding a new checker or format should happen in a few distinct steps:

1. Create a skeleton class representing the new checker or format

    - If it is a new supported file type (such as an archive format), create the file in `BinaryObjectScanner.FileType`. By default, you will need to implement `BinaryObjectScanner.Interfaces.IDetectable` or `BinaryObjectScanner.Interfaces.IExtractable`. Do not implement any other interfaces. Please consider asking project maintainers before doing this work, especially if there are external dependencies.

    - If it is a new supported game engine or standard library, create the file in `BinaryObjectScanner.GameEngine`. By default, you will need to implement at least one of: `BinaryObjectScanner.Interfaces.ILinearExecutableCheck`, `BinaryObjectScanner.Interfaces.INewExecutableCheck`, and `BinaryObjectScanner.Interfaces.IPortableExecutableCheck`. It is exceptionally rare to need to implement `BinaryObjectScanner.Interfaces.IPathCheck`.

    - If it is a new supported executable packer, compressor, or installer format, create the file in `BinaryObjectScanner.Packer`. By default, you will need to implement `BinaryObjectScanner.Interfaces.IExtractable` as well as at least one of: `BinaryObjectScanner.Interfaces.ILinearExecutableCheck`, `BinaryObjectScanner.Interfaces.INewExecutableCheck`, and `BinaryObjectScanner.Interfaces.IPortableExecutableCheck`. It is exceptionally rare to need to implement `BinaryObjectScanner.Interfaces.IPathCheck`.

    - If it is a new supported DRM scheme, copy protection, or obfuscator, create the file in `BinaryObjectScanner.Protection`. By default, you will need to implement at least one of:`BinaryObjectScanner.Interfaces.ILinearExecutableCheck`, `BinaryObjectScanner.Interfaces.INewExecutableCheck`, `BinaryObjectScanner.Interfaces.IPortableExecutableCheck`, and `BinaryObjectScanner.Interfaces.IPathCheck`. It is exceptionally rare to need to implement `BinaryObjectScanner.Interfaces.Extractable`.

    - In addition to the above, there is a debug-only interface called `BinaryObjectScanner.Interfaces.IContentCheck`. Though there are examples of this being used in code, it is highly recommended to avoid this in a final implementation.

2. Look at other, similar classes for guidelines on how any given set of checks should be implemented. Test early and often, including using debugging tools. Err on the side of over-commenting. Do not try to be clever with your code; readable code is royalty.

3. Unless otherwise directed to by a maintainer, the only way to get changes in is through a pull request on GitHub. We do not accept patches in the form of patchfiles or archives. Please note that the maintainers may need an increased amount of time to review for obscure or hard-to-find protections.

## Updating an Existing Checker / Format

In general, if you want to update an existing checker or format, you will want to follow these steps:

1. Ensure that the change you want to make is not already in the latest source. This may sound obvious, but sometimes the check that you want to add may exist in a different form than what you were expecting _or_ it is included in a different checker entirely. Examples of this would include nearly any string finding, as there are many ways of handling this in code.

2. Ensure that your check does not interfere with any existing checks (unless it is meant to replace one of them). If your check will always be hit before the other check, consider replacing the other check. If your check is a broader version of another check, try to place it after. Interference can also mean changing shared code, such as version finding or core functionality.

3. Once you have done the above, follow the code standards that are in the file you're working in (a code standards file will be created later). Please look at other checkers for hints on where your new or updated check should live within the file, or where your new method should go compared to the others.

See the **Adding a New Checker / Format** section above for more details.
