using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBuddy.Entity.Auth
{
    public class LoginRequest
    {
        public int? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int? Otp { get; set; }

    }
}
