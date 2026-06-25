using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Meilisearch;

public class Config : BasePluginConfiguration
{
    public Config()
    {
        ApiKey = string.Empty;
        Url = string.Empty;
        Debug = false;
        IndexName = string.Empty;
        MatchingStrategy = "last";
        EnableSemanticSearch = false;
        EmbedderName = "semantic";
        SemanticRatio = 0.5;
    }

    public string ApiKey { get; set; }
    public string Url { get; set; }

    public bool Debug { get; set; }
    public string IndexName { get; set; }

    /// <summary>
    /// Meilisearch matchingStrategy: "last", "all", or "frequency".
    /// </summary>
    public string MatchingStrategy { get; set; }

    public bool EnableSemanticSearch { get; set; }
    public string EmbedderName { get; set; }

    /// <summary>
    /// Hybrid search semantic ratio between 0.0 (full keyword) and 1.0 (full semantic).
    /// </summary>
    public double SemanticRatio { get; set; }
}
