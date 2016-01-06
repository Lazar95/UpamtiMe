using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class Enumerations
    {
        public enum Categories
        {
            languages = 1,
            science,
            geography,
            general,
        }

        public enum LoginRegisterStatus
        {
            Successful,
            Failed,
            IncorrectPassword,
            EmailExists,
            UsernameExists
        }

        public enum InvalidCard
        {
            Duplicate, 
            EmptyField
        }

        public static void addCategories()
        {
            DataClasses1DataContext dc =  new DataClasses1DataContext();
            foreach (var c in Enum.GetValues(typeof (Categories)))
            {
                Category newC = new Category {name = c.ToString()};
                dc.Categories.InsertOnSubmit(newC);
            }
            dc.SubmitChanges();
        }

       
    }
}
