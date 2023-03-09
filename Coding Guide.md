# Coding Guide

This document serves as the official code standards guide for `BurnOutSharp` internal development. Please note that this is a work in progress and may not encapsulate all standards expected of new or existing code.

## General Code Guidelines

This section contains information on code standards regardless of which part of the project you are working in.

### Style and Naming

- Prefer `System` namespaces for supporting operations before external ones.

- Ordering of `using` statements goes:
    - `using System.*`
    - `using <Alphabetical>`
    - `using static <Alphabetical>`
    - `using X <Alphabetical> = Y`

- Use 4 spaces for `tab`.

- Curly braces should generally start on the line after but inline with the start of the previous statement, even if multiline.

    ```
    if (flag)
    {
        DoSomething();
    }
    else if (flag2
        && flag3)
    {
        DoSomething2();
    }
    ```

- Multi-line statements need to have following lines indented by one step at minimum.

    ```
    if (flag)
    {
        DoSomething();
    }
    else if (flag2
        && flag3
        && (flag4
            || flag5))
    {
        DoSomething2();
    }
    ```

- Methods and classes should use `PascalCase` for naming, even `internal` and `private` ones.

- Class properties should use `PascalCase` for naming, even `protected` and `internal` ones.

- Instance variables should use `camelCase` with a `_` prefix for naming, even `protected`, `internal`, and `private` ones.

- In-method variables should use `camelCase` without a `_` prefix for naming.

- Include explicit access modifiers for all class-level properties, variables, and methods.

- Avoid making everything `public`; only include the necessary level of access.

- Avoid making every method and class instance-based. Use `static` if your method does not need to access instance variables. Use `static` if your class only contains extensions or methods used by other classes.

- Null-coalescing and null-checking operators can be used to make more readable statements and better get across what a statement or string of statements is doing.

    ```
    if (obj?.Parameter != null) { ... }

    bool value = DoSomething() ?? false;
    ```

- `#region` tags, including nested ones, can be used to both segment methods within a class and statements within a method. Indentation follows the surrounding code.

    ```
    #region This is the first region

    public static void Method()
    {
        #region This is an in-code region

        DoSomething();

        #endregion

        DoSomething2();
    }

    #endregion
    ```

- Try to avoid use of other preprocessor directives unless consulting ahead of time with the maintainers.

- Interfaces should be listed in alphabetical order

    ```
    public class Example : IBindable, IComparable, IEquatable
    ```

- Use the `<inheritdoc/>` tag when possible to avoid out-of-date information.

    ```
    public interface IInterface
    {
        /// <summary>
        /// Summary to inherit
        /// </summary>
        void DoSomething();
    }

    public class Example : IInterface
    {
        /// <inheritdoc/>
        public void DoSomething() { ... }
    }
    ```

### Methods

- Try to avoid including too much duplicate code across methods and classes. If you have duplicate code that spans more than ~5 lines, consider writing a helper method.

- Try to use expressive naming. e.g. use names like `PrintSectionTitles` and not `DoTheThing`.

- Try to avoid having too many parameters in a method signature. More parameters means more things interacting.

- Use method overloading to avoid unnecessary complexity in a single method.

    ```
    Instead of:

    Print(string idString, byte[] idArray, int idInt) { ... }

    You should:

    Print(string id) { ... }

    Print(byte[] id) { ... }

    Print(int id) { ... }
    ```

- Use optional parameters when the default value is the most common.

    ```
    Print(string id, bool toLower = false) { ... }
    ```

### `if-else` and `switch` statement syntax

- If all statements in the block are single-line, do not include curly braces.

    ```
    if (flag)
        DoSomething();
    else if (flag2)
        DoSomething2();
    else
        DoSomethingElse();
    ```

- If any of the statements is multi-line _or_ the `if-else` statement is multi-line, include curly braces.

    ```
    if (flag)
    {
        DoSomething();
    }
    else if (flag2
        && flag3
        && flag4)
    {
        DoSomething2();
    }
    else
    {
        DoSomethingElse();
        DoSomethingEvenMore();
    }
    ```

- If comparing against values, try to use a `switch` statement instead.

    ```
    As an if-else statement:
    
    if (value == 1)
        DoValue1();
    else if (value == 2)
        DoValue2();
    else if (value == 3)
        DoValue3();
    else
        DoValueDefault();

    As a switch statement:

    switch (value)
    {
        case 1:
            DoValue1();
            break;
        case 2:
            DoValue2();
            break;
        case 3:
            DoValue3();
            break;
        default:
            DoValueDefault();
            break;
    }
    ```

- When using a `switch` statement, if all switch cases are single-expression, they can be written in-line.  You can also add newlines between cases for segmentation or clarity.If the expressions are too complex, they should not be.

    ```
    switch (value)
    {
        case 1: DoValue1(); break;
        case 2: DoValue2(); break;
        case 3: DoValue3(); break;

        default: DoValueDefault(); break;
    }
    ```

- If any of the switch cases are multi-expression, write all on separate lines. You can also add newlines between cases for segmentation or clarity.

    ```
    switch (value)
    {
        case 1:
            DoValue1();
            break;
        case 2:
            DoValue2();
            break;
        case 3:
            DoValue3();
            break;

        default:
            DoValueDefault();
            DoValueAsWell();
            break;
    }
    ```

### Commenting

- All classes and methods should contain a `summary` block at bare minimum to explain the purpose. For methods, it is highly recommended to also include `param` tags for each parameter and a `return` tag if the method returns a value. Do not hesitate to use `remarks` as well to include additional information.

    ```
    /// <summary>
    /// This class is an example
    /// </summary>
    /// <remarks>
    /// This class does nothing but it is useful to demonstrate
    /// coding standards.
    /// </remarks>
    public class Example
    {
        /// <summmary>
        /// This property is the name of the thing
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// This method is an example method
        /// </summary>
        /// <param name="shouldPrint">Indicates if the value should be printed</param>
        /// <returns>A value between 1 and 10, or null on error</returns>
        public static int? PrintAndReturn(bool shouldPrint)
        {
            ...
        }
    }
    ```

- In-code comments should use the `//` syntax and not the `/* */` syntax, even for multiple lines.

    ```
    // This code block does something important
    var x = SetXFromInputs(y, z);

    // This code block does something really,
    // really, really, really important and
    // I need multiple lines to say so
    var w = SetWFromInputs(x, y, z);
    ```

- Comments should be expressive and fully explain what is being described. Try to avoid using slang, "pointed comments" such as "you should" or "we do".

- If comments include links, they can either be included as-is or using the `<see href="value"/>` tag

    ```
    // This information can be found from the following site:
    // <see href="www.regex101.com"/>
    ```

- Try to avoid using multiple, distinct comment blocks next to each other.

    ```
    // We want to try to avoid this situation where
    // we have multiple things to say.

    // Here, the statements are not inherently linked
    // but still need to go in the same area.
    //
    // But here the statements are logically linked but
    // needed additional formatting
    ```

## Project and Class Organization

This section contains information on project and class organization principles that depend on the part of the project you are working in. See the following table for details.

| Project | Description |
| --- | --- |
| `BurnOutSharp` | One file per class. See below for details on subdirectories. |
| `BurnOutSharp/External` | One directory per external project. |
| `BurnOutSharp/FileType` | One file per file type. |
| `BurnOutSharp/PackerType` | At least one file per packer type. Partial classes allowed. |
| `BurnOutSharp/ProtectionType` | At least one file per protection type. Partial classes allowed. |
| `BurnOutSharp/Tools` | Two files - one for extension methods and one for utilities. |
| `BinaryObjectScanner.ASN1` | Flat directory structure. |
| `BinaryObjectScanner.Builders` | One file per executable type. |
| `BinaryObjectScanner.Compression` | One directory per compression type. |
| `BinaryObjectScanner.Interfaces` | One file per interface. |
| `BinaryObjectScanner.Matching` | Flat directory structure. Include interfaces and base classes. |
| `BinaryObjectScanner.Models` | One directory per executable type. One file per object model. |
| `BinaryObjectScanner.Utilities` | Flat directory structure. |
| `BinaryObjectScanner.Wrappers` | One file per executable type. Common functionality goes in `WrapperBase.cs`. |
| `psxt001z` | Flat directory structure. |
| `Test` | All functionality lives in `Program.cs`. |

If the project or directory you are looking for is not included in the above, please consider it to be outside the context of this document.

## Code Organization

This section contains information on in-code organization principles that depend on the part of the project you are working in. See the following table for details.

| Project | Description |
| --- | --- |
| `BurnOutSharp` | Varies from file to file. |
| `BurnOutSharp/FileType` | `IExtractable` implementations, `IScannable` implementations, helper methods. |
| `BurnOutSharp/PackerType` | `IContentCheck` implementations, `ILinearExecutableCheck` implementations, `INewExecutableCheck` implementations, `IPortableExecutableCheck` implementations, `IPathCheck` implementations, `IExtractable` implementations, helper methods. |
| `BurnOutSharp/ProtectionType` |  `IContentCheck` implementations, `ILinearExecutableCheck` implementations, `INewExecutableCheck` implementations, `IPortableExecutableCheck` implementations, `IPathCheck` implementations, `IExtractable` implementations, helper methods. |
| `BurnOutSharp/Tools` | Methods grouped by function. Regions ordered alphabetically. |
| `BinaryObjectScanner.ASN1` | Partial classes suggested for different implmentations. |
| `BinaryObjectScanner.Builders` | Two copies of each non-generic method: one for byte arrays and one for Streams. |
| `BinaryObjectScanner.Compression` | Varies from file to file. |
| `BinaryObjectScanner.Interfaces` | Methods ordered alphabetically. |
| `BinaryObjectScanner.Matching` | Varies from file to file. |
| `BinaryObjectScanner.Models` | No methods at all, just properties. |
| `BinaryObjectScanner.Utilities` | Varies from file to file. |
| `BurnOutSharp.Wrappers` | Follow region and method grouping from existing wrappers. |
| `psxt001z` | Varies from file to file. |
| `Test` | New functionality should be added as a combination of a flag with a long and a short form, a new line in the help text, and a new method (if necessary). |

If the project or directory you are looking for is not included in the above, please consider it to be outside the context of this document.