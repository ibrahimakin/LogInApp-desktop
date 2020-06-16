using System;
using System.Data.SQLite;

namespace LogInApp.Database
{
    class Operations
    {
        private static string DBpath = @"Data Source=" + Environment.CurrentDirectory + "\\DB\\dbRecords.db;Version=3;Compress=True;Read Only=False;";
        private static SQLiteConnection conn;
        private static bool ConnState;

        public static void OpenConn()
        {
            try
            {
                conn = new SQLiteConnection(DBpath);
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("select id from Records", conn);
                command.ExecuteNonQuery();
                ConnState = true;
            }
            catch (Exception)
            {
                ConnState = false;
            }
        }

        public static void CloseConn()
        {
            conn.Dispose();
        }

        public static bool GetState()
        {
            return ConnState;
        }

        public static SQLiteConnection getConn()
        {
            return conn;
        }

        public static void UpdateTable(string commandText)
        {
            SQLiteCommand command = new SQLiteCommand(commandText, conn);
            command.ExecuteNonQuery();
        }

        public static SQLiteDataReader GetItems(string commandText)
        {
            SQLiteCommand command = new SQLiteCommand(commandText, conn);
            SQLiteDataReader rdr = command.ExecuteReader();
            return rdr;
        }
    }
}
