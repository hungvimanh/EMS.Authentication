using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF.Authentication.Commons
{
    public class AppSettings
    {
        public string JWTSecret { get; set; }
        public int JWTLifeTime { get; set; }
    }
}
