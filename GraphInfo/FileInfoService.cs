using System;
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

            return new GraphicalFileInfo(name, xMax - xMin + 1, yMax - yMin + 1, xDpi, yDpi, pcxColorDepth, "");
        }
        
        var bmi = new BitmapImage(new Uri(filePath));
        var width = bmi.PixelWidth;
        var height = bmi.PixelHeight;
        var colorDepth = bmi.Format.BitsPerPixel;
        var dpiX = bmi.DpiX;
        var dpiY = bmi.DpiY;

        return new GraphicalFileInfo(name, width, height, dpiX, dpiY , colorDepth, "");
    }
}