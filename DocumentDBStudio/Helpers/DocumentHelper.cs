using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.DocumentDBStudio.Models;
using Microsoft.Azure.DocumentDBStudio.Properties;
using Microsoft.Azure.DocumentDBStudio.Providers;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.DocumentDBStudio.Helpers
{
    static class DocumentHelper
    {
        private static DocumentPatternParser _documentPatternParser = new DocumentPatternParser();
        public static string AssignNewIdToDocument(string json)
        {
            try
            {
                dynamic obj = JObject.Parse(json);
                obj.id = Guid.NewGuid();
                json = obj.ToString();
            }
            catch { }
            
            return json;
        }
        public static string RemoveInternalDocumentValues(string json)
        {
            if (Settings.Default.HideDocumentSystemProperties)
            {
                try
                {
                    dynamic obj = JObject.Parse(json);
                    var removeList = new List<string>();
                    foreach (var prop in obj)
                    {
                        if (prop.Name.ToString().StartsWith("_"))
                        {
                            removeList.Add(prop.Name);
                        }
                    }
                    foreach (var remove in removeList)
                    {
                        obj.Remove(remove);
                    }
                    json = obj.ToString();
                }
                catch { }
            }
            return json;
        }

        private static CustomDocumentListDisplay FindCustomDocumentListDisplay(string hostName, string databaseId, string documentCollectionId)
        {
            CustomDocumentListDisplay customListDisplay = null;
            var customDocumentListDisplayCollectionForHost = SettingsProvider.CustomDocumentListDisplayCollection.Find(s => s.HostName == hostName);

            if (customDocumentListDisplayCollectionForHost != null && customDocumentListDisplayCollectionForHost.Items.Count > 0)
            {
                customListDisplay = customDocumentListDisplayCollectionForHost.Items.Find(i => i.DocumentCollectionId == documentCollectionId && i.DatabaseId == databaseId);
            }
            return customListDisplay;
        }

        public static bool GetCustomDocumentDisplayIdentifier(List<dynamic> docs, string hostName, string documentCollectionId, string databaseId, out string customDisplayPattern)
        {

            var customListDisplay = FindCustomDocumentListDisplay(hostName, databaseId, documentCollectionId);
            customDisplayPattern = null;

            try
            {
                string testField;

                if (customListDisplay != null)
                {
                    customDisplayPattern = customListDisplay.DisplayPattern;
                    testField = customListDisplay.SortBy;
                }
                else
                {
                    customDisplayPattern = Settings.Default.CustomDocumentDisplayIdentifier;
                    testField = customDisplayPattern;
                }
                if (!string.IsNullOrWhiteSpace(testField))
                {
                    var useCustom = false;
                    var firstDoc = docs.First();
                    try
                    {
                        var name = firstDoc.GetPropertyValue<string>(testField);
                        useCustom = true;
                    }
                    catch (Exception)
                    {
                    }
                    return useCustom;
                }
            }
            catch (Exception ex)
            {
                var m = ex;
            }
            return false;
        }

        public static void SortDocuments(bool useCustom, List<dynamic> docs, string customDocumentDisplayIdentifier)
        {
            if (useCustom)
            {
                docs.Sort((first, second) =>
                        string.Compare(first.GetPropertyValue<string>(customDocumentDisplayIdentifier),
                            second.GetPropertyValue<string>(customDocumentDisplayIdentifier), StringComparison.Ordinal));
            }
            else
            {
                docs.Sort((first, second) => string.Compare(((Document)first).Id, ((Document)second).Id, StringComparison.Ordinal));
            }
        }

        public static string GetDisplayText(dynamic doc, string hostName, string documentCollectionId, string dbId)
        {
            string customDocumentDisplayIdentifier;
            var useCustom = GetCustomDocumentDisplayIdentifier(new List<dynamic> {doc}, hostName, documentCollectionId, dbId, out customDocumentDisplayIdentifier);
            return GetDisplayText(useCustom, doc, customDocumentDisplayIdentifier);
        }

        public static string GetDisplayText(bool useCustom, dynamic doc, string customDocumentDisplayIdentifier)
        {
            if (useCustom && !string.IsNullOrWhiteSpace(customDocumentDisplayIdentifier))
            {
                try
                {
                    string val;
                    if (customDocumentDisplayIdentifier.Contains("{") && customDocumentDisplayIdentifier.Contains("}"))
                    {
                        val = _documentPatternParser.ParsePattern(customDocumentDisplayIdentifier, doc);
                    }
                    else
                    {
                        val = doc.GetPropertyValue<string>(customDocumentDisplayIdentifier);
                    }
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        return string.Format("{0} [{1}]", val, doc.id);
                    }
                    return doc.id;
                }
                catch
                {
                }
            }
            return doc.id;
        }
    }
}
