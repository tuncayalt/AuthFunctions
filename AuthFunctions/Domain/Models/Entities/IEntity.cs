using System;

namespace AuthFunctions.Domain.Models.Entities
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }

    public interface IEntity<T> : IEntity
    {
        new T Id { get; set; }
    }
}
