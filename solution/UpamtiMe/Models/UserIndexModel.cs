using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Data.DTOs;

namespace UpamtiMe.Models
{
    public class UserIndexModel
    {
        public RegisterTransporterDTO Register { get; set; }
        public LoginTransporterDTO Login { get; set; }
    }
}