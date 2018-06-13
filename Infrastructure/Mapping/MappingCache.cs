using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Infrastructure.Extensions;
using Microsoft.Extensions.DependencyModel;

namespace Infrastructure.Mapping
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class MappingCache : IMappingCache
    {
        public MappingCache()
        {
            Mappings = GetAllModelTypes().Select(type => type.CreateEntityMapping());
        }

        private IEnumerable<IEntityMapping> Mappings { get; }

        private static IEnumerable<Type> GetAllModelTypes()
        {
            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

            var typer = runtimeAssemblyNames
                .Select(Assembly.Load)
                .SelectMany(a => a.ExportedTypes)
                .Where(t => !t.IsAbstract && !t.IsInterface && t.Namespace.Equals("ApplicationCore.Model"))
                .ToList();

            return typer;
        }

        public IEntityMapping GetEntityMap<T>()
        {
            return Mappings.SingleOrDefault(mapping => mapping.Name.Equals(typeof(T).Name));
        }
    }
}