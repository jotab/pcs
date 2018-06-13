using System.Collections.Generic;

namespace Infrastructure.Mapping
{
    public interface IMappingCache
    {
        IEntityMapping GetEntityMap<T>();
    }
}