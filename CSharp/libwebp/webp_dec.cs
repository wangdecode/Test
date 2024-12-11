using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;


namespace WebP
{
    class clsWebP
    {
        /// <summary>Read a WebP file</summary>
        /// <param name="pathFileName">WebP file to load</param>
        /// <param name="bmp">Bitmap with the WebP image</param>
        /// <returns>True if success; False otherwise</returns>
        public static bool Load(string pathFileName, out Bitmap bmp)
        {
            bool result;
            byte[] dataWebP;
            bmp = null;

            try
            {
                //Read webP file
                dataWebP = File.ReadAllBytes(pathFileName);

                result = Decode(dataWebP, out bmp);

                return result;
            }
            catch (Exception ex) { Console.WriteLine(ex); return false; }
        }

        /// <summary>Decode a WebP image</summary>
        /// <param name="webpData">the data to uncompress</param>
        /// <param name="bmp">Bitmap whit the image</param>
        /// <returns>True if success; False otherwise</returns>
        public static bool Decode(byte[] webpData, out Bitmap bmp)
        {
            int imgWidth;
            int imgHeight;
            IntPtr outputBuffer;
            int outputBufferSize;
            bmp = null;

            try
            {
                //Get image width and height
                GCHandle pinnedWebP = GCHandle.Alloc(webpData, GCHandleType.Pinned);
                IntPtr ptrData = pinnedWebP.AddrOfPinnedObject();
                UInt32 dataSize = (uint)webpData.Length;
                if (WebPGetInfo(ptrData, dataSize, out imgWidth, out imgHeight) != 1) return false;

                //Create a BitmapData and Lock all pixels to be written
                bmp = new Bitmap(imgWidth, imgHeight, PixelFormat.Format24bppRgb);
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);

                //Allocate memory for uncompress image
                outputBufferSize = bmpData.Stride * imgHeight;
                outputBuffer = Marshal.AllocHGlobal(outputBufferSize);

                //Uncompress the image
                outputBuffer = WebPDecodeBGRInto(ptrData, dataSize, outputBuffer, outputBufferSize, bmpData.Stride);

                //Write image to bitmap using Marshal
                byte[] buffer = new byte[outputBufferSize];
                Marshal.Copy(outputBuffer, buffer, 0, outputBufferSize);
                Marshal.Copy(buffer, 0, bmpData.Scan0, outputBufferSize);

                //Write image to bitmap using CopyMemory. Faster than Marshall, but only work in windows
                //CopyMemory(bmpData.Scan0, outputBuffer, (uint)outputBufferSize);

                //Unlock the pixels
                bmp.UnlockBits(bmpData);

                //Free memory
                pinnedWebP.Free();
                Marshal.FreeHGlobal(outputBuffer);

                return true;
            }
            catch (Exception ex) { Console.WriteLine(ex); return false; }
        }
        
        /// <summary>Validate the WebP image header and retrieve the image height and width. Pointers *width and *height can be passed NULL if deemed irrelevant</summary>
        /// <param name="data">Pointer to WebP image data</param>
        /// <param name="data_size">This is the size of the memory block pointed to by data containing the image data</param>
        /// <param name="width">The range is limited currently from 1 to 16383</param>
        /// <param name="height">The range is limited currently from 1 to 16383</param>
        /// <returns>1 if success, otherwise error code returned in the case of (a) formatting error(s).</returns>
        [DllImport("libwebpdecoder.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int WebPGetInfo(IntPtr data, UInt32 data_size, out int width, out int height);

        /// <summary>Decode a WebP image pointed to by data</summary>
        /// <param name="data">Pointer to WebP image data</param>
        /// <param name="data_size">This is the size of the memory block pointed to by data containing the image data</param>
        /// <param name="width">The range is limited currently from 1 to 16383</param>
        /// <param name="height">The range is limited currently from 1 to 16383</param>
        /// <returns>output_buffer if function succeeds; NULL otherwise</returns>
        [DllImport("libwebpdecoder.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr WebPDecodeBGR(IntPtr data, UInt32 data_size, ref int width, ref int height);

        /// <summary>Decode WEBP image pointed to by *data and returns BGR samples into a pre-allocated buffer</summary>
        /// <param name="data">Pointer to WebP image data</param>
        /// <param name="data_size">This is the size of the memory block pointed to by data containing the image data</param>
        /// <param name="output_buffer">Pointer to decoded WebP image</param>
        /// <param name="output_buffer_size">Size of allocated buffer</param>
        /// <param name="output_stride">Specifies the distance between scanlines</param>
        /// <returns>output_buffer if function succeeds; NULL otherwise</returns>
        [DllImport("libwebpdecoder.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr WebPDecodeBGRInto(IntPtr data, UInt32 data_size, IntPtr output_buffer, int output_buffer_size, int output_stride);

    }
}