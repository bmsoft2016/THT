using System;
using System.Collections.Generic;

namespace THT.Model.Models
{
    public class User : IBaseEnity
    {
        public User()
        {
            UserRoles = new List<UserRole>();
        }

        public int ID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedDate { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}