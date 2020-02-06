using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF.Authentication.Controller.DTO
{
    public class LoginResultDTO
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public bool FirstLogin { get; set; }
        public bool IsAdmin { get; set; }
    }
}
