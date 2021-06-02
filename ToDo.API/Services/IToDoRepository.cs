using System.Collections.Generic;

namespace ToDo.API.Services
{
    public interface IToDoRepository
    {
        IEnumerable<Entities.ToDo> GetToDo();
        Entities.ToDo GetToDoItem(int id);

        Entities.ToDo AddToDoItem(Entities.ToDo toDo);

        bool Save();

        void DeleteToDoItem(Entities.ToDo toDo);
    }
}