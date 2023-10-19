using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Screenshot
{
    public partial class Form1 : Form
    {
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_WHEEL = 0x0800;
        private const int MOUSEEVENTF_VIRTUALDESK = 0x4000;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref int plii);

        private System.Threading.Timer screenshotTimer;
        private System.Threading.Timer checkMouseTimer;

        public Form1()
        {

            InitializeComponent();
            screenshotTimer = new System.Threading.Timer(TakeScreenshot, null, 0, 10000); // Take screenshot every 10 seconds
            checkMouseTimer = new System.Threading.Timer(CheckMouseActivity, null, 0, 60000); // Check mouse activity every 60 seconds
        }

        private void TakeScreenshot(object state)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string fileName = $"{desktopPath}\\screenshot_{DateTime.Now:yyyyMMddHHmmss}.png";
            using (Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                }
                bitmap.Save(fileName, ImageFormat.Png);
                Console.WriteLine($"Screenshot saved as {fileName}");
            }
        }


        private void CheckMouseActivity(object state)
        {
            int idleTime = 0;
            while (true)
            {
                int lii = 0;
                GetLastInputInfo(ref lii);
                int elapsedSeconds = (Environment.TickCount - lii) / 1000;
                if (elapsedSeconds > 600) // If mouse is inactive for more than 10 minutes (600 seconds)
                {
                    Console.WriteLine("Mouse is inactive for more than 10 minutes. Stopping screenshot capture.");
                    break;
                }

                Thread.Sleep(10000); // Check mouse activity every 10 seconds
            }
        }
    }
}
