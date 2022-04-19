using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace test
{
    class main
    {
        static void Main(string[] args)
        {
            string path="", ext="jpg";
            long quality = 90;
            int width = 0;
            
            int argsl = args.Length;
            if(argsl == 0 || argsl > 4) {readme();return;}
            if(argsl >= 1) path = args[0];
            if(argsl >= 2)
            {
                try
                {
                    width = int.Parse(args[1]);
                } catch(System.FormatException e) {
                    Console.WriteLine(e);
                    return;
                }
            }
            if(argsl >= 3) ext = args[2].ToLower();
            if(argsl >= 4)
            {
                try
                {
                    quality = long.Parse(args[3]);
                } catch(System.FormatException e) {
                    Console.WriteLine(e);
                    return;
                }
            }
            
            Picture pic = new Picture();
            pic.transforms(path, width, ext, quality);
        }
        
        //输出用法
        public static void readme()
        {
            string info = "usage: [exe] path [ReSizeWidth] [ImageFormat] [quality]\n\n" +
                "ReSizeWidth (default=0):\tonly resize image while Width > 0\n\n" +
                "ImageFormat (default=jpg):\tjpg、png、bmp、tiff、gif\n\n" +
                "quality (default=90, jpg use):\t0-100\n";
            Console.WriteLine(info);
        }
    }
    
    //文件类
    class FileClass
    {
        //获取目录及子目录下的文件
        public static string[] GetFileList(string path)
        {
            List<string> fileList = new List<string>();
            
            if (Directory.Exists(path) == true)
            {
                foreach (string file in Directory.GetFiles(path))
                    fileList.Add(file);
                foreach (string directory in Directory.GetDirectories(path))
                    fileList.AddRange(GetFileList(directory));
            }
            
            return fileList.ToArray();
        }
        
        //判断目录是否为空，为空返回true
        public static bool IsDirectoryEmpty(string path)
        {
            if (Directory.Exists(path) == false)
                return true;
            if (Directory.GetDirectories(path).Length > 0 || Directory.GetFiles(path).Length > 0)
                return false;
            else
                return true;
        }
    }
    
    //图片转换类
    class Picture
    {
        Bitmap image;
        int SuccessCount;
        int ErrorCount;
        string FilePath = "";
        
        //转换多个文件
        public void transforms(string path, int width, string ext, long quality)
        {
            string[] files = null;
            
            SuccessCount = 0;
            ErrorCount = 0;
            FilePath = path.Replace("\"","") + "\\";
            string NewPath = path.Replace("\"","")+"_1\\";
            
            if(FileClass.IsDirectoryEmpty(path) || Directory.Exists(NewPath)) return;
            Console.WriteLine(path);
            
            //原文件夹改名
            Directory.Move(path, NewPath);
            //创建新的目录
            DirectoryInfo dir = new DirectoryInfo(FilePath);
            dir.Create();
            
            files = FileClass.GetFileList(NewPath);
            foreach(string file in files)
            {
                transform(file, width, ext , quality);
            }
            
            ErrorCount = files.Length - SuccessCount;
            result();
        }
        
        //转换一个文件
        private void transform(string file, int width, string ext, long quality)
        {
            string file_new;
            int strp = file.LastIndexOf('\\') + 1;
            int strl = file.LastIndexOf('.') - strp;
            
            file_new = FilePath + file.Substring(strp, strl) + "." + ext;
            
            try
            {
                image = ReSize(file, width);
            } catch (System.ArgumentException e)
            {
                Console.WriteLine(file + "\n" + e);
                return;
            }
            
            switch(ext)
            {
                case "jpg":
                    ImageCodecInfo Encoder = GetEncoder(ImageFormat.Jpeg);
                    System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    image.Save(file_new, Encoder, myEncoderParameters);
                    break;
                case "png":
                    image.Save(file_new, ImageFormat.Png);
                    break;
                case "bmp":
                    image.Save(file_new, ImageFormat.Bmp);
                    break;
                case "tiff":
                    image.Save(file_new, ImageFormat.Tiff);
                    break;
                case "gif":
                    image.Save(file_new, ImageFormat.Gif);
                    break;
            }
            
            image.Dispose();
            SuccessCount++;
        }
        
        //选择图片格式
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)  
            {
                if (codec.FormatID == format.Guid) return codec;  
            }
            return null;
        }
        
        //图片缩放，按宽等比缩放
        private Bitmap ReSize(string file, int w)
        {
            Bitmap img = new Bitmap(file, true);
            if( w == 0 || img.Width <= w) return img;
            
            int h = (int)((double)img.Height / img.Width * w);
            Bitmap newimg = new Bitmap(img, w, h);
            
            img.Dispose();
            
            return newimg;
        }
        
        //输出结果
        private void result()
        {
            Console.WriteLine("Success: "+SuccessCount.ToString()+"\tError: "+ErrorCount.ToString());
        }
    }
}
