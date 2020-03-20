using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PiXharp.RawObjects
{
    public class IllustsPage
    {
        [JsonPropertyName("illusts")]
        public Illust[]? Illusts { get; set; }
        [JsonPropertyName("next_url")]
        public string? NextUrl { get; set; }
        [JsonPropertyName("search_span_limit")]
        public int SearchSpanLimit { get; set; }
    }

    public class IllustContainer
    {
        [JsonPropertyName("illust")]
        public Illust? Illust { get; set; }
    }

    public class Illust
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("image_urls")]
        public ImageUrls? ImageUrls { get; set; }
        [JsonPropertyName("caption")]
        public string? Caption { get; set; }
        [JsonPropertyName("restrict")]
        public int Restrict { get; set; }
        [JsonPropertyName("user")]
        public User? User { get; set; }
        [JsonPropertyName("tags")]
        public Tag[]? Tags { get; set; }
        [JsonPropertyName("tools")]
        public string[]? Tools { get; set; }
        [JsonPropertyName("create_date")]
        public DateTimeOffset CreateDate { get; set; }
        [JsonPropertyName("page_count")]
        public int PageCount { get; set; }
        [JsonPropertyName("width")]
        public int Width { get; set; }
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("sanity_level")]
        public int SanityLevel { get; set; }
        [JsonPropertyName("x_restrict")]
        public int XRestrict { get; set; }
        // TODO: seriesの型は何？
        [JsonPropertyName("series")]
        public object? Series { get; set; }
        [JsonPropertyName("meta_single_page")]
        public MetaSinglePage? MetaSinglePage { get; set; }
        [JsonPropertyName("meta_pages")]
        public MetaPages[]? MetaPages { get; set; }
        [JsonPropertyName("total_view")]
        public int TotalView { get; set; }
        [JsonPropertyName("total_bookmarks")]
        public int TotalBookmarks { get; set; }
        [JsonPropertyName("is_bookmarked")]
        public bool IsBookmarked { get; set; }
        [JsonPropertyName("visible")]
        public bool Visible { get; set; }
        [JsonPropertyName("is_muted")]
        public bool IsMuted { get; set; }

        public override string ToString() => $"{ID}:{Title}";
    }

    public class User
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("account")]
        public string? Account { get; set; }
        [JsonPropertyName("profile_image_urls")]
        public ProfileImageUrls? ProfileImageUrls { get; set; }
        [JsonPropertyName("is_followed")]
        public bool IsFollowed { get; set; }
    }

    public class ProfileImageUrls
    {
        [JsonPropertyName("medium")]
        public string? Medium { get; set; }
    }

    public class MetaSinglePage
    {
        [JsonPropertyName("original_image_url")]
        public string? OriginalImageUrl { get; set; }
    }

    public class Tag
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        // TODO: translated_nameの型は？
        [JsonPropertyName("translated_name")]
        public object? TranslatedName { get; set; }
    }

    public class MetaPages
    {
        [JsonPropertyName("image_urls")]
        public ImageUrls? ImageUrls { get; set; }
    }

    public class ImageUrls
    {
        public string? SquareMedium { get; set; }
        public string? Medium { get; set; }
        public string? Large { get; set; }
        public string? Original { get; set; }
    }
}
