using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data;

namespace UpamtiMe.Models
{
    public class DefaultImage
    {
        public byte[] Image { get; set; }
        public int Type { get; set; }
        public int ImgID { get; set; }

        public static List<DefaultImage> convert(List<Data.DefaultPicture> list)
        {
            List<DefaultImage> returnValue = new List<DefaultImage>();
            foreach (DefaultPicture dp in list)
            {
                returnValue.Add(new DefaultImage {Image = dp.image.ToArray(), Type = dp.type, ImgID = dp.defaultPictureID});
            }
            return returnValue;
        }
    }
}