using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace GraphInfo;

public sealed class FileInfoService
{
    public GraphicalFileInfo GetInfo(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        var name = fileInfo.Name;
        var ext = fileInfo.Extension;
        
        if (!Enum.TryParse<FileTypes>(ext[1].ToString().ToUpper() + ext[2..].ToLower(), out var fileType))
        {
            throw new Exception();
        }

        var compression = string.Empty;

        switch (fileType)
        {
            case FileTypes.Jpg:
            case FileTypes.Jpeg:
                compression = "lossy";
                break;
            case FileTypes.Bmp:
                compression = "none";
                break;
            case FileTypes.Tif:
            case FileTypes.Tiff:
                var img = Image.FromFile(fileInfo.FullName);
                var compressionTagIndex = Array.IndexOf(img.PropertyIdList, 0x0103);
                var compressionTag = img.PropertyItems[compressionTagIndex];
                var com = BitConverter.ToInt16(compressionTag.Value, 0);
                compression = com + " (tag)";
                break;
            case FileTypes.Gif:
            case FileTypes.Png:
            case FileTypes.Pcx:
                compression = "lossless";
                break;
        }

        if (fileType == FileTypes.Pcx)
        {
            using var myFile = new BinaryReader(File.Open(filePath, FileMode.Open));
            
            myFile.BaseStream.Seek(3, SeekOrigin.Begin);
            var pcxColorDepth = myFile.ReadByte();
            var xMin = myFile.ReadInt16();
            var yMin = myFile.ReadInt16();
            var xMax = myFile.ReadInt16();
            var yMax = myFile.ReadInt16();
            var xDpi = myFile.ReadInt16();
            var yDpi = myFile.ReadInt16();

            return new GraphicalFileInfo(name, xMax - xMin + 1, yMax - yMin + 1, xDpi, yDpi, pcxColorDepth, compression);
        }

        var bmi = new BitmapImage(new Uri(filePath));
        var width = bmi.PixelWidth;
        var height = bmi.PixelHeight;
        var colorDepth = bmi.Format.BitsPerPixel;
        var dpiX = Math.Round(bmi.DpiX, 1);
        var dpiY = Math.Round(bmi.DpiY, 1);

        return new GraphicalFileInfo(name, width, height, dpiX, dpiY , colorDepth, compression);
    }
}