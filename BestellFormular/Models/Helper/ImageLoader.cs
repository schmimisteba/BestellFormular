using SkiaSharp;

namespace BestellFormular.Models.Helper
{
    public class ImageLoader
    {
        public static (bool hasImage, ImageSource ImageSource, int height, int width) LoadImage(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.EndsWith(".png"))
            {
                return (false, null, 0, 0);
            }
            using (var stream = File.OpenRead(path))
            {
                using (var bitmap = SKBitmap.Decode(stream))
                {
                    ImageSource image = ImageSource.FromStream(() => File.OpenRead(path));
                    var height = bitmap.Height;
                    var width = bitmap.Width;
                    return (true, image, height, width);
                }
            }
        }
    }
}
