using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Model
{
    public class User : CoreEntity
    {
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }

        [NotMapped] 
        public ICollection<Role> Roles { get; set; }
    }
}