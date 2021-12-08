using Microsoft.Net.Http.Headers;
using MVCLocalization.Web.Controllers;

namespace MVCLocalization.Web.Services
{
    public interface IGitHubService
    {
        Task<IEnumerable<GitHubBranch>> GetAspNetCoreDocsBranchesAsync();
    }
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;
        public GitHubService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.github.com/");
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.github.v3+json");
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "HttpRequestsSample");
            string language = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            _httpClient.DefaultRequestHeaders.AcceptLanguage.Add
                (
                    new System.Net.Http.Headers.StringWithQualityHeaderValue(language)
                );
        }
        public async Task<IEnumerable<GitHubBranch>> GetAspNetCoreDocsBranchesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GitHubBranch>>("repos/dotnet/AspNetCore.Docs/branches");
        }
    }

}
