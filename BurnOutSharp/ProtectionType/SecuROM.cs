using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class SecuROM : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                // AddD + (char)0x03 + (char)0x00 + (char)0x00 + (char)0x00)
                new Matcher(new byte?[] { 0x41, 0x64, 0x64, 0x44, 0x03, 0x00, 0x00, 0x00 }, GetV4Version, "SecuROM"),

                // (char)0xCA + (char)0xDD + (char)0xDD + (char)0xAC + (char)0x03
                new Matcher(new byte?[] { 0xCA, 0xDD, 0xDD, 0xAC, 0x03 }, GetV5Version, "SecuROM"),

                // .securom + (char)0xE0 + (char)0xC0
                new Matcher(new byte?[]
                {
                    0x2E, 0x73, 0x65, 0x63, 0x75, 0x72, 0x6F, 0x6D,
                    0xE0, 0xC0
                }, GetV7Version, "SecuROM"),

                // .securom
                new Matcher(new byte?[] { 0x2E, 0x73, 0x65, 0x63, 0x75, 0x72, 0x6F, 0x6D }, GetV7Version, "SecuROM"),

                // _and_play.dll + (char)0x00 + drm_pagui_doit
                new Matcher(new byte?[]
                {
                    0x5F, 0x61, 0x6E, 0x64, 0x5F, 0x70, 0x6C, 0x61,
                    0x79, 0x2E, 0x64, 0x6C, 0x6C, 0x00, 0x64, 0x72,
                    0x6D, 0x5F, 0x70, 0x61, 0x67, 0x75, 0x69, 0x5F,
                    0x64, 0x6F, 0x69, 0x74
                }, Utilities.GetFileVersion, "SecuROM Product Activation"),

                // .cms_t + (char)0x00
                new Matcher(new byte?[] { 0x2E, 0x63, 0x6D, 0x73, 0x5F, 0x74, 0x00 }, "SecuROM 1-3"),

                // .cms_d + (char)0x00
                new Matcher(new byte?[] { 0x2E, 0x63, 0x6D, 0x73, 0x5F, 0x64, 0x00 }, "SecuROM 1-3"),
            };

            return Utilities.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("CMS16.DLL", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("CMS_95.DLL", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("CMS_NT.DLL", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("CMS32_95.DLL", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("CMS32_NT.DLL", StringComparison.OrdinalIgnoreCase)))
            {
                return "SecuROM";
            }
            else if (files.Any(f => Path.GetFileName(f).Equals("SINTF32.DLL", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("SINTF16.DLL", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("SINTFNT.DLL", StringComparison.OrdinalIgnoreCase)))
            {
                return "SecuROM New";
            }
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("CMS16.DLL", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("CMS_95.DLL", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("CMS_NT.DLL", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("CMS32_95.DLL", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("CMS32_NT.DLL", StringComparison.OrdinalIgnoreCase))
            {
                return "SecuROM";
            }
            else if (Path.GetFileName(path).Equals("SINTF32.DLL", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("SINTF16.DLL", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("SINTFNT.DLL", StringComparison.OrdinalIgnoreCase))
            {
                return "SecuROM New";
            }

            return null;
        }

        public static string GetV4Version(string file, byte[] fileContent, int position)
        {
            int index = position + 8; // Begin reading after "AddD"
            char version = (char)fileContent[index];
            index += 2;

            string subVersion = Encoding.ASCII.GetString(fileContent, index, 2);
            index += 3;

            string subSubVersion = Encoding.ASCII.GetString(fileContent, index, 2);
            index += 3;

            string subSubSubVersion = Encoding.ASCII.GetString(fileContent, index, 4);

            if (!char.IsNumber(version))
                return "(very old, v3 or less)";

            return $"{version}.{subVersion}.{subSubVersion}.{subSubSubVersion}";
        }

        public static string GetV5Version(string file, byte[] fileContent, int position)
        {
            int index = position + 8; // Begin reading after "ÊÝÝ¬"
            byte version = (byte)(fileContent[index] & 0x0F);
            index += 2;

            byte[] subVersion = new byte[2];
            subVersion[0] = (byte)(fileContent[index] ^ 36);
            index++;
            subVersion[1] = (byte)(fileContent[index] ^ 28);
            index += 2;

            byte[] subSubVersion = new byte[2];
            subSubVersion[0] = (byte)(fileContent[index] ^ 42);
            index++;
            subSubVersion[0] = (byte)(fileContent[index] ^ 8);
            index += 2;

            byte[] subSubSubVersion = new byte[4];
            subSubSubVersion[0] = (byte)(fileContent[index] ^ 16);
            index++;
            subSubSubVersion[1] = (byte)(fileContent[index] ^ 116);
            index++;
            subSubSubVersion[2] = (byte)(fileContent[index] ^ 34);
            index++;
            subSubSubVersion[3] = (byte)(fileContent[index] ^ 22);

            if (version == 0 || version > 9)
                return "";

            return $"{version}.{subVersion[0]}{subVersion[1]}.{subSubVersion[0]}{subSubVersion[1]}.{subSubSubVersion[0]}{subSubSubVersion[1]}{subSubSubVersion[2]}{subSubSubVersion[3]}";
        }

        public static string GetV7Version(string file, byte[] fileContent, int position)
        {
            int index = 236;
            byte[] bytes = new ReadOnlySpan<byte>(fileContent, index, 4).ToArray();
            
            //SecuROM 7 new and 8
            if (bytes[3] == 0x5C) // if (bytes[0] == 0xED && bytes[3] == 0x5C {
            {
                return $"{bytes[0] ^ 0xEA}.{bytes[1] ^ 0x2C:00}.{bytes[2] ^ 0x8:0000}";
            }

            // SecuROM 7 old
            else
            {
                index = 122;
                bytes = new ReadOnlySpan<byte>(fileContent, index, 2).ToArray();
                return $"7.{bytes[0] ^ 0x10:00}.{bytes[1] ^ 0x10:0000}"; //return "7.01-7.10"
            }
        }
    }
}
