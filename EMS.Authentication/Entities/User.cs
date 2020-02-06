using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF.Authentication.Commons;

namespace TF.Authentication.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public long? StudentId { get; set; }
        public bool IsAdmin { get; set; }
        public string Salt { get; set; }
        public string Jwt { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public bool FirstLogin { get; set; }
    }

    public class UserFilter
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool? IsAdmin { get; set; }
    }
}
