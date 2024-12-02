using System;
using System.Collections.Generic;
using Xunit;

namespace BinaryObjectScanner.Tests
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void IterateWithAction_EmptyEnumerable_Success()
        {
            List<string> set = new List<string>();
            Action<string> action = (s) => s.ToLowerInvariant();

            set.IterateWithAction(action);
            Assert.Empty(set);
        }

        [Fact]
        public void IterateWithAction_EmptyAction_Success()
        {
            List<string> set = ["a", "b", "c"];
            Action<string> action = (s) => { };

            set.IterateWithAction(action);
            Assert.Equal(3, set.Count);
        }

        [Fact]
        public void IterateWithAction_Valid_Success()
        {
            List<string> set = ["a", "b", "c"];
            List<string> actual = new List<string>();

            Action<string> action = (s) =>
            {
                lock (actual)
                {
                    actual.Add(s.ToUpperInvariant());
                }
            };

            set.IterateWithAction(action);
            Assert.Equal(3, set.Count);
            Assert.Equal(3, actual.Count);
        }
    }
}