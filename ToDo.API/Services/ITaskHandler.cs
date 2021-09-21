using ToDo.API.Models;

namespace ToDo.API.Services
{
    public interface ITaskHandler
    {
        void CompleteTask(TaskCompletedDto taskCompleted);
        void InProgressTask(TaskInProgressDto taskInProgress);
    }
}