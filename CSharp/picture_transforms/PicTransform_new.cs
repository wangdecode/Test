using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using WebPMethod;

namespace test
{
    class main
    {
        static void Main(string[] args)
        {
            int argc = args.Length;
            
            if(argc == 0)
            {
                //输出用法
                string info = "usage: [exe] path\n\n" +
                "ReSizeWidth : only resize image while Width > 0\n" +
                "ImageFormat : jpg、png、bmp、tiff、gif\n" +
                "JpgQuality  : 0-100\n" +
                "IsNewFile   : if true will make a new folder\n";
                Console.WriteLine(info);
                return;
            }
            
            Picture pic = new Picture();
            for(int i=0; i < argc ; i++)
            {
                Console.WriteLine(args[i]);
                pic.transforms(args[i]);
            }
            
            Console.WriteLine("finish.");
            Console.Read();
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

        //判断是否为图片文件，是返回true
        /* public static bool IsPictureFile(string file)
        {
            string ext = file.Substring(file.LastIndexOf('\\') + 1);
            int index = ext.LastIndexOf('.');
            if(index != -1){
                ext = ext.Substring(index + 1);
                if(ext == "jpg" || ext == "png" ||
                   ext == "bmp" || ext == "tiff" || ext == "gif")
                    return true;
            }
            return false;
        } */
        
    }
    
    //图片转换类
    class Picture
    {
        int SuccessCount;
        int ErrorCount;
        
        string ext="jpg";
        long quality = 90;
        int width = 0;
        bool IsNewFile = false;
        
        public Picture()
        {
            ReadConfig();
            PrintIni();
        }
        
        public void PrintIni()
        {
            Console.WriteLine("ReSizeWidth = " + width.ToString());
            Console.WriteLine("ImageFormat = " + ext);
            Console.WriteLine("JpgQuality  = " + quality.ToString());
            Console.WriteLine("IsNewFile   = " + IsNewFile.ToString() + "\n");
        }
        
        //读取配置文件
        public void ReadConfig()
        {
            string file = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            file = file.Substring(0,file.LastIndexOf('.') + 1) + "txt";
            
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
                
                tmp = ReadOption(str, "IsNewFile");
                if(tmp.Length > 0)  IsNewFile = bool.Parse(tmp);
                
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
            string FilePath = path.Replace("\"","") + "\\";
            string NewPath;
            
            SuccessCount = 0;
            ErrorCount = 0;
            
            if(FileClass.IsDirectoryEmpty(path)) return;
            
            //当 IsNewFile 为 false 时覆盖原文件
            if(IsNewFile)
            {
                NewPath = path.Replace("\"","")+"_1\\";
                if(Directory.Exists(NewPath)) return;
                //原文件夹改名
                Directory.Move(path, NewPath);
                //创建新的目录
                DirectoryInfo dir = new DirectoryInfo(FilePath);
                dir.Create();
            } else {
                NewPath = FilePath;
            }
            
            ProgressBar pbar = new ProgressBar();
            files = FileClass.GetFileList(NewPath);
            for(int i=0; i<files.Length; i++)
            {
                string file_new;
                int strp = files[i].LastIndexOf('\\') + 1;
                int strl = files[i].LastIndexOf('.') - strp;
                file_new = FilePath + files[i].Substring(strp, strl) + "." + ext;
                transform(files[i], file_new);
                
                pbar.Dispaly(Convert.ToInt32((i+1.0)/files.Length*100));
            }
            ErrorCount = files.Length - SuccessCount;
            
            //输出结果
            Console.WriteLine(
                "\nSuccess: "+SuccessCount.ToString() +
                "\tError: "+ErrorCount.ToString());
        }
        
        //转换一个文件
        private void transform(string file_old, string file_new)
        {
            Bitmap image;
            try
            {
                string f_ext = file_old.Substring(file_old.LastIndexOf('.') + 1);
                Bitmap img;
                
                // 判断是否为 webp
                if(f_ext.ToLower() == "webp" && WebP.CheckFileExists())
                {
                    if(!WebP.Load(file_old, out img)) return;
                }else{
                    img = new Bitmap(file_old, true);
                }
                
                image = ReSize(img, width);
                
                img.Dispose();
            } catch (System.ArgumentException e)
            {
                Console.WriteLine(file_old + "\n" + e);
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
        private Bitmap ReSize(Bitmap img, int w)
        {
            Bitmap newimg;
            if( w == 0 || img.Width <= w)
            {
                newimg = new Bitmap(img);
            } else {
                int h = (int)((double)img.Height / img.Width * w);
                newimg = new Bitmap(img, w, h);
            }
            return newimg;
        }

    }
}
