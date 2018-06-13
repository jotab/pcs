using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using ApplicationCore.Interfaces;
using ApplicationCore.Model;
using Dapper;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
using Infrastructure.Mapping;
using Oracle.ManagedDataAccess.Client;

namespace Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : CoreEntity
    {
        private readonly IDbConnection _connection;
        internal readonly IEntityMapping Mapping;

        public Repository(IDbConnection connection, IMappingCache mappingCache)
        {
            _connection = connection;
            Mapping = mappingCache.GetEntityMap<T>();
        }

        public T GetById(Guid id)
        {
            var command = Mapping.GetSelectSql();
            var pkProperty = Mapping.Pks.First();
            var param = new Dictionary<string, object> {{$"@{pkProperty.PropertyName}", id}};

            command +=
                $"{SqlTerm.Where} {pkProperty.ColumnName} = :{pkProperty.PropertyName} {SqlTerm.And} {SqlTerm.RowNum} = 1";

            return _connection.QuerySingleOrDefault<T>(command, param);
        }

        public IEnumerable<T> List()
        {
            return _connection.Query<T>(Mapping.GetSelectSql());
        }

        public IEnumerable<T> List(Expression<Func<T, bool>> expression)
        {
            return List(Mapping.LinqToSqlWhere(expression));
        }

        public IEnumerable<T> List(ISpecification<T> spec)
        {
            var entityList = List(Mapping.LinqToSqlWhere(spec.Criteria));
            if (spec.Includes.Any())
            {
                //TODO - Implement includes
            }

            return entityList;
        }

        public void Add(T entity)
        {
            AddOrUpdate(entity, Mapping.GetInsertSql());
        }

        public void Delete(T entity)
        {
            var deleteSql = Mapping.GetDeleteSql();
            var command = _connection.CreateCommand();
            if (command is OracleCommand oracleCmd)
            {
                oracleCmd.BindByName = true;
            }

            var propertiesForCondition = Mapping.Pks.Any() ? Mapping.Pks : Mapping.ValueProperties;

            foreach (var pk in propertiesForCondition)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = pk.PropertyName;
                parameter.Value = Mapping.GetPropertyValue(pk);
                command.Parameters.Add(parameter);

                if (deleteSql.Contains(SqlTerm.Where))
                {
                    deleteSql += $"{SqlTerm.And} {pk.ColumnName} = :{pk.PropertyName} ";
                }
                else
                {
                    deleteSql += $"{SqlTerm.Where} {pk.ColumnName} = :{pk.PropertyName} ";
                }
            }

            command.CommandText = deleteSql;
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void Update(T entity)
        {
            AddOrUpdate(entity, Mapping.GetUpdateSql());
        }

        private IEnumerable<T> List(ISqlCommandMapping cmdMapping)
        {
            var command = string.Concat(Mapping.GetSelectSql(), cmdMapping.CommandText);

            return _connection.Query<T>(command, cmdMapping.Parameters);
        }

        private void AddOrUpdate(T entity, string commandText)
        {
            var command = _connection.CreateCommand();
            if (command is OracleCommand oracleCmd)
            {
                oracleCmd.BindByName = true;
            }

            foreach (var propertyMap in Mapping.ValueProperties)
            {
                if (propertyMap.IsDbGenerated)
                {
                    if (!commandText.Contains(SqlTerm.Returning))
                        commandText += $"{SqlTerm.Returning} ";

                    commandText += $"{propertyMap.ColumnName} {SqlTerm.Into} :{propertyMap.PropertyName} ";
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = propertyMap.PropertyName;
                    parameter.Value = entity.GetPropertyValue(propertyMap);
                    parameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(parameter);
                }
                else
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = propertyMap.PropertyName;
                    parameter.Value = entity.GetPropertyValue(propertyMap);
                    command.Parameters.Add(parameter);
                }
            }

            command.CommandText = commandText;
            command.ExecuteNonQuery();

            foreach (var propertyMap in Mapping.Properties.Where(mapping => mapping.IsDbGenerated))
            {
                var outputValue = command.Parameters.Cast<IDataParameter>()
                    .Single(parameter => parameter.ParameterName.Equals(propertyMap.PropertyName));
                entity.SetPropertyValue(propertyMap, outputValue.Value);
            }

            command.Dispose();
        }
    }
}