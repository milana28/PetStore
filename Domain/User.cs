using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PetStore.Domain
{
    public interface IUser
    {
        Models.User SetUser(Models.User user);
        Models.User GetUserByUsername(string username);
        Models.User UpdateUser(string username, Models.User user);
    }
    
    public class User : IUser
    {
        private readonly List<Models.User> _users = new List<Models.User>();
        
        public Models.User SetUser(Models.User user)
        {
            _users.Add(user);
            return user;
        }

        public Models.User GetUserByUsername(string username)
        {
           return _users.Find(u => u.Username == username);
        }

        public Models.User UpdateUser(string username, Models.User user)
        {
            var index = _users.FindIndex(u => u.Username == username);
            if (index != -1)
            {
                _users[index] = user;
            }

            return _users[index];
        }
        
    }
}