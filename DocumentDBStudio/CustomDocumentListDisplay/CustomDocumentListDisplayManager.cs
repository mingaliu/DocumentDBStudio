using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.DocumentDBStudio.CustomDocumentListDisplay.Models;
using Microsoft.Azure.DocumentDBStudio.Helpers;
using Microsoft.Azure.DocumentDBStudio.Properties;
using Microsoft.Azure.DocumentDBStudio.Providers;

namespace Microsoft.Azure.DocumentDBStudio.CustomDocumentListDisplay
{
    public class CustomDocumentListDisplayManager
    {
        private readonly DocumentPatternParser _documentPatternParser = new DocumentPatternParser();
        public Models.CustomDocumentListDisplay FindCustomDocumentListDisplay(string hostName, string databaseId, string documentCollectionId)
        {
            Models.CustomDocumentListDisplay customListDisplay = null;
            var customDocumentListDisplayCollectionForHost = SettingsProvider.CustomDocumentListDisplayCollection.Find(s => s.HostName == hostName);

            if (customDocumentListDisplayCollectionForHost != null && customDocumentListDisplayCollectionForHost.Items.Count > 0)
            {
                customListDisplay = customDocumentListDisplayCollectionForHost.Items.Find(i => i.DocumentCollectionId == documentCollectionId && i.DatabaseId == databaseId);
            }
            return customListDisplay;
        }

        public void SaveCustomDocumentListDisplay(string hostName, Models.CustomDocumentListDisplay cdld)
        {
            // Re-read from file to make sure we have latest version:
            SettingsProvider.ReadCustomDocumentListDisplayCollectionsFromFile();
            var existing = FindCustomDocumentListDisplay(hostName, cdld.DatabaseId, cdld.DocumentCollectionId);
            if (existing != null)
            {
                SettingsProvider.CustomDocumentListDisplayCollection.Find(s => s.HostName == hostName)
                    .Items.Find(i => i.DocumentCollectionId == cdld.DocumentCollectionId)
                    .DisplayPattern = cdld.DisplayPattern;

                SettingsProvider.CustomDocumentListDisplayCollection.Find(s => s.HostName == hostName)
                    .Items.Find(i => i.DocumentCollectionId == cdld.DocumentCollectionId)
                    .SortBy = cdld.SortBy;

                SettingsProvider.CustomDocumentListDisplayCollection.Find(s => s.HostName == hostName)
                    .Items.Find(i => i.DocumentCollectionId == cdld.DocumentCollectionId)
                    .ReverseSort = cdld.ReverseSort;

                SettingsProvider.SaveCustomDocumentListDisplayCollectionToFile();
            }
            else
            {
                var hostNode = SettingsProvider.CustomDocumentListDisplayCollection.Find(s => s.HostName == hostName);
                if (hostNode != null)
                {
                    SettingsProvider.CustomDocumentListDisplayCollection.Find(s => s.HostName == hostName).Items.Add(cdld);
                    SettingsProvider.SaveCustomDocumentListDisplayCollectionToFile();
                }
                else
                {
                    SettingsProvider.CustomDocumentListDisplayCollection.Add(new CustomDocumentListDisplayCollection
                    {
                        HostName = hostName,
                        Items = new List<Models.CustomDocumentListDisplay> { cdld }
                    });
                    SettingsProvider.SaveCustomDocumentListDisplayCollectionToFile();
                }
            }

        }

        public void RemoveCustomDocumentListDisplay(string hostName, string dbId, string dbColl)
        {
            // Re-read from file to make sure we have latest version:
            SettingsProvider.ReadCustomDocumentListDisplayCollectionsFromFile();
            var existing = FindCustomDocumentListDisplay(hostName, dbId, dbColl);
            if (existing != null)
            {
                SettingsProvider.CustomDocumentListDisplayCollection.Find(s => s.HostName == hostName).Items.Remove(existing);
                SettingsProvider.SaveCustomDocumentListDisplayCollectionToFile();
            }
        }

        public bool GetCustomDocumentDisplayIdentifier(
            List<dynamic> docs, string hostName, string documentCollectionId, string databaseId, out string customDisplayPattern, out string sortField, out bool reverseSort
        )
        {
            var customListDisplay = FindCustomDocumentListDisplay(hostName, databaseId, documentCollectionId);
            reverseSort = customListDisplay != null && customListDisplay.ReverseSort;
            return GetCustomDocumentDisplayIdentifier(docs, customListDisplay, Settings.Default.CustomDocumentDisplayIdentifier, out customDisplayPattern, out sortField);
        }

        public bool GetCustomDocumentDisplayIdentifier(
            List<dynamic> docs,
            Models.CustomDocumentListDisplay customListDisplay,
            string customDocumentDisplayIdentifier,
            out string customDisplayPattern,
            out string sortField,
            bool useDirectAccess = false)
        {
            customDisplayPattern = null;
            sortField = "id";

            try
            {
                string testField;

                if (customListDisplay != null)
                {
                    customDisplayPattern = customListDisplay.DisplayPattern;
                    testField = customListDisplay.SortBy;
                    sortField = customListDisplay.SortBy;
                }
                else
                {
                    customDisplayPattern = customDocumentDisplayIdentifier;
                    testField = customDisplayPattern;
                    sortField = customDisplayPattern;
                }
                if (!string.IsNullOrWhiteSpace(testField))
                {
                    var useCustom = false;
                    var firstDoc = docs.First();

                    try
                    {
                        var name = DynamicHelper.GetPropertyValue(firstDoc, testField, useDirectAccess);
                        useCustom = true;
                    }
                    catch (Exception) { }

                    return useCustom;
                }
            }
            catch (Exception ex)
            {
                var m = ex;
            }
            return false;
        }

        public string GetDisplayText(dynamic doc, string hostName, string documentCollectionId, string dbId)
        {
            string customDocumentDisplayIdentifier;
            string sortField;
            bool reverseSort;
            var useCustom = GetCustomDocumentDisplayIdentifier(new List<dynamic> { doc }, hostName, documentCollectionId, dbId, out customDocumentDisplayIdentifier, out sortField, out reverseSort);
            return GetDisplayText(useCustom, doc, customDocumentDisplayIdentifier);
        }

        public string GetDisplayText(bool useCustom, dynamic doc, string customDocumentDisplayIdentifier, bool useDirectAccess = false)
        {
            if (useCustom && !string.IsNullOrWhiteSpace(customDocumentDisplayIdentifier))
            {
                try
                {
                    string val;
                    if (customDocumentDisplayIdentifier.Contains("{") && customDocumentDisplayIdentifier.Contains("}"))
                    {
                        val = _documentPatternParser.ParsePattern(customDocumentDisplayIdentifier, doc, useDirectAccess);
                    }
                    else
                    {
                        val = DynamicHelper.GetPropertyValue(doc, customDocumentDisplayIdentifier, useDirectAccess);
                        if (customDocumentDisplayIdentifier != "id")
                        {
                            val += string.Format(" [{0}]", DynamicHelper.GetPropertyValue(doc, "id", useDirectAccess));
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        return val;
                    }
                    return doc.id;
                }
                catch (Exception)
                {
                }
            }
            return doc.id;
        }
    }
}
