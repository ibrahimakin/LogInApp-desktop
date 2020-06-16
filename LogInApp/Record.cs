namespace LogInApp
{
    using Sync;
    public class Record
    {
        public Record(int id, string site, string email, string username, string hint, string labels, string registrationDate, string changingDate, int sync, string hash)
        {
            Id = id;
            Site = site;
            EMail = email;
            Username = username;
            Hint = hint;
            Labels = labels;
            RegistrationDate = registrationDate;
            ChangingDate = changingDate;
            Sync = sync;
            Hash = hash;
        }

        public int Id { get; set; }
        public string Site { get; set; }
        public string EMail { get; set; }
        public string Username { get; set; }
        public string Hint { get; set; }
        public string Labels { get; set; }
        public string RegistrationDate { get; set; }
        public string ChangingDate { get; set; }
        public int Sync { get; set; }
        public string Hash { get; set; }

        public void setValue(string type, string value)
        {
            string newHash,hashSource;
            switch (type)
            {
                case "Site":
                    hashSource = value + EMail;
                    newHash = MD5Operations.GetMd5Hash(hashSource);
                    Database.Records.UpdateRow(newHash, "hash", Hash);
                    Hash = newHash;
                    Database.Records.UpdateRow(value, "site", Hash);
                    Site = value;
                    break;
                case "EMail":
                    hashSource = Site + value;
                    newHash = MD5Operations.GetMd5Hash(hashSource);
                    Database.Records.UpdateRow(newHash, "hash", Hash);
                    Hash = newHash;
                    Database.Records.UpdateRow(value, "email", Hash);
                    EMail = value;
                    break;
                case "Username":
                    Database.Records.UpdateRow(value, "username", Hash);
                    Username = value;
                    break;
                case "Hint":
                    Database.Records.UpdateRow(value, "hint", Hash);
                    Hint = value;
                    break;
                case "Labels":
                    Database.Records.UpdateRow(value, "labels", Hash);
                    Labels = value;
                    break;
                default:
                    break;
            }
        }

        public override string ToString()
        {
            return "";
        }
    }
}