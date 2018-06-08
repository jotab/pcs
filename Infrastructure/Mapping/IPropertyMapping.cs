namespace Infrastructure.Mapping
{
    internal interface IPropertyMapping
    {
        string PropertyName { get; }
        string ColumnName { get; }
        bool IsPk { get; }
        bool IsFk { get; }
        bool IsDbGenerated { get; }
    }
}