using System.ComponentModel.DataAnnotations;

namespace HIMIS_API.Models.Payment
{
    public class WorkBillStatusDTO
    {
        [Key]
        public Int32? billno { get; set; }
        public string? work_id { get; set; }
        public string? agrbillstatus { get; set; }
        public string? MesurementDT { get; set; }
        public string? billdate { get; set; }

        public Decimal? GrossPaid { get; set; }
        public string? ChequeNo { get; set; }
        public string? ChequeDT { get; set; }
        
        public Int32? daysSinceMeasurement { get; set; }
        public string? BillStatus { get; set; }
        public string? billmbno { get; set; }
        public string? mbno { get; set; }
   
    }
}
