using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using WebPMethod;

namespace Test
{
    class TestWebp
    {
        static void Main(string[] args)
        {
            Bitmap bmp;
            byte[] webpImageData;
            
            // 测试 JPG -> WebP
            bmp = new Bitmap("test.jpg");
            WebP.Save(bmp, 80, "test.webp");

            // 测试 WebP -> PNG
            WebP.Load("test.webp", out bmp);
            bmp.Save("test.png", ImageFormat.Png);

            // 测试 WebP -> PNG.（文件加载到内存）
            webpImageData = File.ReadAllBytes("lossless.webp");
            WebP.Decode(webpImageData, out bmp);
            bmp.Save("test2.png", ImageFormat.Png);
            
            // 测试无损模式下 JPG -> WebP.（文件加载到内存）
            bmp = new Bitmap("test.jpg");
            WebP.EncodeLossless(bmp, out webpImageData);
            File.WriteAllBytes("lossless.webp", webpImageData);

            // 测试有损模式下 JPG -> WebP.（文件加载到内存）
            bmp = new Bitmap("test.jpg");
            WebP.EncodeLossly(bmp, 80, out webpImageData);
            File.WriteAllBytes("lossly.webp", webpImageData);
            
        }
    }
}