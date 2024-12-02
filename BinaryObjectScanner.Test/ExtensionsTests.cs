using System;
using System.Collections.Generic;
using Xunit;

namespace BinaryObjectScanner.Test
{
    public class ExtensionsTests
    {
        #region FileSize

        [Fact]
        public void FileSize_Null_Invalid()
        {
            string? filename = null;
            long actual = filename.FileSize();
            Assert.Equal(-1, actual);
        }

        [Fact]
        public void FileSize_Empty_Invalid()
        {
            string? filename = string.Empty;
            long actual = filename.FileSize();
            Assert.Equal(-1, actual);
        }

        [Fact]
        public void FileSize_Invalid_Invalid()
        {
            string? filename = "INVALID";
            long actual = filename.FileSize();
            Assert.Equal(-1, actual);
        }

        #endregion

        #region IterateWithAction

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

        #endregion

        #region OptionalContains

        [Fact]
        public void OptionalContains_NullStringNoComparison_False()
        {
            string? str = null;
            string prefix = "prefix";

            bool actual = str.OptionalContains(prefix);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalContains_NullStringComparison_False()
        {
            string? str = null;
            string prefix = "prefix";

            bool actual = str.OptionalContains(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalContains_EmptyStringNoComparison_False()
        {
            string? str = string.Empty;
            string prefix = "prefix";

            bool actual = str.OptionalContains(prefix);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalContains_EmptyStringComparison_False()
        {
            string? str = string.Empty;
            string prefix = "prefix";

            bool actual = str.OptionalContains(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalContains_NoMatchNoComparison_False()
        {
            string? str = "postfix";
            string prefix = "prefix";

            bool actual = str.OptionalContains(prefix);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalContains_NoMatchComparison_False()
        {
            string? str = "postfix";
            string prefix = "prefix";

            bool actual = str.OptionalContains(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalContains_MatchesNoComparison_True()
        {
            string? str = "prefix";
            string prefix = "prefix";

            bool actual = str.OptionalContains(prefix);
            Assert.True(actual);
        }

        [Fact]
        public void OptionalContains_MatchesComparison_True()
        {
            string? str = "prefix";
            string prefix = "prefix";

            bool actual = str.OptionalContains(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.True(actual);
        }

        #endregion

        #region OptionalEquals

        [Fact]
        public void OptionalEquals_NullStringNoComparison_False()
        {
            string? str = null;
            string prefix = "prefix";

            bool actual = str.OptionalEquals(prefix);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalEquals_NullStringComparison_False()
        {
            string? str = null;
            string prefix = "prefix";

            bool actual = str.OptionalEquals(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalEquals_EmptyStringNoComparison_False()
        {
            string? str = string.Empty;
            string prefix = "prefix";

            bool actual = str.OptionalEquals(prefix);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalEquals_EmptyStringComparison_False()
        {
            string? str = string.Empty;
            string prefix = "prefix";

            bool actual = str.OptionalEquals(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalEquals_NoMatchNoComparison_False()
        {
            string? str = "postfix";
            string prefix = "prefix";

            bool actual = str.OptionalEquals(prefix);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalEquals_NoMatchComparison_False()
        {
            string? str = "postfix";
            string prefix = "prefix";

            bool actual = str.OptionalEquals(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalEquals_MatchesNoComparison_True()
        {
            string? str = "prefix";
            string prefix = "prefix";

            bool actual = str.OptionalEquals(prefix);
            Assert.True(actual);
        }

        [Fact]
        public void OptionalEquals_MatchesComparison_True()
        {
            string? str = "prefix";
            string prefix = "prefix";

            bool actual = str.OptionalEquals(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.True(actual);
        }

        #endregion

        #region OptionalStartsWith

        [Fact]
        public void OptionalStartsWith_NullStringNoComparison_False()
        {
            string? str = null;
            string prefix = "prefix";

            bool actual = str.OptionalStartsWith(prefix);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalStartsWith_NullStringComparison_False()
        {
            string? str = null;
            string prefix = "prefix";

            bool actual = str.OptionalStartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalStartsWith_EmptyStringNoComparison_False()
        {
            string? str = string.Empty;
            string prefix = "prefix";

            bool actual = str.OptionalStartsWith(prefix);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalStartsWith_EmptyStringComparison_False()
        {
            string? str = string.Empty;
            string prefix = "prefix";

            bool actual = str.OptionalStartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalStartsWith_NoMatchNoComparison_False()
        {
            string? str = "postfix";
            string prefix = "prefix";

            bool actual = str.OptionalStartsWith(prefix);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalStartsWith_NoMatchComparison_False()
        {
            string? str = "postfix";
            string prefix = "prefix";

            bool actual = str.OptionalStartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.False(actual);
        }

        [Fact]
        public void OptionalStartsWith_MatchesNoComparison_True()
        {
            string? str = "prefix";
            string prefix = "prefix";

            bool actual = str.OptionalStartsWith(prefix);
            Assert.True(actual);
        }

        [Fact]
        public void OptionalStartsWith_MatchesComparison_True()
        {
            string? str = "prefix";
            string prefix = "prefix";

            bool actual = str.OptionalStartsWith(prefix, StringComparison.OrdinalIgnoreCase);
            Assert.True(actual);
        }

        #endregion
    }
}