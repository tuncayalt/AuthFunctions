using System;

namespace AuthFunctions.Domain.Models.Entities
{
    public class Entity : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class Entity<T> : Entity, IEntity
    {
        public new T Id { get; set; }
    }
}
