using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace FrameCommon;

/// <summary>
/// 基础帮助类
/// </summary>
public static class BaseHelper
{
    /// <summary>
    /// 当前时间Day转星期
    /// </summary>
    /// <param name="day">当前时间的Day</param>
    /// <returns>星期</returns>
    public static string DayToWeek(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday:
                return "星期一";
            case DayOfWeek.Tuesday:
                return "星期二";
            case DayOfWeek.Wednesday:
                return "星期三";
            case DayOfWeek.Thursday:
                return "星期四";
            case DayOfWeek.Friday:
                return "星期五";
            case DayOfWeek.Saturday:
                return "星期六";
            case DayOfWeek.Sunday:
                return "星期日";
            default:
                return "";
        }
    }

    /// <summary>
    /// 将文件保存到本地
    /// <param name="url">网络地址</param>
    /// <param name="fileFullName">保存路径</param>
    /// <param name="fileName">保存文件名</param>
    /// </summary>
    public static void UrlSaveLocal(string url, string fileFullName, string fileName)
    {
        byte[] b;
        HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
        WebResponse myResp = myReq.GetResponse();
        Stream stream = myResp.GetResponseStream();
        using (BinaryReader br = new BinaryReader(stream))
        {
            b = br.ReadBytes(9000000);
            br.Close();
        }
        myResp.Close();
        if (!Directory.Exists(fileFullName))
        {
            Directory.CreateDirectory(fileFullName);
        }
        FileStream fs = new FileStream(fileFullName + fileName, FileMode.Create);
        BinaryWriter w = new BinaryWriter(fs);
        try
        {
            w.Write(b);
        }
        finally
        {
            fs.Close();
            w.Close();
        }
    }

    /// <summary>
    /// 本地图片转Base64
    /// </summary>
    /// <param name="fileFullName">图片路径</param>
    /// <returns></returns>
    public static string LocalImageToBase64(string fileFullName)
    {
        using (Bitmap bmp = new Bitmap(fileFullName))
        {
            MemoryStream ms = new MemoryStream();
            string suffix = fileFullName.Substring(fileFullName.LastIndexOf('.') + 1,
                fileFullName.Length - fileFullName.LastIndexOf('.') - 1).ToLower();

            ImageFormat suffixName;
            switch (suffix)
            {
                case "png":
                    suffixName = ImageFormat.Png;
                    break;

                case "jpg":
                    suffixName = ImageFormat.Jpeg;
                    break;

                case "jpeg":
                    suffixName = ImageFormat.Jpeg;
                    break;

                case "bmp":
                    suffixName = ImageFormat.Bmp;
                    break;

                case "gif":
                    suffixName = ImageFormat.Gif;
                    break;

                default:
                    suffixName = ImageFormat.Jpeg;
                    break;
            }
            bmp.Save(ms, suffixName);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            string base64 = Convert.ToBase64String(arr);
            return base64;
        }
    }

    /// <summary>
    /// 网络图片转Base64
    /// </summary>
    /// <param name="url">图片地址</param>
    /// <returns></returns>
    public static string UrlImageToBase64(string url)
    {
        #region MyRegion
        //Uri url1 = new Uri(url);
        //WebRequest webRequest = WebRequest.Create(url1);
        //WebResponse webResponse = webRequest.GetResponse();
        //Image image = Image.FromStream(webResponse.GetResponseStream());
        //if (image == null)
        //{
        //    return null;
        //}
        //Bitmap myImage = new Bitmap(image);
        //MemoryStream ms = new MemoryStream();
        //myImage.Save(ms, image.RawFormat);
        //byte[] arr = new byte[ms.Length];
        //ms.Position = 0;
        //ms.Read(arr, 0, (int)ms.Length);
        //ms.Close();
        //var base64 = Convert.ToBase64String(arr);
        #endregion

        byte[] Bytes = new WebClient().DownloadData(url);
        Image image;
        using (MemoryStream ms1 = new MemoryStream(Bytes))
        {
            Image outputImg = Image.FromStream(ms1);
            image = outputImg;
        }
        Bitmap myImage = new Bitmap(image);
        MemoryStream ms = new MemoryStream();
        myImage.Save(ms, image.RawFormat);
        byte[] arr = new byte[ms.Length];
        ms.Position = 0;
        ms.Read(arr, 0, (int)ms.Length);
        ms.Close();
        var base64 = Convert.ToBase64String(arr);
        return base64;
    }

}
