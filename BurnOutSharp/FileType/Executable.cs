﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BurnOutSharp.FileType
{
    internal class Executable : IScannable
    {
        /// <summary>
        /// Cache for all IContentCheck types
        /// </summary>
        private static IEnumerable<Type> contentCheckClasses = null;

        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic)
        {
            // DOS MZ executable file format (and descendants)
            if (magic.StartsWith(new byte?[] { 0x4d, 0x5a }))
                return true;

            // Executable and Linkable Format
            if (magic.StartsWith(new byte?[] { 0x7f, 0x45, 0x4c, 0x46 }))
                return true;

            // Mach-O binary (32-bit)
            if (magic.StartsWith(new byte?[] { 0xfe, 0xed, 0xfa, 0xce }))
                return true;

            // Mach-O binary (32-bit, reverse byte ordering scheme)
            if (magic.StartsWith(new byte?[] { 0xce, 0xfa, 0xed, 0xfe }))
                return true;

            // Mach-O binary (64-bit)
            if (magic.StartsWith(new byte?[] { 0xfe, 0xed, 0xfa, 0xcf }))
                return true;

            // Mach-O binary (64-bit, reverse byte ordering scheme)
            if (magic.StartsWith(new byte?[] { 0xcf, 0xfa, 0xed, 0xfe }))
                return true;

            // Prefrred Executable File Format
            if (magic.StartsWith(new byte?[] { 0x4a, 0x6f, 0x79, 0x21, 0x70, 0x65, 0x66, 0x66 }))
                return true;

            return false;
        }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // Load the current file content
            byte[] fileContent = null;
            using (BinaryReader br = new BinaryReader(stream, Encoding.Default, true))
            {
                fileContent = br.ReadBytes((int)stream.Length);
            }

            // If we can, seek to the beginning of the stream
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            // Files can be protected in multiple ways
            var protections = new Dictionary<string, List<string>>();

            // Get all IContentCheck implementations
            if (contentCheckClasses == null)
            {
                contentCheckClasses = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(t => t.IsClass && t.GetInterface("IContentCheck") != null);
            }

            // Iterate through all content checks
            foreach (var contentCheckClass in contentCheckClasses)
            {
                IContentCheck contentCheck = Activator.CreateInstance(contentCheckClass) as IContentCheck;
                string protection = contentCheck.CheckContents(file, fileContent, scanner.IncludePosition);

                // If we have a valid content check based on settings
                if (!contentCheckClass.Namespace.ToLowerInvariant().Contains("packertype") || scanner.ScanPackers)
                {
                    if (!string.IsNullOrWhiteSpace(protection))
                        Utilities.AppendToDictionary(protections, file, protection);
                }

                // If we have an IScannable implementation
                if (contentCheckClass.GetInterface("IScannable") != null)
                {
                    IScannable scannable = Activator.CreateInstance(contentCheckClass) as IScannable;
                    if (file != null && !string.IsNullOrEmpty(protection))
                    {
                        var subProtections = scannable.Scan(scanner, null, file);
                        Utilities.PrependToKeys(subProtections, file);
                        Utilities.AppendToDictionary(protections, subProtections);
                    }
                }
            }

            return protections;
        }
    }
}
