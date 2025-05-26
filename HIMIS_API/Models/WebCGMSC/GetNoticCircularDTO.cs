namespace HIMIS_API.Models.WebCGMSC
{
    public class GetNoticCircularDTO
    {
        public string? CountATT { get; set; }
        public string? Expr1 { get; set; }
        public string? Content_Registration_Id { get; set; }
        public string? Content_Subject { get; set; }
        public string? DisplayNew { get; set; }
        public string? Department { get; set; }
        public string? Content_Discription { get; set; }
        public string? Content_Publising_Date { get; set; } // Stored as formatted string (dd/MM/yyyy)
        public string? Expiry_DateOnNotice_Board { get; set; } // Stored as formatted string (dd/MM/yyyy)
        public string? Expiry_DateOnDepartment_Board { get; set; }
        public string? Status { get; set; }
        public DateTime? Date_TimeStamp { get; set; }
        public string? Ip { get; set; }
        public string? SystemInfo { get; set; }
        public string? CreaterUserName { get; set; }
        public string? ContentCategoryCode { get; set; }
        public string? ContentCategoryName { get; set; }
    }
}
