﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCall.Model
{
    
    
        public class ImagePacket
        {
            public string hash { get; set; } = string.Empty;
            public int len { get; set; } = 0;
            public string image { get; set; } = string.Empty;
            public ImagePacket() { }
            public ImagePacket(byte[] img_sources)
            {
                hash = ImageBox.StringHash(img_sources);
                len = img_sources.Length;
                image = ImageBox.EncodeBytes(img_sources);
            }
            public byte[] GetRawData()
            {
                byte[] data = ImageBox.DecodeBytes(image);

                if (data.Length != len) throw new Exception("Error data len");
                if (!ImageBox.StringHash(data).Equals(hash)) throw new Exception("Error hash");

                return data;
            }
        }

        public static class ImageBox
        {
            public static Image TakeScreen()
            {
                Bitmap bitmap = new Bitmap(110, 50);
                Graphics g = Graphics.FromImage(bitmap);
                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                return bitmap;
            }
       
            public static byte[] ImageToBytes(Image value)
            {
                ImageConverter converter = new ImageConverter();
                byte[] arr = (byte[])converter.ConvertTo(value, typeof(byte[]));
                return arr;
            }
          
     
            public static Image BytesToImage(byte[] value)
            {
                using (var ms = new MemoryStream(value))
                {
                    return Image.FromStream(ms);
                }
            }
          
            public static string EncodeBytes(byte[] value) => Convert.ToBase64String(value);
          
            public static byte[] DecodeBytes(string value) => Convert.FromBase64String(value);
        
            public static string StringHash(byte[] value)
            {
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] hashBytes = md5.ComputeHash(value);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }
                    return sb.ToString().ToLower();
                }
            }
        }
    
}
