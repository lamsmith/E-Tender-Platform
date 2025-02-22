﻿using RFQService.Domain.Enums;

namespace RFQService.Application.DTO.Responses
{

    public class RFQResponseModel
    {
        public Guid Id { get; set; }
        public string ContractTitle { get; set; }
        public string CompanyName { get; set; }
        public string ScopeOfSupply { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliveryTerms { get; set; }
        public string OtherInformation { get; set; }
        public DateTime Deadline { get; set; }
        public VisibilityType Visibility { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
       
    }

   
}
