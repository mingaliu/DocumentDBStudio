using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Azure.DocumentDBStudio.Models;
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
        private static void SaveCustomDocumentListDisplayCollectionToFile()
        {
            File.WriteAllText(CustomDocumentListDisplayFileName, JsonConvert.SerializeObject(_customDocumentListDisplayCollections, Formatting.Indented));
        }

        private static List<CustomDocumentListDisplayCollection> _customDocumentListDisplayCollections = null;
        public static List<CustomDocumentListDisplayCollection> CustomDocumentListDisplayCollection
        {
            get
            {
                if (_customDocumentListDisplayCollections == null)
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
                                    _customDocumentListDisplayCollections = JsonConvert.DeserializeObject<List<CustomDocumentListDisplayCollection>>(contents);
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
                return _customDocumentListDisplayCollections;
            }
        }
    }
}
