namespace StackTraceExplorer.Helpers
{
    using System;
    using System.IO;
    using System.Linq;

    public static class StringHelper
    {
        private static readonly char[] Separators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        public static string FindLongestMatchingSuffix(string searchString, string[] candidates, StringComparison comparisonType)
        {
            int nextSeparatorIndex = 0;
            int previousSeparatorIndex = -1;
            while (true)
            {
                string suffixToSearchFor;
                if (previousSeparatorIndex == -1)
                {
                    // Search for the whole string the first time
                    suffixToSearchFor = searchString;
                }
                else
                {
                    nextSeparatorIndex = searchString.IndexOfAny(Separators, previousSeparatorIndex);
                    if (nextSeparatorIndex == -1)
                    {
                        break;
                    }

                    suffixToSearchFor = searchString.Substring(nextSeparatorIndex);
                }

                string match = candidates.FirstOrDefault(s => s.EndsWith(suffixToSearchFor, comparisonType));
                if (match != null)
                {
                    return match;
                }

                previousSeparatorIndex = nextSeparatorIndex + 1;
            }

            throw new ArgumentException("None of the candidates match", nameof(candidates));
        }
    }
}
