namespace GraphInfo;

public sealed class GraphicalFileInfo
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double DpiX { get; set; }
    public double DpiY { get; set; }
    public int ColorDepth { get; set; }
    public string Compression { get; set; }

    public GraphicalFileInfo(string name, int width, int height, double dpiX, double dpiY, int colorDepth, string compression)
    {
        Name = name;
        Width = width;
        Height = height;
        ColorDepth = colorDepth;
        Compression = compression;
        DpiX = dpiX;
        DpiY = dpiY;
    }

    public FileInfoViewModel ToFileInfoViewModel()
    {
        return new FileInfoViewModel
        {
            Name = this.Name, 
            ColorDepth = this.ColorDepth, 
            Compression = this.Compression, 
            Dpi = $"{this.DpiX} x {this.DpiY}", 
            Resolution = $"{this.Width} x {this.Height}"
        };
    }
}