using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xarivu.Coder.Utilities
{
    public static class StringUtilities
    {
        /// <summary>
        /// Make sure all new-lines in string are represented as "\r\n".
        /// Will fix cases where '\n' is used without '\r' for new-line.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string UpdateNewlines(string s)
        {
            if (s == null) return null;

            var t = s.Replace("\r\n", "\n");
            return s.Replace("\n", "\r\n");
        }

        /// <summary>
        /// Remove all whitespace characters from a string, including tabs and new-line (carriage return, line feed, etc).
        /// See https://msdn.microsoft.com/en-us/library/system.char.iswhitespace(v=vs.110).aspx
        /// </summary>
        /// <param name="s"></param>
        /// <param name="replaceWith">If specified, then each whitespace character is replaced with this value.
        /// Note if there are consecutive whitespace characters, then all of them are replaced with a single replaceWith value.</param>
        /// <returns></returns>
        public static string RemoveWhitespace(string s, string replaceWith = null)
        {
            if (s == null) return null;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; ++i)
            {
                if (char.IsWhiteSpace(s[i]))
                {
                    // Increment i to index after all consecutive whitespace characters.
                    do
                    {
                        ++i;
                    }
                    while (i < s.Length && char.IsWhiteSpace(s[i]));

                    if (replaceWith != null)
                    {
                        sb.Append(replaceWith);
                    }
                }
                else
                {
                    sb.Append(s[i]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Remove all characters except unicode letter, digit, and underscore '_'.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveExceptLetterDigitUnderscore(string s)
        {
            if (s == null) return null;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; ++i)
            {
                if (char.IsLetterOrDigit(s[i]) || s[i] == '_')
                {
                    sb.Append(s[i]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a pascal-case string to camel-case.
        /// The method has a very basic implemenation which converts the first character to lower case.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToCamelCase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            string firstCharStr = name.Substring(0, 1).ToLower();
            string remainingStr = name.Substring(1);
            return firstCharStr + remainingStr;
        }

        public static void AddEmptyLineIfNeeded(StringBuilder sb)
        {
            var len = sb.Length;

            if (len == 0) return;

            // Check if the previous characters are curly start brace followed by new line characters.
            // In that case don't add newline.
            var checkStr = $"{{{Environment.NewLine}";
            var checkAt = len - checkStr.Length;
            if (checkAt >= 0)
            {
                bool match = true;
                for (int i = checkAt, j = 0; i < len; ++i, ++j)
                {
                    if (sb[i] != checkStr[j])
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return;
                }
            }

            sb.AppendLine();
        }

        public static void ForEachAppendLine<T>(
            StringBuilder sb,
            IEnumerable<T> items,
            int tabs,
            Func<T, string> createLineFunc,
            bool skipTabsOnFirst,
            bool addComma,
            bool skipLastNewLine)
        {
            if (!ListUtilities.HasElements(items)) return;

            var list = items.ToList();
            var count = list.Count();
            for (int i = 0; i < count; ++i)
            {
                var isFirstLine = i == 0;
                var isLastLine = i == count - 1;
                var item = list[i];

                string line = createLineFunc(item);

                if (addComma && !isLastLine)
                {
                    line += ",";
                }

                var lineTabs = isFirstLine && skipTabsOnFirst ? 0 : tabs;
                AppendTabs(sb, lineTabs);

                var appendNewLine = !isLastLine || !skipLastNewLine;
                if (appendNewLine)
                {
                    sb.AppendLine(line);
                }
                else
                {
                    sb.Append(line);
                }
            }
        }

        public static void AppendTabs(StringBuilder sb, int tabs)
        {
            for (int t = 0; t < tabs; ++t)
            {
                sb.Append("    ");
            }
        }

        public static void AppendLine(StringBuilder sb, int tabs, string str)
        {
            var lines = str.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; ++i)
            {
                var line = lines[i];
                AppendTabs(sb, tabs);

                sb.AppendLine(line);
            }
        }

        public static int FindNonSpace(string str, int start)
        {
            int i = start;
            while (i < str.Length && char.IsWhiteSpace(str[i]))
            {
                ++i;
            }

            if (i == str.Length) return -1;

            return i;
        }

        public static int FindSpace(string str, int start)
        {
            int i = start;
            while (i < str.Length && !char.IsWhiteSpace(str[i]))
            {
                ++i;
            }

            if (i == str.Length) return -1;

            return i;
        }

        /// <summary>
        /// Expect a string with optional whitespace before it.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start"></param>
        /// <param name="expect"></param>
        /// <returns>If the expected string is found, the index after the expected string. Otherwise -1.</returns>
        public static int Expect(string str, int start, string expect)
        {
            int i = FindNonSpace(str, start);
            if (i == -1) return -1;
            for (int j = 0; j < expect.Length; ++j)
            {
                if (i == str.Length) return -1;

                if (str[i] != expect[j]) return -1;
                ++i;
            }

            return i;
        }

        public static int ExpectSequence(string str, int start, string[] expectedStrings)
        {
            int i = start;

            foreach (var expect in expectedStrings)
            {
                i = Expect(str, i, expect);
                if (i == -1) return -1;
            }

            return i;
        }

        public static int GetNextWord(string str, int start, out string word)
        {
            word = null;

            int wordStart = FindNonSpace(str, start);
            if (wordStart == -1) return -1;

            int wordEnd = FindSpace(str, wordStart);
            if (wordEnd == -1)
            {
                wordEnd = str.Length;
            }

            word = str.Substring(wordStart, wordEnd - wordStart);

            return wordEnd;
        }
    }
}
