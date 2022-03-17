using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;

namespace CaptureScreen
{
    class MainClass
    {
        static void Main(string[] args)
        {
            Application.Run(new Form1());
        }
    }
    
    class Capture
    {
        int ScreenX=0;
        int ScreenY=0;
        ImageCodecInfo myImageCodecInfo;
        
        public Capture()
        {
            ScreenX = Screen.PrimaryScreen.Bounds.Width;
            ScreenY = Screen.PrimaryScreen.Bounds.Height;
            myImageCodecInfo = GetEncoderInfo("image/jpeg");
            //Console.WriteLine("w="+ScreenX.ToString()+",h="+ScreenY.ToString());
        }
        
        public void capture()
        {
            int w = ScreenX;
            int h = ScreenY;
            string FileName = GetFileName();
            
            Bitmap bitmap = new Bitmap(w, h);
            using(Graphics graphics = Graphics.FromImage(bitmap))
            {
                //sourceX, sourceY, destinationX, destinationY, Size
                graphics.CopyFromScreen(0, 0, 0, 0, new Size(w, h));
                
                Encoder myEncoder;
                EncoderParameter myEncoderParameter;
                EncoderParameters myEncoderParameters;
                
                myEncoder = Encoder.Quality;
                myEncoderParameters = new EncoderParameters(1);
                myEncoderParameter = new EncoderParameter(myEncoder, 90L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                
                bitmap.Save(FileName, myImageCodecInfo, myEncoderParameters);
                bitmap.Dispose();
            }
        }
        
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for(j = 0; j < encoders.Length; ++j)
            {
                if(encoders[j].MimeType == mimeType)
                return encoders[j];
            }
            return null;
        }
        
        private string GetFileName()
        {
            string ext=".jpg";
            string nowdate = DateTime.Now.ToString("yyyy.MM.dd_hhmmss");
            string name = nowdate + ext;
            
            int num = 1;
            while(File.Exists(name))
            {
                name = nowdate + "_" + num.ToString() + ext;
                num++;
            }
            
            //Console.WriteLine("name="+name);
            return name;
        }
    }
    
    public class HotKey
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        
        [Flags()]
        public enum KeyModifiers { None = 0, Alt = 1, Ctrl = 2, Shift = 4, WindowsKey = 8 }
        
        public static void RegHotKey(IntPtr hwnd, int hotKeyId, KeyModifiers keyModifiers, Keys key)
        {
            if (!RegisterHotKey(hwnd, hotKeyId, keyModifiers, key))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode == 1409)
                    Console.WriteLine("ÈÈ¼ü±»Õ¼ÓÃ!");
                else
                    Console.WriteLine("×¢²áÈÈ¼üÊ§°Ü£¡´íÎó´úÂë£º" + errorCode.ToString());
            }
        }
        
        public static void UnRegHotKey(IntPtr hwnd, int hotKeyId)
        {
            UnregisterHotKey(hwnd, hotKeyId);
        }
    }
    
    public partial class Form1 : Form
    {
        public Form1()  
        {
            this.Load += new EventHandler(this.Form1_Load);
            this.Shown += new EventHandler(this.Form1_Shown);
            this.FormClosing += new FormClosingEventHandler(this.Form1_Close);
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            //×¢²áÈÈ¼üF2£¬IdºÅÎª100
            HotKey.RegHotKey(Handle, 100, HotKey.KeyModifiers.None, Keys.F2);
            HotKey.RegHotKey(Handle, 101, HotKey.KeyModifiers.None, Keys.F3);
            Console.WriteLine("reg F2:capture\nreg F3:exit");
        }
        
        private void Form1_Shown(Object sender, EventArgs e)
        {
            this.Hide();
        }
        
        private void Form1_Close(object sender, FormClosingEventArgs e)
        {
            HotKey.UnRegHotKey(Handle, 100);
            HotKey.UnRegHotKey(Handle, 101);
            Console.WriteLine("unreg F2 F3");
        }
        
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 100:
                            Capture c = new Capture();
                            c.capture();
                            break;
                        case 101:
                            this.Close();
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
