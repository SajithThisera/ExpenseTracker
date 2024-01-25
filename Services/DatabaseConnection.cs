using System.Data.SqlClient;

namespace ExpenseTracker.Services
{
    public sealed class DatabaseConnection
    {
        private static DatabaseConnection? instance = null;
        public SqlConnection connection;

        private DatabaseConnection() 
        {
            Init();
        }

        public static DatabaseConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseConnection();
                }
                return instance;
            }
        }

        private void Init()
        {
            const string connectionString = "Server=localhost\\SQLEXPRESS;Database=trackerdb;Trusted_Connection=True;";

            connection = new SqlConnection(connectionString);
        }

        public void RunQuery(String query, SqlConnection connection)
        {

        }
    }
}
