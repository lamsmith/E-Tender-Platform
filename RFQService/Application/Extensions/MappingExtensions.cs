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
        #region Create Operations

        // Maps RFQCreationRequestModel (from API) to CreateRFQCommand (for CQRS)
        public static CreateRFQCommand ToCommand(this RFQCreationRequestModel request, Guid createdByUserId)
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
                CreatedByUserId = createdByUserId,
                RecipientEmails = request.RecipientEmails
            };
        }

        // Maps CreateRFQCommand (CQRS command) to RFQ (Domain Entity)
        public static RFQ ToRFQ(this CreateRFQCommand command)
        {
            var rfq = new RFQ
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
                Recipients = new List<RFQRecipient>()
            };

            // Add recipients with reference to the parent RFQ
            if (command.RecipientEmails?.Any() == true)
            {
                rfq.Recipients = command.RecipientEmails.Select(email => new RFQRecipient
                {
                    Id = Guid.NewGuid(),
                    RFQId = rfq.Id,
                    Email = email,
                    RFQ = rfq
                }).ToList();
            }

            return rfq;
        }

        #endregion

        #region Update Operations

        // Maps RFQUpdateRequestModel (from API) to UpdateRFQCommand (for CQRS)
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

        // Updates existing RFQ entity with data from UpdateRFQCommand
        public static void UpdateFromCommand(this RFQ rfq, UpdateRFQCommand command)
        {
            // Update basic properties
            rfq.ContractTitle = command.ContractTitle;
            rfq.CompanyName = command.CompanyName;
            rfq.ScopeOfSupply = command.ScopeOfSupply;
            rfq.PaymentTerms = command.PaymentTerms;
            rfq.DeliveryTerms = command.DeliveryTerms;
            rfq.OtherInformation = command.OtherInformation;
            rfq.Status = command.Status;
            rfq.Deadline = command.Deadline;
            rfq.Visibility = command.Visibility;

            // Create a new list of recipients
            var newRecipients = command.RecipientEmails.Select(email => new RFQRecipient
            {
                Id = Guid.NewGuid(),
                RFQId = rfq.Id,
                Email = email,
                RFQ = rfq  
            }).ToList();

            // Clear existing recipients and add new ones
            rfq.Recipients.Clear();
            foreach (var recipient in newRecipients)
            {
                rfq.Recipients.Add(recipient);
            }
        }

        #endregion

        #region Response Mappings

        // Maps RFQ domain entity to RFQResponseModel (API response)
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

        #endregion

        #region Pagination

        // Generic pagination mapping helper
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

        #endregion
    }
}
