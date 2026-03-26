using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Newtonsoft.Json;
namespace SkillBuddy.Entity
{


    public class ApplicationUser : IdentityUser<Guid>
    {
        [JsonProperty("id")]
        public new Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("last_name", NullValueHandling = NullValueHandling.Ignore)]
        public string? LastName { get; set; }

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string? Username { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; } = null!;

        [JsonProperty("role_id")]
        public Guid? RoleId { get; set; }

        [JsonProperty("is_customer")]
        public bool IsCustomer { get; set; } = false;

        [JsonProperty("mobile_verified_at")]
        public DateTime? MobileVerifiedAt { get; set; }

        [JsonProperty("email")]
        public new string? Email { get; set; }

        [JsonProperty("is_verified")]
        public bool IsVerified { get; set; } = false;

        [JsonProperty("verify_code")]
        public string? VerifyCode { get; set; }

        [JsonProperty("email_verified_at")]
        public DateTime? EmailVerifiedAt { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; } = null!;

        [JsonProperty("notification_preference")]
        public string NotificationPreference { get; set; } = "mail";

        [JsonProperty("is_active")]
        public bool IsActive { get; set; } = true;

        [JsonProperty("avatar")]
        public string? Avatar { get; set; }

        [JsonProperty("image")]
        public string? Image { get; set; }

        [JsonProperty("slug")]
        public string? Slug { get; set; }

        [JsonProperty("phone")]
        public string? Phone { get; set; }

        [JsonProperty("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("secret_login")]
        public bool SecretLogin { get; set; } = false;

        [JsonProperty("lang_code")]
        public string LangCode { get; set; } = "en";

        [JsonProperty("currency_id")]
        public Guid? CurrencyId { get; set; }

        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; } = "INR";

        [JsonProperty("commision_type_id")]
        public int? CommisionTypeId { get; set; }

        [JsonProperty("commision_rate")]
        public float? CommisionRate { get; set; }

        [JsonProperty("remember_token")]
        public string? RememberToken { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("others")]
        public string? Others { get; set; }
    }
    public class ApplicationUserSearch : ApplicationUser
    {
        public new List<Guid> Id { get; set; }
        public new List<Guid> UserID1 { get; set; }
        public string? Token { get; set; }
    }
}