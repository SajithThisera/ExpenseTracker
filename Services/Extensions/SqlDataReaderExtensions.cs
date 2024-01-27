using System.Data.SqlClient;
using System.Data;

namespace ExpenseTracker.Services.Extensions
{
    public static class SqlDataReaderExtensions
    {
        public static T Get<T>(this SqlDataReader reader, string columnName)
        {
            if (reader.IsDBNull(columnName))
                return default;
            return reader.GetFieldValue<T>(columnName);
        }

        public static T Get<T>(this SqlDataReader reader, int columnOrdinal)
        {
            if (reader.IsDBNull(columnOrdinal))
                return default;
            return reader.GetFieldValue<T>(columnOrdinal);
        }
    }
}
