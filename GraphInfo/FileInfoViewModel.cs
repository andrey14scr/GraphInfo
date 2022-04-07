namespace GraphInfo;

public sealed class FileInfoViewModel
{
    public string Name { get; set; }
    public string Resolution { get; set; }
    public string Dpi { get; set; }
    public double ColorDepth { get; set; }
    public string Compression { get; set; }

    public FileInfoViewModel()
    {
        
    }

    public FileInfoViewModel(string name, string resolution, string dpi, int colorDepth, string compression)
    {
        Name = name;
        ColorDepth = colorDepth;
        Compression = compression;
        Dpi = dpi;
        Resolution = resolution;
    }
}