using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace HIMIS_API.Models.Tender
{
    public class TobeTenderDetailsASDTO
    {

        [Key]
        public string? work_id { get; set; }
        public string? letterno { get; set; }
        public string? District { get; set; }
        public string? Head { get; set; }
        public string? Division { get; set; }
        public string? DETAILS_ENG { get; set; }
        public string? workname { get; set; }
        public string? Block_Name_En { get; set; }
        public Decimal? ValueWorks { get; set; }
        public string? ASDate { get; set; }
        public string? parentprogress { get; set; }
        public string? DashName { get; set; }
        public string? GroupName { get; set; }
        public Int32? ppid { get; set; }

        public string? wocancelletterno { get; set; }
        public string? PDate { get; set; }
        public string? WOCancelProposalLetterNo { get; set; }


    }
}
