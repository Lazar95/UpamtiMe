using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DTOs;

namespace Data.Entities
{


    public class Login
    {
        public static LoginDTO CreateLoginDTO(LoginTransporterDTO ltd)
        {
            User usr = Users.Login(ltd.Username, ltd.Password);
            LoginDTO ld = new LoginDTO();
            if (usr == null)
            {
                if (Users.checkUsername(ltd.Username))
                {
                    ld.LoginRegisterStatus = Enumerations.LoginRegisterStatus.IncorrectPassword;
                }
                else
                {
                    ld.LoginRegisterStatus = Enumerations.LoginRegisterStatus.Failed;
                }

                return ld;
            }

            
            ld.LoginRegisterStatus = Enumerations.LoginRegisterStatus.Successful;
            ld.Username = usr.username;
            ld.FirstName = usr.name;
            ld.LastName = usr.surname;
            ld.UserID = usr.userID;
            ld.RememberMe = ltd.RememberMe;

            return ld;
        }

        public static LoginDTO CreateRegisterLoginDTO(RegisterTransporterDTO rtd)
        {
            LoginDTO ld = new LoginDTO();
            if (Users.checkUsername(rtd.Username))
            {
                ld.LoginRegisterStatus = Enumerations.LoginRegisterStatus.UsernameExists;
                return ld;
            }
            if (Users.checkEmail(rtd.Email))
            {
                ld.LoginRegisterStatus = Enumerations.LoginRegisterStatus.EmailExists;
                return ld;
            }

            User usr = Users.addUser(rtd.Name, rtd.Username, rtd.Password, rtd.Email);

            ld.LoginRegisterStatus = Enumerations.LoginRegisterStatus.Successful;
            ld.Username = usr.username;
            ld.FirstName = usr.name;
            ld.LastName = usr.surname;
            ld.UserID = usr.userID;

            return ld;
        }
    }
}
