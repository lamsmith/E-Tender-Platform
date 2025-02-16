using System;

namespace SharedLibrary.Models.Messages.BidEvents
{
    public class BidStatusUpdatedMessage
    {
        public Guid BidId { get; set; }
        public string NewStatus { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}