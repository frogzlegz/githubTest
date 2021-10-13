using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
namespace SQL_Interface_Test._classes
    //Checkin
{
    public static class clsDB
    {
        public class DatabaseConnection
        {
            public ConnectionType connectionType;
            public string connectionString;
            public DbConnection connection;

            public DatabaseConnection(ConnectionType connectionType, string connectionString)
            {
                this.connectionType = connectionType;
                this.connectionString = connectionString;
            }

            public DbConnection Connect()
            {
                if (connection != null)
                {
                    connection.Close();
                    connection = null;
                }

                switch (connectionType)
                {
                    default:
                    case ConnectionType.MySql:
                        {
                            connection = new MySqlConnection(connectionString);
                            break;
                        }
                    case ConnectionType.Local:
                    case ConnectionType.VS:
                        {
                            connection = new SqlConnection(connectionString);
                            break;
                        }
                }

                return connection;
            }

            public DbConnection OpenConnection()
            {
                try
                {
                    DbConnection connection = CurrentConnection.Connect();
                    connection.Open();
                    return connection;
                }
                catch (SqlException e)
                {
                    MainWindow.UpdateMessage(e.Message);
                }

                return null;
            }

            public DbCommand CreateCommand(string sql)
            {
                switch (this.connectionType)
                {
                    default:
                    case ConnectionType.MySql:
                        return new MySqlCommand(sql, (MySqlConnection)connection);
                    case ConnectionType.Local:
                    case ConnectionType.VS:
                        return new SqlCommand(sql, (SqlConnection)connection);
                }
            }
        }

        public enum ConnectionType {MySql, Local, VS}

        // DB names
        public static string nm_DB_Local = "db_Local";
        public static string nm_DB_VS = "db_local";
        public static string nm_DB_Current = nm_DB_VS;


        // Extra step for VS DB. Not required for Local

        // public static string dir_DB_VS = "C:\\Users\\boggl_000\\source\\repos\\SQL Interface Test\\SQL Interface Test\\_data\\" + nm_DB_Current + ".mdf";
        public static string dir_DB_VS = "H:\\My Documents\\Development\\Year_2\\VS2019\\SQL Interface Test\\SQL Interface Test\\_data\\" + nm_DB_Current + ".mdf"; 

        // Path Definitions
        public static DatabaseConnection PathDB_VS = new DatabaseConnection(ConnectionType.Local, "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"" + dir_DB_VS + "\";Integrated Security=True");
        public static DatabaseConnection PathDB_Local = new DatabaseConnection(ConnectionType.Local, "Data Source=(localdb)\\ProjectsV13;Initial Catalog=" + nm_DB_Current + ";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        public static DatabaseConnection PathDB_MySQL = new DatabaseConnection (ConnectionType.MySql, "server=127.0.0.1;uid=root;pwd=Gibson111!;persistsecurityinfo=True;database=msql_DB_1");

        // Current path/DB
        public static string Path_Current;
        public static string DB_Current;
        public static DatabaseConnection CurrentConnection = PathDB_VS;

        public static ConnectionType Connection_Type = ConnectionType.MySql;

        public static DataTable Get_DataTable(string tableName)
        {                        
            if (CurrentConnection.connection == null || CurrentConnection.connection.State != ConnectionState.Open)
                return null;

            DbCommand command = CurrentConnection.CreateCommand("SELECT * FROM " + tableName);

            DbDataReader reader1 = command.ExecuteReader();

            DataTable table = new DataTable();

            if (reader1.Read())
            {
                table.Load(reader1);
            }

            reader1.Close();

            return table;            
        }

        public static int Get_Last_ID(string tableName)
        {
            int lastID = 0;

            DbCommand command = CurrentConnection.CreateCommand("SELECT MAX(ID) FROM " + tableName);

            DbDataReader reader1;
            try
            {
                reader1 = command.ExecuteReader();

                if (reader1.Read())
                {
                    try
                    {
                        lastID = ((IDataRecord)reader1).GetInt32(0);
                    }
                    catch (System.Data.SqlTypes.SqlNullValueException e)
                    {
                        MainWindow.UpdateMessage(e.Message);
                    }
                }

                reader1.Close();
            }
            catch (SqlException e)
            {
                MainWindow.UpdateMessage(e.Message);
            }

            return lastID;
        }
        private static void ReadSingleRow(IDataRecord record)
        {
            Console.WriteLine(String.Format("{0}, {1}", record[0], record[1]));
        }

        public static void Execute_SQL(string SQL_Text)
        {
            CurrentConnection.OpenConnection();
            DbCommand command = CurrentConnection.CreateCommand(SQL_Text);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                MainWindow.UpdateMessage(e.Message);
            }
        }

        public static DataSet Retrieve_Data(string SQL_Text)
        {
            DbCommand command = CurrentConnection.CreateCommand(SQL_Text);

            DbDataReader reader1;
            reader1 = command.ExecuteReader();

            reader1.Close();

            return null;
        }
    }
}