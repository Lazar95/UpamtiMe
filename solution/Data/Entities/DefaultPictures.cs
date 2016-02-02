using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class DefaultPictures
    {

        public static void uploadImage(int type, byte[] file)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            DefaultPicture dp = new DefaultPicture
            {
                type = type,
                image = new System.Data.Linq.Binary(file)
            };
            dc.DefaultPictures.InsertOnSubmit(dp);
            dc.SubmitChanges();
        }

        public static void removeImage(int imgID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            DefaultPicture dp = (from a in dc.DefaultPictures where a.defaultPictureID == imgID select a).First();
            dc.DefaultPictures.DeleteOnSubmit(dp);
            dc.SubmitChanges();
        }

        public static void editImage(int imgID, int type, byte[] file)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            DefaultPicture dp = (from a in dc.DefaultPictures where a.defaultPictureID == imgID select a).First();
            dp.image = new System.Data.Linq.Binary(file);
            dc.SubmitChanges();
        }

        public static List<DefaultPicture> getAll()
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return  (from a in dc.DefaultPictures select a).ToList();
        }

        public static int getRandom(int type)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.DefaultPictures where a.type == type orderby Guid.NewGuid() select a.defaultPictureID).First();
        }

        public static byte[] getAt(int imgID)
        {
            DataClasses1DataContext dc = new DataClasses1DataContext();
            return (from a in dc.DefaultPictures
                where a.defaultPictureID == imgID
                select a).First().image.ToArray();
        }
    }
}
