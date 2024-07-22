using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace modi_ocr
{
    public partial class Form1 : Form
    {
        string filepath;
        string basepath = Environment.CurrentDirectory;
        Form2 form2 = new Form2();//新建窗体2
        public Form1()
        {
            InitializeComponent();
            //显示窗体2
            form2.Show();
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            filepath = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            viewpic(filepath);
        }
        private void viewpic(string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader br = new BinaryReader(fs);
            MemoryStream ms = new MemoryStream(br.ReadBytes((int)fs.Length));
            fs.Close();
            pictureBox1.Image = Image.FromStream(ms);
        }
        private void modiocr(string filepath)
        {
            MODI.MiLANGUAGES lang;
            switch (comboBox1.SelectedIndex)
            {
                case 0: lang = MODI.MiLANGUAGES.miLANG_CHINESE_SIMPLIFIED; break;
                case 1: lang = MODI.MiLANGUAGES.miLANG_JAPANESE; break;
                default: lang = MODI.MiLANGUAGES.miLANG_ENGLISH; break;
            }
            MODI.Document doc = new MODI.Document();
            try
            {
                doc.Create(filepath);
                doc.OCR(lang, AutoRotation.Checked, StraightenImage.Checked);
            }
            catch (System.Exception ex)
            {
                textBox1.Text = ex.Message;
                return;
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < doc.Images.Count; i++)
            {
                sb.Append(doc.Images[i].Layout.Text);
            }
            doc.Close(false);
            textBox1.Text = sb.ToString();
            if (AutoCopy.Checked == true)
            {
                Clipboard.SetDataObject(textBox1.Text);
            }
        }
        private void getword(Bitmap curBitmap, byte[] color)
        {
            byte temp = 0;
            if ( (color[0] + color[1] + color[2]) < 360)
            {
                temp = 255;
            }
            Rectangle rect = new Rectangle(0, 0, curBitmap.Width, curBitmap.Height);
            BitmapData bmpData = curBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            int bytesCount = bmpData.Stride * bmpData.Height;
            byte[] arrDst = new byte[bytesCount];
            System.Runtime.InteropServices.Marshal.Copy(ptr, arrDst, 0, bytesCount);
            bytesCount = bytesCount - 2;
            int t;
            int t2 = Int32.Parse(textBox2.Text);
            if (t2 < 0 || t2 > 1000)
            {
                t2 = 100;
            }
            for (int i = 0; i < bytesCount; i += 3)//B G R
            {
                t = Math.Abs(arrDst[i] - color[0]) + Math.Abs(arrDst[i + 1] - color[1]) + Math.Abs(arrDst[i + 2] - color[2]);
                if (t > t2)
                {
                    arrDst[i] = temp;
                    arrDst[i + 1] = temp;
                    arrDst[i + 2] = temp;
                }
            }
            System.Runtime.InteropServices.Marshal.Copy(arrDst, 0, ptr, bytesCount);
            curBitmap.UnlockBits(bmpData);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "支持格式(bmp,jpg,png)|*.bmp;*.jpg;*.jpeg;*.png|所有文件|*.*";
            if (file.ShowDialog() == DialogResult.OK)
            {
                filepath = file.FileName;
                viewpic(filepath);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(filepath) == true)
            {
                modiocr(filepath);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(form2.Width, form2.Height);
            Graphics img = Graphics.FromImage(image);
            img.CopyFromScreen(form2.Location.X, form2.Location.Y, 0, 0, image.Size);
            if (checkBox1.Checked == true)
            {
                byte[] color = { 0xff, 0xff, 0xff };
                switch (comboBox2.SelectedIndex)
                {
                    case 0: break;
                    case 1: color[0] = 0; color[1] = 0; color[2] = 0; break;
                    default: color[0] = 0; color[1] = 0; color[2] = 0; break;
                }
                getword(image, color);
            }
            pictureBox1.Image = Image.FromHbitmap(image.GetHbitmap());
            filepath = basepath + "\\temp.png";
            image.Save(filepath, ImageFormat.Jpeg);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (form2.Visible == true)
            {
                form2.Hide();
                button4.Text = "显示窗口";
            }
            else
            {
                form2.Show();
                button4.Text = "隐藏窗口";
            }
        }
    }
}
