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
            if (args.Length == 0)
            {
                readme();
                return;
            }
            
            Picture pic = new Picture();
            String[] files = null;
            files = GetFileList(args[0]);
            int count = 0;
            
            switch (args.Length)
            {
                case 1:
                    count = pic.transforms(files);
                    break;
                case 2:
                    count = pic.transforms(files, args[1].ToLower());
                    break;
                case 3:
                    long quality;
                    try
                    {
                        quality = long.Parse(args[2]);
                    } catch(System.FormatException e) {
                        Console.WriteLine(e);
                        return;
                    }
                    count = pic.transforms(files, args[1].ToLower(), quality);
                    break;
                default:
                    readme();
                    return;
            }
            
            Console.WriteLine("Success: "+count.ToString()+"\nError: "+(files.Length-count).ToString());
        }
        
        //获取目录及子目录下的文件
        public static String[] GetFileList(String path)
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
        
        //输出用法
        public static void readme()
        {
            string info = "usage: [exe] path [ImageFormat] [quality]\n\n" +
                "ImageFormat (default=jpg):\njpg、png、bmp、tiff、gif\n\n" +
                "quality (default=90, jpg only):\n0-100\n";
            Console.WriteLine(info);
        }
    }
    
    //图片转换类
    class Picture
    {
        Bitmap image;
        int SuccessCount;
        
        //转换多个文件
        public int transforms(String[] files,String ext="jpg",long quality=90)
        {
            SuccessCount = 0;
            foreach(String file in files)
            {
                transform(file, ext , quality);
            }
            return SuccessCount;
        }
        
        //转换一个文件
        private void transform(String file,String ext="jpg",long quality=90)
        {
            String file_new;
            String[] files;
            
            files = file.Split('.');
            file_new = System.AppDomain.CurrentDomain.BaseDirectory +
                files[0].Substring(files[0].LastIndexOf('\\')+1) + "." + ext;
            
            try
            {
                image = new Bitmap(file, true);
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
                if (codec.FormatID == format.Guid)  
                {  
                    return codec;  
                }  
            }  
            return null;  
        }
    }
}
