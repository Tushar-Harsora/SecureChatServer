namespace SecureChatServer.Models
{
    public class User
    {
        public int uid { get; set;  }
        public string email { get; set;  }
        public string username { get; set;  }
        public string public_key { get; set;  }

        public User()
        {
            uid = -1;
            email = "example@example.com";
            username = "example";
            public_key = "Placeholder";
        }
        public User(int uid, string email, string username, string public_key)
        {
            this.uid = uid;
            this.email = email;
            this.username = username;
            this.public_key = public_key;
        }
    }
}
