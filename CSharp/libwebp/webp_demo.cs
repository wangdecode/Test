using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using WebP;

namespace Test
{
    class TestWebp
    {
        static void Main(string[] args)
        {

            //Test JPG to WebP
            Bitmap bmp1 = new Bitmap("test.jpg");
            clsWebP.Save(bmp1, 80, "test.webp");

            //Test WebP to PNG
            Bitmap bmp2;
            clsWebP.Load("test.webp", out bmp2);
            bmp2.Save("test.png", ImageFormat.Png);

            //Test JPG to WebP in lossless mode. Using compress in memory
            byte[] webpImageData1;
            Bitmap bmp3 = new Bitmap("test.jpg");
            clsWebP.EncodeLossless(bmp3, out webpImageData1);
            File.WriteAllBytes("lossless.webp", webpImageData1);

            //Test JPG to WebP in lossly mode. Using encode in memory
            byte[] webpImageData2;
            Bitmap bmp4 = new Bitmap("test.jpg");
            clsWebP.EncodeLossly(bmp4, 80, out webpImageData2);
            File.WriteAllBytes("lossly.webp", webpImageData2);

            //Test WebP to PNG. Using decode in memory
            Bitmap bmp5;
            byte[] webpImageData3 = File.ReadAllBytes("lossless.webp");
            clsWebP.Decode(webpImageData3, out bmp5);
            bmp4.Save("test2.png", ImageFormat.Png);

            //Test WebP to pictureBox
            Bitmap bmp6;
            clsWebP.Load("test.webp", out bmp6);
            pictureBox.Image = bmp6;
        }
    }
}
