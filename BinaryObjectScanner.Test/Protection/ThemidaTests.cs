﻿using System.IO;
using BinaryObjectScanner.Protection;
using Xunit;

namespace BinaryObjectScanner.Test.Protection
{
    public class ThemidaTests
    {
        [Fact]
        public void CheckPortableExecutableTest()
        {
            string file = "filename";
            SabreTools.Models.PortableExecutable.Executable model = new();
            Stream source = new MemoryStream();
            SabreTools.Serialization.Wrappers.PortableExecutable pex = new(model, source);

            var checker = new Themida();
            string? actual = checker.CheckExecutable(file, pex, includeDebug: false);
            Assert.Null(actual);
        }
    }
}