using System.Collections.Generic;

namespace Microsoft.Azure.DocumentDBStudio.Models
{
    public class CustomDocumentListDisplayCollection
    {
        public string HostName { get; set; }
        public List<CustomDocumentListDisplay> Items { get; set; }
    }
}
