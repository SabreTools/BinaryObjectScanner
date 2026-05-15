# BinaryObjectScanner

This library contains all of the logic for protection and file type scanning.

## Project and Class Organization

This section contains information on project and class organization principles that depend on the part of the project you are working in. See the following table for details.

| Project | Description |
| --- | --- |
| `BinaryObjectScanner` | One file per class. See below for details on subdirectories. |
| `BinaryObjectScanner/FileType` | One file per file type. |
| `BinaryObjectScanner/GameEngine` | At least one file per game engine. Partial classes allowed. |
| `BinaryObjectScanner/Interfaces` | One file per interface. |
| `BinaryObjectScanner/Packer` | At least one file per packer type. Partial classes allowed. |
| `BinaryObjectScanner/Protection` | At least one file per protection type. Partial classes allowed. |

If the project or directory you are looking for is not included in the above, please consider it to be outside the context of this document.

## Code Organization

This section contains information on in-code organization principles that depend on the part of the project you are working in. See the following table for details.

Typed checks, such as `IExecutableCheck<T>` should always follow this order: `MSDOS`, `LinearExecutable`, `NewExecutable`, `PortableExecutable`.

| Project | Description |
| --- | --- |
| `BinaryObjectScanner` | Varies from file to file. |
| `BinaryObjectScanner/FileType` | `IDetectable` implementations, helper methods. |
| `BinaryObjectScanner/GameEngine` | `IContentCheck` implementations, `IExecutableCheck<T>` implementations, `IPathCheck` implementations, helper methods. |
| `BinaryObjectScanner/Interfaces` | Methods ordered alphabetically. |
| `BinaryObjectScanner/Packer` | `IContentCheck` implementations, `IExecutableCheck<T>` implementations, `IPathCheck` implementations, helper methods. |
| `BinaryObjectScanner/Protection` | `IContentCheck` implementations, `IExecutableCheck<T>` implementations, `IPathCheck` implementations, helper methods. |

If the project or directory you are looking for is not included in the above, please consider it to be outside the context of this document.
