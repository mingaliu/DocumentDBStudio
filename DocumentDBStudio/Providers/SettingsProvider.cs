using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Azure.DocumentDBStudio.CustomDocumentListDisplay.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.DocumentDBStudio.Providers
{
    public static class SettingsProvider
    {
        private static readonly object SettingsLock = new object();

        private static string CustomDocumentListDisplayFileName
        {
            get
            {
                return Path.Combine(SystemInfoProvider.LocalApplicationDataPath, "Settings.CustomDocumentListDisplay.json");
            }
        }
        public static void SaveCustomDocumentListDisplayCollectionToFile()
        {
            _customDocumentListDisplayCollections = _customDocumentListDisplayCollections.Where(cdc => cdc.Items != null && cdc.Items.Count > 0).ToList();
            File.WriteAllText(CustomDocumentListDisplayFileName, JsonConvert.SerializeObject(_customDocumentListDisplayCollections, Formatting.Indented));
        }

        private static List<CustomDocumentListDisplayCollection> _customDocumentListDisplayCollections = null;
        public static List<CustomDocumentListDisplayCollection> CustomDocumentListDisplayCollection
        {
            get
            {
                if (_customDocumentListDisplayCollections == null)
                {
                    ReadCustomDocumentListDisplayCollectionsFromFile();
                }
                return _customDocumentListDisplayCollections;
            }
            set
            {
                _customDocumentListDisplayCollections = value;
            }
        }

        public static void ReadCustomDocumentListDisplayCollectionsFromFile()
        {
            if (Monitor.TryEnter(SettingsLock, TimeSpan.FromSeconds(5)))
            {
                try
                {
                    if (!Directory.Exists(SystemInfoProvider.LocalApplicationDataPath))
                    {
                        Directory.CreateDirectory(SystemInfoProvider.LocalApplicationDataPath);
                    }

                    if (!File.Exists(CustomDocumentListDisplayFileName))
                    {
                        _customDocumentListDisplayCollections = new List<CustomDocumentListDisplayCollection>();
                        SaveCustomDocumentListDisplayCollectionToFile();
                    }
                    else
                    {
                        try
                        {
                            var contents = File.ReadAllText(CustomDocumentListDisplayFileName);
                            _customDocumentListDisplayCollections =
                                JsonConvert.DeserializeObject<List<CustomDocumentListDisplayCollection>>(contents);
                        }
                        catch
                        {
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(SettingsLock);
                }
            }
        }
    }
}
