using System.Collections.Generic;
using Microsoft.Azure.DocumentDBStudio.Properties;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.DocumentDBStudio.Helpers
{
    static class DocumentHelper
    {
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
    }
}
