#if NET40

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System
{
    /// <summary>Defines a provider for progress updates.</summary>
    /// <typeparam name="T">The type of progress update value.</typeparam>
    /// <see href="https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/IProgress.cs"/>
    public interface IProgress<in T>
    {
        /// <summary>Reports a progress update.</summary>
        /// <param name="value">The value of the updated progress.</param>
        void Report(T value);
    }
}

#endif