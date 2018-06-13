using System.Reflection;

namespace Infrastructure.Mapping
{
    public interface IPropertyMapping
    {
        string PropertyName { get; }
        string ColumnName { get; }
        bool IsPk { get; }
        bool IsFk { get; }
        bool IsDbGenerated { get; }
        bool IsNavigation { get; }
        string RelatedProperty { get; }
        PropertyInfo PropertyInfo { get; }
    }
}