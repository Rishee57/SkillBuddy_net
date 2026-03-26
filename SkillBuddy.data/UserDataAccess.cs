using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SkillBuddy.data.DataBaseManager;
using SkillBuddy.Entity;

namespace SkillBuddy.data
{
  public class UserDataAccess
  {
    public UserDataAccess()
    {
    }

    // Minimal stubs — implement real data access as needed
    public Task<List<ApplicationUser>> SearchAsync(ApplicationUser user)
    {
            DataManager dm =new DataManager("SP_GET_User");
            dm.AddVarcharPara("pEmail" ,50 , user.Email ?? string.Empty);
      return Task.FromResult(new List<ApplicationUser>());
    }
  }
}
