using System;
using System.Collections.Generic;
using System.Linq;
using PiXharp.Raw;
using PiXharp.Exceptions;
using System.IO;

namespace PiXharp
{
    public enum ImageSize
    {
        SquareMedium,
        Medium,
        Large,
        Original
    }

    public enum ImageType
    {
        Illust,
        Ugoira,
        Manga
    }

    public enum Rating
    {
        None,
        R18,
        R18G
    }

    internal class ImageUris
    {
        internal Uri SquareMediumImageUri { get; }

        internal Uri MediumImageUri { get; }

        internal Uri LargeImageUri { get; }

        internal Uri OriginalImageUri { get; }

        internal ImageUris(string squareMediumUri, string mediumUri, string largeUri, string originalUri)
        {
            SquareMediumImageUri = new Uri(squareMediumUri);
            MediumImageUri = new Uri(mediumUri);
            LargeImageUri = new Uri(largeUri);
            OriginalImageUri = new Uri(originalUri);
        }

        internal Uri this[ImageSize imageSize]
        {
            get
            {
                switch (imageSize)
                {
                    case ImageSize.SquareMedium:
                        return SquareMediumImageUri;
                    case ImageSize.Medium:
                        return MediumImageUri;
                    case ImageSize.Large:
                        return LargeImageUri;
                    case ImageSize.Original:
                        return OriginalImageUri;
                    default:
                        throw new ArgumentException(nameof(imageSize), $"Parameter imageSize is invalid.");
                }
            }
        }
    }

    public class Illust
    {
        public long ID { get; }

        public string Title { get; }

        public string Caption { get; }

        public DateTimeOffset CreateDate { get; }

        public int PageCount { get; }

        public int TotalViews { get; }

        public int TotalBookmarks { get; }

        public ImageType ImageType { get; }        

        public IEnumerable<string> Tags { get; }

        // TODO: Add rating

        internal IReadOnlyList<ImageUris> ImageUris { get; }

        internal Illust(IllustResponse illust)
        {
            ID = illust.ID;
            Title = illust.Title ?? throw new PixivException($"Illust title is null. ID: {illust.ID}");
            Caption = illust.Caption ?? throw new PixivException($"Caption is null. ID: {illust.ID}");
            CreateDate = illust.CreateDate;
            PageCount = illust.PageCount;
            ImageType = GetImageTypeOf(illust);
            Tags = GetTagsOf(illust);
            ImageUris = GetImageUris(illust);
            TotalViews = illust.TotalView;
            TotalBookmarks = illust.TotalBookmarks;
        }

        public string GetFileName(int page, ImageSize imageSize)
        {
            if (page < 0 || PageCount <= page)
            {
                throw new ArgumentOutOfRangeException(nameof(page), $"Page is out of range. Value must be between 0 and {PageCount - 1}");
            }

            return Path.GetFileName(ImageUris[page][imageSize].ToString());
        }

        private ImageType GetImageTypeOf(IllustResponse illust)
        {
            switch (illust.Type ?? "")
            {
                case "illust":
                    return ImageType.Illust;
                case "ugoira":
                    return ImageType.Ugoira;
                case "manga":
                    return ImageType.Manga;
                default:
                    throw new PixivException($"Image type is undefined. ID: {illust.ID}");
            }
        }

        private IEnumerable<string> GetTagsOf(IllustResponse illust)
        {
            return illust.Tags?.Select(t => t?.Name ?? throw new PixivException($"Tag is null. ID: {illust.ID}")).ToArray()
                ?? throw new PixivException($"Tag is null. ID: {illust.ID}");
        }

        private IReadOnlyList<ImageUris> GetImageUris(IllustResponse illust)
        {
            var list = new List<ImageUris>();

            if (illust.PageCount == 1)
            {
                var sqM = illust.ImageUrls?.SquareMedium;
                var m = illust.ImageUrls?.Medium;
                var l = illust.ImageUrls?.Large;
                var org = illust.MetaSinglePage?.OriginalImageUrl;
                if (sqM != null && m != null && l != null && org != null)
                {
                    list.Add(new ImageUris(sqM, m, l, org));
                }
                else
                {
                    throw new PixivException($"Image urls are null. ID: {illust.ID}");
                }
            }
            else
            {
                var uris = illust.MetaPages.Select(p =>
                {
                    var sqM = p?.ImageUrls?.SquareMedium;
                    var m = p?.ImageUrls?.Medium;
                    var l = p?.ImageUrls?.Large;
                    var org = p?.ImageUrls?.Original;

                    if (sqM != null && m != null && l != null && org != null)
                    {
                        return new ImageUris(sqM, m, l, org);
                    }
                    else
                    {
                        throw new PixivException($"Image urls are null. ID: {illust.ID}");
                    }
                });
                list.AddRange(uris);
            }

            return list;
        }

        public override string ToString() => $"{ID}:{Title}";
    }
}