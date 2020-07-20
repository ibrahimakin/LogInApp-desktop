using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace LogInApp.Database
{
    class Records
    {
        static List<string> emails;
        static List<string> usernames;
        static List<string> hints;
        static List<string> labelses;
        public static void AddToTable(string site, string email, string username, string hint, string labels, string now, string value)
        {
            string commandText = "insert into Records (site, email, username, hint, labels, registrationDate, changingDate, sync, hash) values ('" + site + "', '" + email + "', '" + username + "', '" + hint + "', '" + labels + "', '" + now + "', '" + now + "', 2, '" + value + "');";
            Operations.UpdateTable(commandText);
        }

        public static List<Record> GetItems()
        {
            string commandText = "select * from Records";
            SQLiteDataReader rdr = Operations.GetItems(commandText);

            List<Record> records = new List<Record>();
            emails = new List<string>();
            usernames = new List<string>();
            hints = new List<string>();
            labelses = new List<string>();

            int id, sync;
            string site, email, username, hint, labels, registrationDate, changingDate, hash;

            while (rdr.Read())
            {
                id = Convert.ToInt32(rdr["id"]);
                site = rdr["site"].ToString();
                email = rdr["email"].ToString();
                username = rdr["username"].ToString();
                hint = rdr["hint"].ToString();
                labels = rdr["labels"].ToString();
                registrationDate = rdr["registrationDate"].ToString();
                changingDate = rdr["changingDate"].ToString();
                sync = Convert.ToInt32(rdr["sync"]);
                hash = rdr["hash"].ToString();

                Record item = new Record(id, site, email, username, hint, labels, registrationDate, changingDate, sync, hash);
                records.Add(item);

                if (!emails.Contains(email)) { emails.Add(email); }
                if (!usernames.Contains(username)) { usernames.Add(username); }
                if (!hints.Contains(hint)) { hints.Add(hint); }
                if (!labelses.Contains(labels)) { labelses.Add(labels); }
            }
            return records;
        }

        public static List<string> GetEMails()
        {
            return emails;
        }

        public static List<string> GetUsernames()
        {
            return usernames;
        }

        public static List<string> GetHints()
        {
            return hints;
        }

        public static List<string> GetLabels()
        {
            return labelses;
        }

        public static void DeleteFromTable(string id)
        {
            string commandText = "delete from Records where id = '" + id + "'";
            Operations.UpdateTable(commandText);
        }

        public static void DeleteFromTableMD5(string hash)
        {
            string commandText = "delete from Records where hash = '" + hash + "'";
            Operations.UpdateTable(commandText);
        }

        public static void UpdateRow(string Value, string Column, string hash)
        {
            string now = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            string commandText = "update Records set " + Column + " = '" + Value + "', changingDate = '" + now + "' where hash = '" + hash + "'";
            Operations.UpdateTable(commandText);

            commandText = "update Records set sync = 1 where hash = '" + hash + "' and sync=0";
            Operations.UpdateTable(commandText);
        }

        // Sadece bir kez kullanıldı
        public static void GenerateHash(int id, string name)
        {
            string Value = name;//Sync.MD5Operations.GetMd5Hash(name);
            string commandText = "update Records set hash = '" + Value + "' where id = '" + id + "'";
            Operations.UpdateTable(commandText);
        }
    }
}
