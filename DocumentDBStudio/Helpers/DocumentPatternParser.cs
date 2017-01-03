using System.Text.RegularExpressions;

namespace Microsoft.Azure.DocumentDBStudio.Helpers
{
    public class DocumentPatternParser
    {
        private const string VariableRegexPattern = @"{\w+}";

        public string ParsePattern(string pattern, dynamic document, bool useDirectAccess = false)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return pattern;
            var returnData = pattern;
            var variableMatches = Regex.Matches(pattern, VariableRegexPattern);

            returnData = ParsePatternMatches(variableMatches, returnData, document, useDirectAccess);

            variableMatches = Regex.Matches(returnData, VariableRegexPattern);

            return variableMatches.Count > 0
                ? ParsePattern(returnData, document)
                : returnData;
        }

        private string ParsePatternMatches(MatchCollection variableMatches, string returnData, dynamic document, bool useDirectAccess = false)
        {
            foreach (Match match in variableMatches)
            {
                var matchedValue = match.Value;
                var expression = GetPartFromMatch(matchedValue);
                returnData = returnData.Replace(matchedValue, DynamicHelper.GetPropertyValue(document, expression, useDirectAccess));
            }
            return returnData;
        }

        private static string GetPartFromMatch(string matchedValue)
        {
            return matchedValue.Substring(1, matchedValue.Length - 2).Trim();
        }
    }
}
