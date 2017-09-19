using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MemoryGameService
{
    public class DBController
    {
        private static MySqlConnection DBConnection;
        private static MySqlConnection AsyncDBConnection;
        //singleton instance
        private static volatile DBController instance = null;
        private static readonly object syncRoot = new Object();
        public static DBController Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DBController();
                    }
                }
                return instance;
            }
        }
        public string serverName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string DB_Name { get; set; }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int FreeConsole();
        private DBController()
        {
            DBConnection = new MySqlConnection();
            AsyncDBConnection = new MySqlConnection();
            //Try Remote DB Connection
            serverName = "sql8.freemysqlhosting.net";
            userName = "sql8117524";
            password = "JdGdIWEaG7";
            DB_Name = "sql8117524";
            if(!connect())
            {
                //Try Another Remote DB Connection
                serverName = "chumsteam.ddns.net";
                userName = "chumsteam";
                password = "ch@m$te@m";
                DB_Name = "playonline";
                if (!connect())
                {
                    //Try LocalNetwork DB Connection
                    serverName = "192.168.1.2"; //Host Local IP
                    userName = "chumsteam";
                    password = "ch@m$te@m";
                    DB_Name = "playonline";
                    if (!connect())
                    {
                        //Try Localhost DB Connection
                        serverName = "127.0.0.1";
                        userName = "root";
                        password = "lionman66";
                        DB_Name = "playonline";
                        if (!connect())
                        {
                            //Try Default Localhost DB Connection
                            serverName = "127.0.0.1";
                            userName = "root";
                            password = "";
                            DB_Name = "playonline";
                            if (!connect())
                            {
                                AllocConsole();
                                do
                                {
                                    Console.Clear();
                                    Console.WriteLine(" >>> Failed To Connect To Any Database! <<< ");
                                    Console.Write("\n\nEnter Server Name/IP >");
                                    serverName = Console.ReadLine();
                                    Console.Write("Enter User Name >");
                                    userName = Console.ReadLine();
                                    Console.Write("Enter Password >");
                                    password = Console.ReadLine();
                                    Console.Write("Enter Database Name >");
                                    DB_Name = Console.ReadLine();
                                }
                                while (!connect());
                                FreeConsole();
                            }
                        }
                    }
                }
            }
        }
        public bool connect()
        {
            DBConnection.ConnectionString =
                "server=" + serverName + ";" +
                "uid=" + userName + ";" +
                "pwd=" + password + ";" +
                "database=" + DB_Name + ";";
            AsyncDBConnection.ConnectionString =
                "server=" + serverName + ";" +
                "uid=" + userName + ";" +
                "pwd=" + password + ";" +
                "database=" + DB_Name + ";";
            try
            {
                DBConnection.Open();
                //Thread.Sleep(1000);
                AsyncDBConnection.Open();
                return true;
            }
            catch (MySql.Data.MySqlClient.MySqlException)
            {
                return false;
            }
        }
        public bool disconnect() 
        {
            try
            {
                DBConnection.Close();
                AsyncDBConnection.Close();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }
        public bool add(string tableName, string commaSeparatedColumnNames,
            string commaSeparatedValues)
        {
            string query = "INSERT INTO " + tableName + " (" + commaSeparatedColumnNames + ") VALUES (" + commaSeparatedValues + ")";
            //check connection
            if (DBConnection.State == System.Data.ConnectionState.Open)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, DBConnection);
                //Execute command
                cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }
        public bool update(string tableName, string columnName,
            string columnValue, string constraintName,
            string constraintValue)
        {
            string query = "UPDATE " + tableName + " SET " + columnName + "= '" + columnValue + "' WHERE " + constraintName + "=" + "'" + constraintValue + "'";

            //Open connection
            if (DBConnection.State == System.Data.ConnectionState.Open)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = DBConnection;
                //Execute query
                cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }
        public bool update(string query)
        {
            //Open connection
            if (DBConnection.State == System.Data.ConnectionState.Open)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = DBConnection;
                //Execute query
                cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }
        public bool delete(string tableName, string constraintName,
             string constraintValue)
        {
            string query = "DELETE FROM " + tableName + " WHERE " + constraintName + "='" + constraintValue + "'";

            if (DBConnection.State == System.Data.ConnectionState.Open)
            {
                MySqlCommand cmd = new MySqlCommand(query, DBConnection);
                cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }
        public bool delete(string query)
        {
            if (DBConnection.State == System.Data.ConnectionState.Open)
            {
                MySqlCommand cmd = new MySqlCommand(query, DBConnection);
                cmd.ExecuteNonQuery();
                return true;
            }
            return false;
        }
        public MySqlDataReader retreiveDate(string Query)
        {
            if (DBConnection.State == System.Data.ConnectionState.Open)
            {
                //Try - Catch To Avoid SQL Injection!
                try
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(Query, DBConnection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    return dataReader;
                }
                catch(Exception)
                {
                    return null;
                }
            }
            return null;
        }
        public dynamic getMaxVal(string tableName, string columnName)
        {
            string query = "SELECT MAX(" + columnName + ") FROM " + tableName;
            if (DBConnection.State == System.Data.ConnectionState.Open)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, DBConnection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read())
                {
                    var max = dataReader[columnName];
                    dataReader.Close();
                    return max;
                }
                else
                    return null;
            }
            return null;
        }
        public dynamic getMaxVal(string tableName, string columnName, string whereCondition)
        {
            string query = "SELECT MAX(" + columnName + ") FROM " + tableName +' '+ whereCondition;
            if (DBConnection.State == System.Data.ConnectionState.Open)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, DBConnection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read())
                {
                    var max = dataReader[columnName];
                    dataReader.Close();
                    return max;
                }
                else
                    return null;
            }
            return null;
        }
        public int countRows(string tableName, string columnName)
        {
            string query = "SELECT COUNT(" + columnName + ") FROM " + tableName;
            if (DBConnection.State == System.Data.ConnectionState.Open)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, AsyncDBConnection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read())
                {
                    int count = dataReader.GetInt32(0);
                    dataReader.Close();
                    return count;
                }
                else
                    return 0;
            }
            return 0;
        }
        public int countRows(string tableName, string columnName,
            string constraintName, string constraintValue)
        {
            string query = "SELECT COUNT(" + columnName + ") FROM " + tableName + " WHERE " +
                constraintName + "='" + constraintValue+"'";
            if (DBConnection.State == System.Data.ConnectionState.Open)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, AsyncDBConnection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read())
                {
                    int count = dataReader.GetInt32(0);
                    dataReader.Close();
                    return count;
                }
                else
                    return 0;
            }
            return 0;
        }
        public dynamic getKeyByKey(string tableName, string KnownKeyName, string KnownKeyValue, string unKnownKeyName)
        {
            string query = "SELECT " + unKnownKeyName + " FROM " + tableName + " WHERE " + KnownKeyName + "='" + KnownKeyValue+"'";
            if (DBConnection.State == System.Data.ConnectionState.Open)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, DBConnection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();
                if (dataReader.Read())
                {
                    var unKnownKeyValue = dataReader[0];
                    dataReader.Close();
                    return unKnownKeyValue;
                }
                else
                    return null;
            }
            return null;
        }
    }
}
