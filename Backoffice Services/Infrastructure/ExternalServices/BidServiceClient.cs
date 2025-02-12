using System.Net.Http.Json;
using Backoffice_Services.Application.DTO.BidManagement.Common;
using Backoffice_Services.Application.DTO.BidManagement.Responses;
using Backoffice_Services.Application.Features.BidManagement.Commands;
using Backoffice_Services.Infrastructure.Cache;
using Microsoft.Extensions.Configuration;
using RFQService.Domain.Paging;

namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public class BidServiceClient : IBidServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cacheService;
        private readonly ILogger<BidServiceClient> _logger;
        private readonly TimeSpan _defaultCacheTime = TimeSpan.FromMinutes(5);

        public BidServiceClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ICacheService cacheService,
            ILogger<BidServiceClient> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["ExternalServices:BidService:BaseUrl"]);
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<PaginatedList<BidResponseModel>> GetBidsAsync(
            Guid? rfqId,
            Guid? bidderId,
            BidStatus? status,
            DateTime? fromDate,
            DateTime? toDate,
            PageRequest pageRequest)
        {
            try
            {
                var cacheKey = $"bids:{rfqId}:{bidderId}:{status}:{fromDate:yyyy-MM-dd}:{toDate:yyyy-MM-dd}:{pageRequest.Page}:{pageRequest.PageSize}";

                // Try to get from cache first
                var cachedResult = await _cacheService.GetAsync<PaginatedList<BidResponseModel>>(cacheKey);
                if (cachedResult != null)
                {
                    _logger.LogInformation("Cache hit for bids query: {CacheKey}", cacheKey);
                    return cachedResult;
                }

                var queryString = BuildQueryString(rfqId, bidderId, status, fromDate, toDate, pageRequest);
                var response = await _httpClient.GetAsync($"api/bids{queryString}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch bids. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to fetch bids from Bid Service");
                }

                var result = await response.Content.ReadFromJsonAsync<PaginatedList<BidResponseModel>>();

                // Cache the result
                await _cacheService.SetAsync(cacheKey, result, _defaultCacheTime);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bids");
                throw;
            }
        }

        public async Task<BidResponseModel> GetBidByIdAsync(Guid bidId)
        {
            try
            {
                var cacheKey = $"bid:{bidId}";

                // Try to get from cache first
                var cachedBid = await _cacheService.GetAsync<BidResponseModel>(cacheKey);
                if (cachedBid != null)
                {
                    _logger.LogInformation("Cache hit for bid: {BidId}", bidId);
                    return cachedBid;
                }

                var response = await _httpClient.GetAsync($"api/bids/{bidId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to get bid. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to get bid from Bid Service");
                }

                var bid = await response.Content.ReadFromJsonAsync<BidResponseModel>();

                // Cache the result
                await _cacheService.SetAsync(cacheKey, bid, _defaultCacheTime);

                return bid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bid: {BidId}", bidId);
                throw;
            }
        }

        public async Task<BidResponseModel> SubmitBidAsync(SubmitBidCommand request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/bids", request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to submit bid. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to submit bid to Bid Service");
                }

                var result = await response.Content.ReadFromJsonAsync<BidResponseModel>();

                // Invalidate relevant caches
                await InvalidateBidCaches(request.RfqId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting bid for RFQ: {RfqId}", request.RfqId);
                throw;
            }
        }

        public async Task<bool> UpdateBidStatusAsync(Guid bidId, BidStatus status, string? notes)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    $"api/bids/{bidId}/status",
                    new { Status = status, Notes = notes });

                response.EnsureSuccessStatusCode();

                // Use the new method with isRfq = false for bid-specific cache
                await InvalidateBidCaches(bidId, isRfq: false);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bid status. BidId: {BidId}, Status: {Status}", bidId, status);
                throw;
            }
        }

        private async Task InvalidateBidCaches(Guid id, bool isRfq = false)
        {
            try
            {
                if (isRfq)
                {
                    // Remove RFQ-specific bid list caches
                    var cacheKey = $"bids:rfq:{id}:*";
                    await _cacheService.RemoveAsync(cacheKey);
                    await _cacheService.RemoveAsync($"rfq_{id}_bids");
                }
                else
                {
                    // Remove bid-specific caches
                    await _cacheService.RemoveAsync($"bid_{id}");
                    await _cacheService.RemoveAsync("bids_all");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating bid caches for {Type} {Id}",
                    isRfq ? "RfqId" : "BidId", id);
            }
        }

        private string BuildQueryString(
            Guid? rfqId,
            Guid? bidderId,
            BidStatus? status,
            DateTime? fromDate,
            DateTime? toDate,
            PageRequest pageRequest)
        {
            var query = new List<string>();

            if (rfqId.HasValue)
                query.Add($"rfqId={rfqId}");

            if (bidderId.HasValue)
                query.Add($"bidderId={bidderId}");

            if (status.HasValue)
                query.Add($"status={status}");

            if (fromDate.HasValue)
                query.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");

            if (toDate.HasValue)
                query.Add($"toDate={toDate.Value:yyyy-MM-dd}");

            query.Add($"page={pageRequest.Page}");
            query.Add($"pageSize={pageRequest.PageSize}");

            return query.Any() ? "?" + string.Join("&", query) : string.Empty;
        }
    }
}