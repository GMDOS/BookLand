using Npgsql;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace BookLand.Data
{
    public static class Pg
    {
        public static string? connectionString;
        public static NpgsqlConnection ConectarAoBanco()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        }
        public static async Task<T?> GetData<T>(NpgsqlDataReader reader) where T : new()
        {
            if (!reader.HasRows)
            {
                return default;
            }
            await reader.ReadAsync();
            T obj = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (reader.HasColumn(property.Name))
                {
                    int ordinal = reader.GetOrdinal(property.Name);
                    object value = reader.GetValue(ordinal);

                    if (value != DBNull.Value)
                    {
                        Type propertyType = property.PropertyType;

                        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            // Se a propriedade for uma lista genérica
                            Type listType = propertyType.GetGenericArguments()[0];
                            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType))!;

                            // Processar cada item na lista
                            if (listType == typeof(string))
                            {
                                string[] arrayValues = (string[])value;
                                foreach (string itemValue in arrayValues)
                                {
                                    list.Add(itemValue);
                                }
                            }

                            property.SetValue(obj, list);
                        } else if (propertyType == typeof(string))
                        {
                            // Se a propriedade for uma string
                            property.SetValue(obj, value.ToString());
                        } else
                        {
                            // Para propriedades simples
                            property.SetValue(obj, Convert.ChangeType(value, propertyType));
                        }
                    } else if (!property.PropertyType.IsGenericType)
                    {
                        // Se o valor for DBNull e a propriedade não for uma lista genérica,
                        // definimos o valor da propriedade como null
                        property.SetValue(obj, null);
                    }
                }
            }

            return obj;
        }

        public static bool HasColumn(this NpgsqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
