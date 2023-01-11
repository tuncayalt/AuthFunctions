using System.Collections.Generic;

namespace AuthFunctions.Domain.Models.Entities
{
    public class User : Entity
    {
        public User()
        {
            Roles = new List<Role>();
        }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public Profile Profile { get; set; }

        public virtual IList<Role> Roles { get; set; }
    }

    public class User<T> : User
    {
        public new T Id { get; set; }
    }
}
