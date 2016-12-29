using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.DocumentDBStudio.Properties;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.DocumentDBStudio.Helpers
{
    static class DocumentHelper
    {

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

        public static bool GetCustomDocumentDisplayIdentifier(List<dynamic> docs, out string custom)
        {
            custom = null;
            try
            {
                custom = Properties.Settings.Default.CustomDocumentDisplayIdentifier;
                if (!string.IsNullOrWhiteSpace(custom))
                {
                    var useCustom = false;
                    var firstDoc = docs.First();
                    try
                    {
                        var name = firstDoc.GetPropertyValue<string>(custom);
                        useCustom = true;
                    }
                    catch (Exception)
                    {
                    }
                    return useCustom;
                }
            }
            catch { }
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

        public static string GetDisplayText(dynamic doc)
        {
            string customDocumentDisplayIdentifier;
            var useCustom = GetCustomDocumentDisplayIdentifier(new List<dynamic> {doc}, out customDocumentDisplayIdentifier);
            return GetDisplayText(useCustom, doc, customDocumentDisplayIdentifier);
        }

        public static string GetDisplayText(bool useCustom, dynamic doc, string customDocumentDisplayIdentifier)
        {
            if (useCustom && !string.IsNullOrWhiteSpace(customDocumentDisplayIdentifier))
            {
                try
                {
                    var val = doc.GetPropertyValue<string>(customDocumentDisplayIdentifier);
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
