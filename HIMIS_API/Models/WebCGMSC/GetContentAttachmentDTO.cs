namespace HIMIS_API.Models.WebCGMSC
{
    public class GetContentAttachmentDTO
    {
        public string? FileName { get; set; }

        public string? FilePath { get; set; }

        public string? Caption { get; set; }

        public string? DisplayNew { get; set; }
        public DateTime? EntryDT { get; set; }
        
    }
}
