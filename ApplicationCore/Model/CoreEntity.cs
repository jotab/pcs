using System;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Model
{
    public abstract class CoreEntity
    {
        [Key] 
        public Guid Id { get; set; }

        protected CoreEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}