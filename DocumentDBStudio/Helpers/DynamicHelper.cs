using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.DocumentDBStudio.Helpers
{
    public static class DynamicHelper
    {
        public static string GetPropertyValue(dynamic property, string valueKey, bool useDirectAccess = false)
        {
            if (!useDirectAccess)
            {
                try
                {
                    return property.GetPropertyValue<string>(valueKey);
                }
                catch (Exception)
                {
                }
            }
            return property[valueKey];
        }

        public static List<string> GetPropertyKeysForDynamic(dynamic dynamicToGetPropertiesFor)
        {
            JObject attributesAsJObject = dynamicToGetPropertiesFor;
            var values = attributesAsJObject.ToObject<Dictionary<string, object>>();
            var finalKeyList = new List<string>();
            foreach (var v in values.Where(x => x.Value != null))
            {
                try
                {
                    var type = v.Value.GetType();
                    if (type != typeof(JArray) && type != typeof(JObject))
                    {
                        finalKeyList.Add(v.Key);
                    }
                }
                catch (Exception)
                {
                }

            }
            return finalKeyList;
        }
    }
}
