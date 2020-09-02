using System;

namespace AkkaNetThumbnailGenerator
{
    public class FrontMatter
    {
        public string Title { get; set; } = default!;
        public string Author { get; set; } = default!;
        public DateTimeOffset? Date { get; set; } = default!;
    }
}