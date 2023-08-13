using System.Net.Http.Json;
using DebtSnowballer.Shared.DTOs;

namespace Server.BLL.ServerSideServices;

public class MultiPurposeService : IMultiPurposeService
{
    private readonly HttpClient _httpClient;

    public MultiPurposeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DebtPayDownMethodDto>> GetAllStrategyTypesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<DebtPayDownMethodDto>>("api/MultiPurpose");
    }
}