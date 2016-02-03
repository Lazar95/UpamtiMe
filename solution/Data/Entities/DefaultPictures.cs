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
            //svima koji su imali tu defaultnu sliku svlja neku drugu
            var newDefaultPicture =
                (from a in dc.DefaultPictures where a.defaultPictureID != imgID && a.type == dp.type select a.defaultPictureID);
            int newDefaultPictureID;
            if (newDefaultPicture.Any())
            {
                newDefaultPictureID = newDefaultPicture.First();
            }
            else
            {
                return;//ne moze da obrise tu defaultnu ako ne postoji druga koja ce da je zameni
            }

            if (dp.type == (int)Enumerations.DefaultPicture.Avatar)
            {
                List<User> users = (from a in dc.Users select a).ToList();
                foreach (User user in users)
                {
                    if (user.defaultAvatarID == imgID)
                        user.defaultAvatarID = newDefaultPictureID;
                }
            }
            else if (dp.type == (int) Enumerations.DefaultPicture.Course)
            {
                List<Course> courses = (from a in dc.Courses select a).ToList();
                foreach (Course course in courses)
                {
                    if (course.defaultImageID == imgID)
                        course.defaultImageID = newDefaultPictureID;
                }
            }
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
            return (from a in dc.DefaultPictures where a.type == type select a.defaultPictureID).ToList().OrderBy(x => Guid.NewGuid()).First();
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
