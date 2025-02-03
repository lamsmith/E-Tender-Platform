namespace Backoffice_Services.Application.DTO.RFQManagement.Common
{
    public class RFQFilterModel
    {
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}