using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ApplicationCore.Model
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class CoreEntity
    {
        [Key] public string Id { get; private set; }

        [NotMapped] public Guid GuidId => Guid.Parse(Id);

        protected CoreEntity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}