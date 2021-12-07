using System.Text.Json.Serialization;

namespace MVCLocalization.Web.Controllers
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    [Serializable]
    public class Commit
    {
        public string sha { get; set; }
        public string url { get; set; }
    }
    [Serializable]
    public class GitHubBranch
    {
        public string name { get; set; }
        public Commit commit { get; set; }

        [JsonPropertyName("protected")]
        public bool IsProtected { get; set; }
    }
}