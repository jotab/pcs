using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Model
{
    public class Role : CoreEntity
    {
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        
        [NotMapped]
        public ICollection<User> Users { get; set; }
    }
}