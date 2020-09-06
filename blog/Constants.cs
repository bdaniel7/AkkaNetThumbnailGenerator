namespace AkkaNetThumbnailGenerator
{
    public static class Constants
    {
        public static readonly string PostsDirectory = @"E:\projects\bdaniel7\AkkaNetThumbnailGenerator\_posts";
        public static readonly string BackgroundImage = @"E:\projects\bdaniel7\AkkaNetThumbnailGenerator\twitter.png";
        // don't generate the pngs in the project directory because Rider will attempt to include them in the project and index them.
        public static readonly string OutputDirectory = @"E:\temp\_out";
    }
}