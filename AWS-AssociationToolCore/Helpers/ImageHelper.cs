using SkiaSharp;
using System;
using System.IO;

// Root namespace lo undi, kabatti project loni anni controllers nunchi
// extra "using" lekunda direct ga ImageHelper.GetSize(...) ani vaadochu.
namespace AWS_AssociationToolCore
{
    public static class ImageHelper
    {
        /// <summary>
        /// Image width, height teesukuntundi (poorti image decode cheyadu, kabatti fast).
        /// File ledu leda image kaadu aithe (0, 0) return chestundi.
        /// </summary>
        public static (int Width, int Height) GetSize(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return (0, 0);

                using (var codec = SKCodec.Create(path))
                {
                    return codec == null ? (0, 0) : (codec.Info.Width, codec.Info.Height);
                }
            }
            catch
            {
                return (0, 0);
            }
        }

        /// <summary>
        /// Source image ni left/top/right/bottom (px) margins tho crop chesi destPath ki save chestundi.
        /// destPath extension (.png / .webp / .jpg) batti format select avtundi. Success aithe true.
        /// </summary>
        public static bool Crop(string sourcePath, string destPath, int left, int top, int right, int bottom, int quality = 90)
        {
            try
            {
                using (var original = SKBitmap.Decode(sourcePath))
                {
                    if (original == null) return false;

                    int cropWidth = original.Width - left - right;
                    int cropHeight = original.Height - top - bottom;
                    if (left < 0 || top < 0 || cropWidth <= 0 || cropHeight <= 0) return false;

                    var rect = new SKRectI(left, top, left + cropWidth, top + cropHeight);

                    using (var cropped = new SKBitmap())
                    {
                        if (!original.ExtractSubset(cropped, rect)) return false;
                        return SaveBitmap(cropped, destPath, quality);
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// File path nunchi resize chesi destPath ki save chestundi.
        /// height = 0 aithe aspect ratio maintain avtundi.
        /// </summary>
        public static bool Resize(string sourcePath, string destPath, int width, int height = 0, int quality = 90)
        {
            try
            {
                using (var fs = File.OpenRead(sourcePath))
                {
                    return Resize(fs, destPath, width, height, quality);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Stream nunchi resize chesi destPath ki save chestundi.
        /// width, height rendu istey ImageSharp laage exact size ki resize avtundi. height = 0 aithe aspect ratio maintain avtundi.
        /// </summary>
        public static bool Resize(Stream source, string destPath, int width, int height = 0, int quality = 90)
        {
            try
            {
                if (source == null || width <= 0) return false;
                if (source.CanSeek) source.Position = 0;

                using (var original = SKBitmap.Decode(source))
                {
                    if (original == null) return false;

                    if (height <= 0)
                        height = (int)Math.Round(original.Height * (width / (double)original.Width));
                    if (height <= 0) return false;

                    using (var resized = original.Resize(new SKImageInfo(width, height), SKFilterQuality.High))
                    {
                        if (resized == null) return false;
                        return SaveBitmap(resized, destPath, quality);
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Stream lo unna image ni destPath ki save chestundi (extension batti format: .png / .webp / .jpg).
        /// ImageSharp lo Image.Load(stream) + Save(path) ki replacement.
        /// </summary>
        public static bool SaveAs(Stream source, string destPath, int quality = 90)
        {
            try
            {
                if (source == null) return false;
                if (source.CanSeek) source.Position = 0;

                using (var bitmap = SKBitmap.Decode(source))
                {
                    if (bitmap == null) return false;
                    return SaveBitmap(bitmap, destPath, quality);
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Crop (left/top/right/bottom margins) chesi, tarvata width x height ki resize chesi destPath ki save chestundi.
        /// Crop size valid kakapothe original image ni alane save chestundi (ImageSharp code laage).
        /// </summary>
        public static bool CropAndResize(string sourcePath, string destPath, int left, int top, int right, int bottom, int width, int height, int quality = 90)
        {
            try
            {
                using (var original = SKBitmap.Decode(sourcePath))
                {
                    if (original == null) return false;

                    int cropWidth = original.Width - left - right;
                    int cropHeight = original.Height - top - bottom;

                    if (left >= 0 && top >= 0 && cropWidth > 0 && cropHeight > 0 && width > 0 && height > 0)
                    {
                        var rect = new SKRectI(left, top, left + cropWidth, top + cropHeight);

                        using (var cropped = new SKBitmap())
                        {
                            if (!original.ExtractSubset(cropped, rect)) return false;

                            using (var resized = cropped.Resize(new SKImageInfo(width, height), SKFilterQuality.High))
                            {
                                if (resized == null) return false;
                                return SaveBitmap(resized, destPath, quality);
                            }
                        }
                    }

                    // crop values valid kaaka pothe: original ni alane save
                    return SaveBitmap(original, destPath, quality);
                }
            }
            catch
            {
                return false;
            }
        }
        private static bool SaveBitmap(SKBitmap bitmap, string destPath, int quality)
        {
            string ext = Path.GetExtension(destPath).ToLowerInvariant();
            var format = ext == ".png" ? SKEncodedImageFormat.Png
                       : ext == ".webp" ? SKEncodedImageFormat.Webp
                       : SKEncodedImageFormat.Jpeg;

            string dir = Path.GetDirectoryName(destPath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            using (var img = SKImage.FromBitmap(bitmap))
            using (var data = img.Encode(format, quality))
            using (var fs = File.Create(destPath))
            {
                data.SaveTo(fs);
            }
            return true;
        }
    }
}
