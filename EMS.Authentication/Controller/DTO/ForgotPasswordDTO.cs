using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF.Authentication.Commons;

namespace TF.Authentication.Controller.DTO
{
    public class ForgotPasswordDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
