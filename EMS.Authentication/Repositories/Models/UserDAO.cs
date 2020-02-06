using System;
using System.Collections.Generic;

namespace EMS.Authentication.Repositories.Models
{
    public partial class UserDAO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool IsAdmin { get; set; }
        public long? StudentId { get; set; }
        public string Email { get; set; }

        public virtual StudentDAO Student { get; set; }
    }
}
