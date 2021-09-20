namespace SecureChatServer.Models
{
    public class User
    {
        public int uid { get; set;  }
        public string email { get; set;  }
        public string phone_number { get; set; }
        public string username { get; set;  }
        public string public_key { get; set;  }

        public User()
        {
            uid = -1;
            email = "example@example.com";
            phone_number = "9999999999";
            username = "example";
            public_key = "Placeholder";
        }
        public User(int uid, string email, string phone_number, string username, string public_key)
        {
            this.uid = uid;
            this.email = email;
            this.phone_number = phone_number;
            this.username = username;
            this.public_key = public_key;
        }
    }
}
