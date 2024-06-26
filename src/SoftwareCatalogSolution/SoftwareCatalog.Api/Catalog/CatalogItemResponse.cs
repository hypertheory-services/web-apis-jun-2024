using System.Text.Json.Serialization;

namespace SoftwareCatalog.Api.Catalog;

public record CatalogItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("_embedded")]
    public Dictionary<string, MetaInfo> Embedded { get; set; } = [];

    [JsonPropertyName("_links")]
    public Dictionary<string, string> Links { get; set; } = [];
}

public record MetaInfo
{
    public string Description { get; set; } = string.Empty;
}



/*{
  "id": "b0025560-22da-410b-aaae-fb4ffd1418f9",
  "title": "Jetbrains Rider",
  "_embedded": {
    "info": {
      "description": "An Ide For Developers"
    }
  },
  "_links": {
    "owner": "/techs/839839893"
  }
}*/