using System;
using System.Collections.Generic;
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

        public static void SortDocuments(bool useCustom, List<dynamic> docs, string sortField, bool reverseSort)
        {
            if (useCustom)
            {
                if (reverseSort)
                {
                    docs.Sort( (second, first) => string.Compare(first.GetPropertyValue<string>(sortField), second.GetPropertyValue<string>(sortField), StringComparison.Ordinal));
                }
                else
                {
                    docs.Sort( (first, second) => string.Compare(first.GetPropertyValue<string>(sortField), second.GetPropertyValue<string>(sortField), StringComparison.Ordinal));
                }

            }
            else
            {
                docs.Sort((first, second) => string.Compare(((Document)first).Id, ((Document)second).Id, StringComparison.Ordinal));
            }
        }
    }
}
