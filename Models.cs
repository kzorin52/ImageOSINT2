using Newtonsoft.Json;

namespace ImageOSINT2;

public class ImageOne
{
    [JsonProperty("serp-item")] public SerpItem? SerpItem { get; set; }
}

public class SerpItem
{
    [JsonProperty("reqid")] public string? Reqid { get; set; }

    [JsonProperty("freshness")] public string? Freshness { get; set; }

    [JsonProperty("preview")] public List<Preview>? Preview { get; set; }

    [JsonProperty("dups")] public List<Dup>? Dups { get; set; }

    [JsonProperty("thumb")] public Thumb? Thumb { get; set; }

    [JsonProperty("snippet")] public Snippet? Snippet { get; set; }

    [JsonProperty("detail_url")] public string? DetailUrl { get; set; }

    [JsonProperty("img_href")] public Uri? ImgHref { get; set; }

    [JsonProperty("useProxy")] public bool UseProxy { get; set; }

    [JsonProperty("pos")] public long Pos { get; set; }

    [JsonProperty("id")] public string? Id { get; set; }

    [JsonProperty("rimId")] public string? RimId { get; set; }

    [JsonProperty("isMarketIncut")] public bool IsMarketIncut { get; set; }

    [JsonProperty("counterPath")] public string? CounterPath { get; set; }
}

public class Dup
{
    [JsonProperty("url")] public string? Url { get; set; }

    [JsonProperty("fileSizeInBytes")] public long FileSizeInBytes { get; set; }

    [JsonProperty("w")] public long W { get; set; }

    [JsonProperty("h")] public long H { get; set; }

    [JsonProperty("origin")] public DupOrigin? Origin { get; set; }

    [JsonProperty("isMixedImage")] public bool IsMixedImage { get; set; }
}

public class DupOrigin
{
    [JsonProperty("w")] public long W { get; set; }

    [JsonProperty("h")] public long H { get; set; }

    [JsonProperty("url")] public Uri? Url { get; set; }
}

public class Preview
{
    [JsonProperty("url")] public string? Url { get; set; }

    [JsonProperty("fileSizeInBytes")] public long FileSizeInBytes { get; set; }

    [JsonProperty("w")] public long W { get; set; }

    [JsonProperty("h")] public long H { get; set; }

    [JsonProperty("origin")] public PreviewOrigin? Origin { get; set; }

    [JsonProperty("isMixedImage")] public bool IsMixedImage { get; set; }
}

public class PreviewOrigin
{
    [JsonProperty("w")] public long W { get; set; }

    [JsonProperty("h")] public long H { get; set; }

    [JsonProperty("url")] public Uri? Url { get; set; }
}

public class Snippet
{
    [JsonProperty("title")] public string? Title { get; set; }

    [JsonProperty("hasTitle")] public bool HasTitle { get; set; }

    [JsonProperty("text")] public string? Text { get; set; }

    [JsonProperty("url")] public Uri? Url { get; set; }

    [JsonProperty("domain")] public string? Domain { get; set; }

    [JsonProperty("shopScore")] public long ShopScore { get; set; }
}

public class Thumb
{
    [JsonProperty("url")] public string? Url { get; set; }

    [JsonProperty("size")] public Size? Size { get; set; }
}

public class Size
{
    [JsonProperty("width")] public long Width { get; set; }

    [JsonProperty("height")] public long Height { get; set; }
}