using System.Collections.Generic;

namespace Infrastructure.Mapping
{
    internal interface ISqlCommandMapping
    {
        string CommandText { get; set; }
        IDictionary<string, object> Parameters { get; set; }
    }
}