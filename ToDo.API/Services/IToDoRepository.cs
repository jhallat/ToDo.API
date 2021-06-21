using System.Collections.Generic;

namespace ToDo.API.Services
{
    public interface IToDoRepository
    {
        IEnumerable<Entities.ToDo> GetToDo();

        IEnumerable<Entities.ToDo> GetToDoByTimestamp(string timestamp);

        public IEnumerable<Entities.ToDo> GetToDoByCompleted(bool completed);
        
        Entities.ToDo GetToDoItem(int id);

        Entities.ToDo AddToDoItem(Entities.ToDo toDo);

        bool Save();

        void DeleteToDoItem(Entities.ToDo toDo);
    }
}