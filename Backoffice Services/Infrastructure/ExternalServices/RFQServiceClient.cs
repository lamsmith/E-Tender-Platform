using System.Net.Http.Json;
using Backoffice_Services.Application.DTO.RFQManagement.Responses;
using Backoffice_Services.Application.Features.RFQManagement.Commands;
using Backoffice_Services.Application.Features.RFQManagement.Handlers;
using Backoffice_Services.Application.DTO.RFQManagement.Common;
using RFQService.Domain.Paging;

namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public class RFQServiceClient : IRFQServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RFQServiceClient> _logger;

        public RFQServiceClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<RFQServiceClient> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["ExternalServices:RFQService:BaseUrl"]);
            _logger = logger;
        }

        public async Task<PaginatedList<RFQResponseModel>> GetRFQsAsync(RFQFilterModel filter, PageRequest pageRequest)
        {
            try
            {
                var queryString = BuildQueryString(filter, pageRequest);
                var response = await _httpClient.GetAsync($"api/rfq{queryString}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch RFQs. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to fetch RFQs from RFQ Service");
                }

                return await response.Content.ReadFromJsonAsync<PaginatedList<RFQResponseModel>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting RFQs");
                throw;
            }
        }

        public async Task<RFQResponseModel> CreateRFQAsync(CreateRFQCommand request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/rfq", request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to create RFQ. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to create RFQ in RFQ Service");
                }

                return await response.Content.ReadFromJsonAsync<RFQResponseModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating RFQ: {@Request}", request);
                throw;
            }
        }

        public async Task<RFQResponseModel> GetRFQByIdAsync(Guid rfqId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/rfq/{rfqId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch RFQ. Status: {StatusCode}", response.StatusCode);
                    throw new Exception($"Failed to fetch RFQ {rfqId} from RFQ Service");
                }

                return await response.Content.ReadFromJsonAsync<RFQResponseModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching RFQ: {RFQId}", rfqId);
                throw;
            }
        }

        public async Task<bool> UpdateRFQStatusAsync(Guid rfqId, string status)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"/api/rfq/{rfqId}/status", new { Status = status });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating RFQ status: {RFQId}, {Status}", rfqId, status);
                throw;
            }
        }

        private string BuildQueryString(RFQFilterModel filter, PageRequest pageRequest)
        {
            var query = new List<string>();

            if (!string.IsNullOrEmpty(filter.Status))
                query.Add($"status={Uri.EscapeDataString(filter.Status)}");

            if (filter.FromDate.HasValue)
                query.Add($"fromDate={filter.FromDate.Value:yyyy-MM-dd}");

            if (filter.ToDate.HasValue)
                query.Add($"toDate={filter.ToDate.Value:yyyy-MM-dd}");

            query.Add($"page={pageRequest.Page}");
            query.Add($"pageSize={pageRequest.PageSize}");

            return query.Any() ? "?" + string.Join("&", query) : string.Empty;
        }
    }
}
