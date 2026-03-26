using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkillBuddy.Entity;
using SkillBuddy.data;

namespace SkillBuddy.BLL
{
    public class UserManager
    {
        UserDataAccess da = new UserDataAccess();

        public async Task<List<ApplicationUser>> Search(ApplicationUser user)
        {
            return await da.SearchAsync(user);
        }

        //     public async Task<List<ApplicationUser>> UserSearch(ApplicationUserSearch user)
        //     {
        //         return await da.UserSearch(user);
        //     }

        //     public async Task<Guid?> Save(ApplicationUser user)
        //     {
        //         if (user == null)
        //             return null;

        //         return await da.Save(user);
        //     }

        //     public List<ApplicationUser> GetAllUsersByID(Guid RoleGroupID)
        //     {
        //         return da.GetAllUsersByID(RoleGroupID);
        //     }

        //     public static bool CheckPasswordAsync(ApplicationUser user, string password)
        //     {
        //         if (String.IsNullOrEmpty(user.PasswordHash)
        //             || string.IsNullOrEmpty(password)
        //         )
        //         {
        //             return false;
        //         }

        //         Cryptography cryptography = new Cryptography("key");
        //         //.Decrypt(user.PasswordHash)
        //         password = cryptography.Encrypt(password);

        //         return password.Equals(user.PasswordHash, StringComparison.Ordinal);
        //     }
    }
}