using System;
using System.IO;

namespace Microsoft.Azure.DocumentDBStudio.Providers
{
    public static class SystemInfoProvider
    {
        public static string LocalApplicationDataPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DocumentDBStudio");
            }
        }
    }
}
