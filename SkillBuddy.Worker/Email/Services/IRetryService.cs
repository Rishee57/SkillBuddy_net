using System;
using System.Threading.Tasks;

namespace SkillBuddy.Worker.Email.Services
{
    public interface IRetryService
    {
        Task ExecuteAsync(Func<Task> action);
    }
}
