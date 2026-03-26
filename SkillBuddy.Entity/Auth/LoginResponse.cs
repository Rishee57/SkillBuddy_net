using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBuddy.Entity.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime ExpierAt { get; set; }
    }
}
