namespace Microsoft.Azure.DocumentDBStudio.CustomDocumentListDisplay.Models
{
    public class CustomDocumentListDisplay
    {
        public string DatabaseId { get; set; }
        public string DocumentCollectionId { get; set; }
        public string SortBy { get; set; }
        public string DisplayPattern { get; set; }
        public bool ReverseSort { get; set; }
    }
}
