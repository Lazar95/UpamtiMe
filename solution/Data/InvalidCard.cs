using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class InvalidCard : Exception
    {
        public Enumerations.InvalidCard error { get; set; }

        public InvalidCard(Enumerations.InvalidCard error)
        {
            this.error = error;
        }

    }
}
