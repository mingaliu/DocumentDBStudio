using System.IO;
using System.Reflection;

namespace Microsoft.Azure.DocumentDBStudio.Providers
{
    public static class EmbeddedResourceProvider
    {
        public static string ReadEmbeddedResource(string resourceLocation)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();

                using (var stream = assembly.GetManifestResourceStream(resourceLocation))
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
            }
            catch { }
            return "";
        }

        public static void CopyStreamToFile(string resourceName, string fileLocation)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("Microsoft.Azure.DocumentDBStudio.Resources.{0}", resourceName)))
            {
                using (var fileStream = File.Create(fileLocation))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }
    }
}
