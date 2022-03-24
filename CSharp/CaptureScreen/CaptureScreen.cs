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
    
    // 截图类
    class Capture
    {
        int ScreenW=0;
        int ScreenH=0;
        ImageCodecInfo myImageCodecInfo;
        
        [DllImport("gdi32.dll", EntryPoint = "GetDeviceCaps", SetLastError = true)]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        
        public Capture()
        {
            //ScreenW = Screen.PrimaryScreen.Bounds.Width;
            //ScreenH = Screen.PrimaryScreen.Bounds.Height;
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr desktop = g.GetHdc();
                ScreenH = GetDeviceCaps(desktop, 117);
                ScreenW = GetDeviceCaps(desktop, 118);
            }
            
            myImageCodecInfo = GetEncoderInfo("image/jpeg");
            //Console.WriteLine("w="+ScreenW.ToString()+",h="+ScreenH.ToString());
        }
        
        // 截图方法
        public void capture()
        {
            int w = ScreenW;
            int h = ScreenH;
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
        
        // 判断系统是否支持图片格式，不支持返回null
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
        
        // 按时间生成文件名
        private string GetFileName()
        {
            string ext = ".jpg";
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
    
    // 热键类
    public class HotKey
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        
        [Flags()]
        public enum KeyModifiers { None = 0, Alt = 1, Ctrl = 2, Shift = 4, WindowsKey = 8 }
        
        // 注册热键
        public static void RegHotKey(IntPtr hwnd, int hotKeyId, KeyModifiers keyModifiers, Keys key)
        {
            if (!RegisterHotKey(hwnd, hotKeyId, keyModifiers, key))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode == 1409)
                    Console.WriteLine("热键被占用!");
                else
                    Console.WriteLine("注册热键失败！错误代码：" + errorCode.ToString());
            }
        }
        
        // 取消热键
        public static void UnRegHotKey(IntPtr hwnd, int hotKeyId)
        {
            UnregisterHotKey(hwnd, hotKeyId);
        }
    }
    
    // 窗口类
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
            //注册热键F2，Id号为100
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
