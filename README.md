# Meilisearch Plugin for Jellyfin (with Semantic Hybrid Search)
A plugin for Jellyfin that improves search by utilizing Meilisearch as a search engine. Search logic is offloaded to a Meilisearch instance, and the response from Jellyfin is modified.

Improved:
* Speed
* Results _([fuzzy matching](https://en.wikipedia.org/wiki/Approximate_string_matching), typos)_
* **Semantic search** _(find "dinosaurs hunting humans" → Jurassic Park, without keywords matching)_

> [!NOTE]
> As long as your client uses `/Items` endpoint for search, it should be supported seamlessly _I guess_
> Inspired by [JellySearch](https://gitlab.com/DomiStyle/jellysearch) and [jellyfin-plugin-meilisearch](https://github.com/arnesacnussem/jellyfin-plugin-meilisearch).

---
### Setup instructions
1. **Setup a Meilisearch instance** _(maybe a hosted one in the cloud will also work, but I don't recommend)_
    - Docker is recommended. Example `docker-compose.yml`:
       ```
       services:
          meilisearch:
            container_name: meilisearch
            image: getmeili/meilisearch:v1.48.1
            restart: unless-stopped
        
            environment:
              MEILI_ENV: production
              MEILI_NO_ANALYTICS: "true"
              MEILI_MASTER_KEY: super-secret-key
              MEILI_EXPERIMENTAL_ALLOWED_IP_NETWORKS: 172.16.0.0/12,fd00::/8
              MEILI_OLLAMA_URL: http://ollama:11434/api/embed
              MEILI_EXPERIMENTAL_REST_EMBEDDER_TIMEOUT_SECONDS: 300
        
            volumes:
              - ./data:/meili_data
        
            ports:
              - 7700:7700
       ```

2. **(Optional) Setup Ollama for semantic search**
    - Run an Ollama instance and pull an embedding model:
       ```
       ollama pull bge-m3
       ```
    - Configure an embedder on your Meilisearch index named `semantic`:
       ```json
       {
         "semantic": {
           "source": "ollama",
           "model": "bge-m3",
           "documentTemplate": "{{doc.name}}. {{doc.overview}} Genres: {{doc.genres}}. Tags: {{doc.tags}}. {{doc.tagline}}",
           "documentTemplateMaxBytes": 2000
         }
       }
       ```
    - Wait for all documents to be embedded before enabling hybrid search.

3. **Install the Meilisearch plugin**
    - In Jellyfin:
        1. Add the plugin Repository:
            ```
            https://raw.githubusercontent.com/Samxel/jellyfin-plugin-meilisearch-hybrid/refs/heads/manifest/manifest.json
            ```
        2. Install the Meilisearch plugin
        3. Restart Jellyfin Server

4. **Configure the Meilisearch plugin**
   - In Meilisearch plugin's page:    
       1. **Meilisearch URL**: URL to your Meilisearch instance, as seen by Jellyfin _(example: `http://meilisearch:7700`)_
       2. **Meilisearch Api Key**: API key to access your Meilisearch instance _(if required)_
       3. **(Optional) Enable Semantic (Hybrid) Search**: toggle on if you have an Ollama embedder configured
       4. **Embedder Name**: name of the embedder on the index _(default: `semantic`)_
       5. **Semantic Ratio**: balance between keyword `0.0` and semantic `1.0` search _(default: `0.5`)_
       6. Click `Save`
       7. The plugin's page should show a healthy status
           - Example:
              ```
              {
                  "meilisearch": "Server: available",
                  "meilisearchOk": true,
                  "averageSearchTime": "0ms",
                  "indexStatus": {
                    "Database": "...",
                    "Items": "40758",
                    "LastIndexed": "6/25/2026 10:17:00 AM"
                  }
                }
              ```

> [!NOTE]
> You can also set the environment variables in Jellyfin to configure the plugin without editing the UI: `MEILI_URL` and `MEILI_MASTER_KEY`

> [!NOTE]
> If you want to share one Meilisearch instance across multiple Jellyfin instances, fill in the `Meilisearch Index Name`. If left empty, it will use the server name.

5. Test Meilisearch plugin search
    1. Try Jellyfin search — with semantic search enabled, try queries like _"dinosaurs hunting humans"_ or _"sad film about hope"_
    2. Issues? Check **Jellyfin's logs** and **Meilisearch's logs**

---
Index will update on following events:
- Server start
- Configuration change
- Library scan complete
- Update index task being triggered

---
### How it works
The core feature, which is to mutate the search request, is done by injecting an [`ActionFilter`](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-8.0#action-filters).
So it may only support a few versions of Jellyfin. At the moment tested on `Jellyfin 10.11.11`,
but it should work on other versions as long as the required parameter name of `/Items` endpoint doesn't change.

Semantic hybrid search uses [Meilisearch's built-in vector search](https://www.meilisearch.com/docs/learn/ai_powered_search/getting_started_with_ai_search) with an [Ollama](https://ollama.com) embedding model running locally. No external API calls, fully self-hosted.

---
This is a fork of [arnesacnussem/jellyfin-plugin-meilisearch](https://github.com/arnesacnussem/jellyfin-plugin-meilisearch) with added semantic hybrid search support.
