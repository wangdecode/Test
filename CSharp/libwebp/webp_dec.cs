using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;


namespace WebPMethod
{
    class WebP
    {
        // 检测文件是否存在
        // 返回：成功返回 True，否则返回 False
        public static bool CheckFileExists()
        {
            return File.Exists("libwebp.dll");
        }

        // 读取 WebP 文件
        // 输入：pathFileName，写入文件及路径
        // 输入：bmp，返回转换后的 Bitmap
        // 返回：成功返回 True，否则返回 False
        public static bool Load(string pathFileName, out Bitmap bmp)
        {
            bool result;
            byte[] dataWebP;
            bmp = null;

            try
            {
                // 读取 WebP 文件
                dataWebP = File.ReadAllBytes(pathFileName);
                result = Decode(dataWebP, out bmp);
                return result;
            }
            catch (Exception e) { Console.WriteLine(e); return false; }
        }

        // 解码 WebP 文件
        // 输入：webpData，要解析的 WebP 数据
        // 输入：bmp，返回解析后的 Bitmap
        // 返回：成功返回 True，否则返回 False
        public static bool Decode(byte[] webpData, out Bitmap bmp)
        {
            int imgWidth;
            int imgHeight;
            IntPtr outputBuffer;
            int outputBufferSize;
            bmp = null;

            try
            {
                // 获取图片宽高
                GCHandle pinnedWebP = GCHandle.Alloc(webpData, GCHandleType.Pinned);
                IntPtr ptrData = pinnedWebP.AddrOfPinnedObject();
                UInt32 dataSize = (uint)webpData.Length;
                if (WebPGetInfo(ptrData, dataSize, out imgWidth, out imgHeight) != 1) return false;

                // 创建 Bitmap 数据并锁定和写入
                bmp = new Bitmap(imgWidth, imgHeight, PixelFormat.Format24bppRgb);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);

                // 为解压图片分配内存空间
                outputBufferSize = bmpData.Stride * imgHeight;
                outputBuffer = Marshal.AllocHGlobal(outputBufferSize);

                // 解压图片
                outputBuffer = WebPDecodeBGRInto(ptrData, dataSize, outputBuffer, outputBufferSize, bmpData.Stride);

                // 使用 Marshal 写入 bitmap 数据
                //byte[] buffer = new byte[outputBufferSize];
                //Marshal.Copy(outputBuffer, buffer, 0, outputBufferSize);
                //Marshal.Copy(buffer, 0, bmpData.Scan0, outputBufferSize);

                // 使用 CopyMemory 写入 bitmap 数据. 比 Marshal 快，仅限 windows
                CopyMemory(bmpData.Scan0, outputBuffer, (uint)outputBufferSize);

                // 解除锁定
                bmp.UnlockBits(bmpData);

                // 释放内存
                pinnedWebP.Free();
                Marshal.FreeHGlobal(outputBuffer);

                return true;
            }
            catch (NullReferenceException) { return false; }
            catch (DllNotFoundException) { return false; }
            catch (EntryPointNotFoundException) { return false; }
            catch (Exception e) { Console.WriteLine(e); return false; }
        }

        // 验证 WebP 图片标头并检索图片宽度和高度，如果格式正确则 *width 和 *height 指针返回对应宽和高，否则返回 NULL
        // 输入：data，指向 WebP 图片数据的指针
        // 输入：data_size，data 指向的内存块大小
        // 输入：width ，宽度，范围为 1-16383.
        // 输入：height，高度，范围为 1-16383.
        // 返回：成功返回 1，否则返回对应错误代码
        [DllImport("libwebp.dll")]
        static extern int WebPGetInfo(IntPtr data, UInt32 data_size, out int width, out int height);

        // 解码 WebP 图片到预分配的缓冲区 output_buffer 中（BGR格式）
        // 输入：data，指向 WebP 图片数据的指针
        // 输入：data_size，data 指向的内存块大小
        // 输入：output_buffer ，返回指向已解码 WebP 图片的指针
        // 输入：output_buffer_size，分配的缓冲区大小
        // 输入：output_stride，扫描线间的距离
        // 返回：成功返回 output_buffer ，否则返回 NULL
        [DllImport("libwebp.dll")]
        static extern IntPtr WebPDecodeBGRInto(IntPtr data, UInt32 data_size, IntPtr output_buffer, int output_buffer_size, int output_stride);

        // CopyMemory方法
        [DllImport("kernel32.dll")]
        static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
    }
}
