using RFQService.Application.DTO.Requests;
using RFQService.Application.DTO.Responses;
using RFQService.Domain.Entities;
using RFQService.Domain.Paging;
using RFQService.Application.Features.Commands;
using RFQService.Domain.Enums;

namespace RFQService.Application.Extensions
{
    public static class MappingExtensions
    {
        public static PaginatedList<T> ToPaginated<T, TB>(this PaginatedList<TB> list, IEnumerable<T> items)
            where T : notnull where TB : notnull
        {
            return new PaginatedList<T>
            {
                Items = items,
                Page = list.Page,
                PageSize = list.PageSize,
                TotalItems = list.TotalItems
            };
        }

        // Mapping from RFQCreationRequestModel to RFQ
        public static RFQ ToRFQ(this RFQCreationRequestModel request)
        {
            return new RFQ
            {
                ContractTitle = request.ContractTitle,
                ScopeOfSupply = request.ScopeOfSupply,
                PaymentTerms = request.PaymentTerms,
                DeliveryTerms = request.DeliveryTerms,
                OtherInformation = request.OtherInformation,
                Deadline = request.Deadline,
                Visibility = request.Visibility,
                CreatedByUserId = request.CreatedByUserId,
                CreatedAt = DateTime.UtcNow,

            };
        }

        // from RFQ to RFQResponseModel
        public static RFQResponseModel ToRFQResponse(this RFQ rfq)
        {
            return new RFQResponseModel
            {
                Id = rfq.Id,
                ContractTitle = rfq.ContractTitle,
                ScopeOfSupply = rfq.ScopeOfSupply,
                PaymentTerms = rfq.PaymentTerms,
                DeliveryTerms = rfq.DeliveryTerms,
                OtherInformation = rfq.OtherInformation,
                Deadline = rfq.Deadline,
                Visibility = rfq.Visibility,
                CreatedByUserId = rfq.CreatedByUserId,
                CreatedAt = rfq.CreatedAt,

            };
        }

        // Mapping from RFQCreationRequestModel to CreateRFQCommand
        public static CreateRFQCommand ToCommand(this RFQCreationRequestModel request)
        {
            return new CreateRFQCommand
            {
                ContractTitle = request.ContractTitle,
                CompanyName = request.CompanyName,
                ScopeOfSupply = request.ScopeOfSupply,
                PaymentTerms = request.PaymentTerms,
                DeliveryTerms = request.DeliveryTerms,
                OtherInformation = request.OtherInformation,
                Status = Status.Open,
                Deadline = request.Deadline,
                Visibility = request.Visibility,
                RecipientEmails = request.RecipientEmails ?? new List<string>()
            };
        }

        // Mapping from CreateRFQCommand to RFQ
        public static RFQ ToRFQ(this CreateRFQCommand command)
        {
            return new RFQ
            {
                Id = Guid.NewGuid(),
                ContractTitle = command.ContractTitle,
                CompanyName = command.CompanyName,
                ScopeOfSupply = command.ScopeOfSupply,
                PaymentTerms = command.PaymentTerms,
                DeliveryTerms = command.DeliveryTerms,
                OtherInformation = command.OtherInformation,
                Status = command.Status,
                Deadline = command.Deadline,
                Visibility = command.Visibility,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = command.CreatedByUserId,
                Recipients = command.RecipientEmails.Select(email => new RFQRecipient
                {
                    Email = email
                }).ToList()
            };
        }

        // Mapping from RFQUpdateRequestModel to UpdateRFQCommand
        public static UpdateRFQCommand ToCommand(this RFQUpdateRequestModel request, Guid id)
        {
            return new UpdateRFQCommand
            {
                Id = id,
                ContractTitle = request.ContractTitle,
                CompanyName = request.CompanyName,
                ScopeOfSupply = request.ScopeOfSupply,
                PaymentTerms = request.PaymentTerms,
                DeliveryTerms = request.DeliveryTerms,
                OtherInformation = request.OtherInformation,
                Status = request.Status,
                Deadline = request.Deadline,
                Visibility = request.Visibility,
                RecipientEmails = request.RecipientEmails
            };
        }

        // Mapping from UpdateRFQCommand to RFQ for updates
        public static void UpdateFromCommand(this RFQ rfq, UpdateRFQCommand command)
        {
            rfq.ContractTitle = command.ContractTitle;
            rfq.CompanyName = command.CompanyName;
            rfq.ScopeOfSupply = command.ScopeOfSupply;
            rfq.PaymentTerms = command.PaymentTerms;
            rfq.DeliveryTerms = command.DeliveryTerms;
            rfq.OtherInformation = command.OtherInformation;
            rfq.Status = command.Status;
            rfq.Deadline = command.Deadline;
            rfq.Visibility = command.Visibility;

            // Update recipients
            rfq.Recipients.Clear();
            rfq.Recipients = command.RecipientEmails.Select(email => new RFQRecipient
            {
                RFQId = rfq.Id,
                Email = email
            }).ToList();
        }
    }
}
