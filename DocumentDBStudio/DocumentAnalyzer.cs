using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.DocumentDBStudio
{
    internal class DocumentAnalyzer
    {
        public static object ExtractPartitionKeyValue(Document document, PartitionKeyDefinition partitionKeyDefinition)
        {
            if (partitionKeyDefinition == null || partitionKeyDefinition.Paths.Count == 0)
            {
                return Undefined.Value;
            }

            return partitionKeyDefinition.Paths.Select(path =>
            {
                string[] parts = PathParser.GetPathParts(path);
                Debug.Assert(parts.Length >= 1, "Partition key component definition path is invalid.");

                return DocumentAnalyzer.GetValueByPath<object>(JToken.FromObject(document), parts, Undefined.Value);
            }).ToArray().FirstOrDefault();
        }

        /// <summary>
        /// Get the value associated with the specified property name.
        /// </summary>
        /// <param name="token">JSON tree.</param>
        /// <param name="fieldNames">Field names which compose a path to the property to be retrieved.</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetValueByPath<T>(JToken token, string[] fieldNames, T defaultValue)
        {
            if (fieldNames == null)
            {
                throw new ArgumentNullException("fieldNames");
            }

            foreach (string fieldName in fieldNames)
            {
                if (token == null)
                {
                    break;
                }

                token = token[fieldName];
            }

            if (token != null)
            {
                if (typeof(T).IsEnum)  //Enum Backward compatibility support.
                {
                    if (token.Type == JTokenType.String)
                    {
                        T result = (T)Enum.Parse(typeof(T), (string)token, true);
                        return result;
                    }
                }
                return token.ToObject<T>();
            }

            return defaultValue;
        }
    }

    internal sealed class PathParser
    {
        private static char segmentSeparator = '/';
        private static string errorMessageFormat = "Invalid path, failed at {0}";

        /// <summary>
        /// Extract parts from path
        /// </summary>
        /// <remarks>
        /// This code doesn't do as much validation as the backend, as it assumes that IndexingPolicy path coming from the backend is valid.
        /// </remarks>
        /// <param name="path">A path string</param>
        /// <returns>An array of parts of path</returns>
        public static string[] GetPathParts(string path)
        {
            List<string> tokens = new List<string>();
            int currentIndex = 0;

            while (currentIndex < path.Length)
            {
                if (path[currentIndex] != segmentSeparator)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, errorMessageFormat, currentIndex));
                }

                if (++currentIndex == path.Length) break;

                if (path[currentIndex] == '\"' || path[currentIndex] == '\'')
                {
                    tokens.Add(GetEscapedToken(path, ref currentIndex));
                }
                else
                {
                    tokens.Add(GetToken(path, ref currentIndex));
                }
            }

            return tokens.ToArray();
        }

        private static string GetEscapedToken(string path, ref int currentIndex)
        {
            char quote = path[currentIndex];
            int newIndex = ++currentIndex;

            while (true)
            {
                newIndex = path.IndexOf(quote, newIndex);
                if (newIndex == -1)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, errorMessageFormat, currentIndex));
                }

                if (path[newIndex - 1] != '\\')
                {
                    break;
                }

                ++newIndex;
            }

            string token = path.Substring(currentIndex, newIndex - currentIndex);
            currentIndex = newIndex + 1;
            return token;
        }

        private static string GetToken(string path, ref int currentIndex)
        {
            int newIndex = path.IndexOf(segmentSeparator, currentIndex);
            string token = null;
            if (newIndex == -1)
            {
                token = path.Substring(currentIndex);
                currentIndex = path.Length;
            }
            else
            {
                token = path.Substring(currentIndex, newIndex - currentIndex);
                currentIndex = newIndex;
            }

            token = token.Trim();
            return token;
        }
    }

}
