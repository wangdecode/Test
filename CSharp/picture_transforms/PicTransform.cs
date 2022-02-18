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
            
            String[] files = null;
            Picture pic = new Picture();
            files = GetFileList(args[0]);
            
            switch (args.Length)
            {
                case 1:
                    pic.transforms(files);
                    break;
                case 2:
                    pic.transforms(files, args[1].ToLower());
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
                    pic.transforms(files, args[1].ToLower(), quality);
                    break;
                default:
                    readme();
                    return;
            }
            
            Console.WriteLine("count: "+files.Length);
        }
        
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
        
        public static void readme()
        {
            string info = "usage: [exe] path [ImageFormat] [quality]\n\n" +
                "ImageFormat (default=jpg):\njpg、png、bmp、tiff、gif\n\n" +
                "quality (default=90, jpg only):\n0-100\n";
            Console.WriteLine(info);
        }
    }
    
    
    class Picture
    {
        Bitmap image;
        
        public void transformStream(String file,String ext="jpg",long quality=90)
        {
            String file_new;
            String[] files;
            
            files = file.Split('.');
            file_new = System.AppDomain.CurrentDomain.BaseDirectory +
                files[0].Substring(files[0].LastIndexOf('\\')+1) + "." + ext;
            
            image = new Bitmap(file, true);
            
            
            image.Dispose();
        }
        
        public void transforms(String[] files,String ext="jpg",long quality=90)
        {
            foreach(String file in files)
            {
                transform(file, ext , quality);
            }
        }
        
        public void transform(String file,String ext="jpg",long quality=90)
        {
            String file_new;
            String[] files;
            
            files = file.Split('.');
            file_new = System.AppDomain.CurrentDomain.BaseDirectory +
                files[0].Substring(files[0].LastIndexOf('\\')+1) + "." + ext;
            
            
            
            image = new Bitmap(file, true);
            
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
        }
        
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
