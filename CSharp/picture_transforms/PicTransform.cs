using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace test
{
    class main
    {
        static void Main(string[] args)
        {

            int argc = args.Length;
            if(argc == 0) {readme();return;}

            Picture pic = new Picture();
            
            for(int i=0; i < argc ; i++)
                pic.transforms(args[i]);
            
            Console.WriteLine("finish.");
            Console.Read();
        }
        
        //输出用法
        public static void readme()
        {
            string info = "usage: [exe] path\n\n" +
                "ReSizeWidth (default=0)  : only resize image while Width > 0\n" +
                "ImageFormat (default=jpg): jpg、png、bmp、tiff、gif\n" +
                "JpgQuality  (default=90) : 0-100\n";
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
        
        string ext="jpg";
        long quality = 90;
        int width = 0;
        
        public Picture()
        {
            ReadIni();
            Console.WriteLine("ReSizeWidth = " + width.ToString());
            Console.WriteLine("ImageFormat = " + ext);
            Console.WriteLine("JpgQuality  = " + quality.ToString() + "\n");
        }
        
        //读取配置文件
        public void ReadIni()
        {
            string file = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            file = file.Substring(0,file.LastIndexOf('.') + 1) + "ini";
            
            FileInfo fileinfo = new FileInfo(file);
            if (!fileinfo.Exists || fileinfo.Length > 4096) return;
            
            string str = null;
            using (StreamReader sr = new StreamReader(file))
            {
                str = sr.ReadToEnd();
            }
            
            string tmp;
            
            try
            {
                tmp = ReadOption(str, "ReSizeWidth");
                if(tmp.Length > 0) width = int.Parse(tmp);

                tmp = ReadOption(str, "ImageFormat");
                if(tmp.Length > 0) ext = tmp.ToLower();

                tmp = ReadOption(str, "JpgQuality");
                if(tmp.Length > 0) quality = long.Parse(tmp);
            } catch(System.FormatException e) {
                Console.WriteLine(e);
            }
        }
        
        private string ReadOption(string str, string option)
        {
            string pattern;
            Regex rgx;
            
            pattern = "(?<=" + option +"\\s*=\\s*)[^\\s]+";
            rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            string m = rgx.Match(str).Groups[0].ToString();
            
            return m;
        }
        
        //转换多个文件
        public void transforms(string path)
        {
            string[] files = null;
            
            SuccessCount = 0;
            ErrorCount = 0;
            FilePath = path.Replace("\"","") + "\\";
            string NewPath = path.Replace("\"","")+"_1\\";
            
            if(FileClass.IsDirectoryEmpty(path) || Directory.Exists(NewPath)) return;
            
            //原文件夹改名
            Directory.Move(path, NewPath);
            //创建新的目录
            DirectoryInfo dir = new DirectoryInfo(FilePath);
            dir.Create();
            
            files = FileClass.GetFileList(NewPath);
            foreach(string file in files)
            {
                transform(file);
            }
            
            ErrorCount = files.Length - SuccessCount;
            
            Console.WriteLine(path);
            result();
        }
        
        //转换一个文件
        private void transform(string file)
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
            Console.WriteLine(
                "Success: "+SuccessCount.ToString() +
                "\tError: "+ErrorCount.ToString());
        }
    }
}
