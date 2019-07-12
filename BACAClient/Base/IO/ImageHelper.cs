using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;
using Base.Ex;

namespace Base.IO
{
    public class ImageEx
    {
        /// <summary>
        /// 创建一个带水印的图片
        /// </summary>
        /// <param name="imagePath">原图片路径</param>
        /// <param name="waterMarkImgPath">水印图片路径</param>
        /// <param name="defaultPath">默认图片路径（如果原图片路径不存在时使用）</param>
        /// <returns></returns>
        public Bitmap CreateWaterMarkImage(string imagePath, string waterMarkImgPath, string defaultPath)
        {
            Bitmap bitmap = null;
            try
            {
                if (File.Exists(imagePath))
                {
                    Image cover = Image.FromFile(imagePath);
                    bitmap = new Bitmap(cover, 200, 280);
                    Image WaterMark = Image.FromFile(waterMarkImgPath);
                    Graphics g = Graphics.FromImage(bitmap);
                    //g.DrawImage(WaterMark, Cover.Width - WaterMark.Width, Cover.Height - WaterMark.Height, WaterMark.Width, WaterMark.Height);
                    g.DrawImage(WaterMark, new Rectangle(bitmap.Width - WaterMark.Width, bitmap.Height - WaterMark.Height, WaterMark.Width, WaterMark.Height), 0, 0, WaterMark.Width, WaterMark.Height, GraphicsUnit.Pixel);
                    g.Dispose();
                    WaterMark.Dispose();
                    cover.Dispose();
                }
                else
                {
                    //否则就显示默认图片
                    bitmap = Image.FromFile(defaultPath) as Bitmap;
                }
            }
            catch (Exception ex)
            {
                Loger.Log(Assembly.GetExecutingAssembly().FullName, MethodBase.GetCurrentMethod().FullName(), ex);
            }
            return bitmap;
        }
    }
}
