using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF.Authentication.Commons
{
    public interface ICurrentContext : IServiceScoped
    {
        Guid UserId { get; set; }
        string Username { get; set; }
        Guid StudentId { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Phone { get; set; }
        bool Gender { get; set; }
        bool IsAdmin { get; set; }
    }
    public class CurrentContext : ICurrentContext
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public Guid StudentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Gender { get; set; }
        public bool IsAdmin { get; set; }
    }
}
