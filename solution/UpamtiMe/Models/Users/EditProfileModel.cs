using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpamtiMe.Models
{
    public class EditProfileModel
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public byte[] Image { get; set; }

        public static EditProfileModel Load(int userID)
        {
            Data.User u = Data.Users.GetUser(userID);
            EditProfileModel returnValue = new EditProfileModel
            {
                UserID = u.userID,
                Username = u.username,
                Name = u.name,
                Surname = u.surname,
                Location = u.location,
                Bio = u.bio,
                Image = u.avatar?.ToArray(),
            };

            if (returnValue.Image == null || returnValue.Image.Length == 0)
            {
                returnValue.Image = Data.DefaultPictures.getAt(u.defaultAvatarID);
            }

            return returnValue;
        }
    }

   
    
}