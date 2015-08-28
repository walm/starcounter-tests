using System;
using System.Web;
using Starcounter;

namespace Chat
{
    [Database]
    public class User
    {
        public string Username;
        public string Password;               
    }

    [Database]
    public class Message
    {
        public string Body;
        public DateTime CreatedAt;
        public User User;

        public Message(string body, User user)
        {
            Body = body;
            User = user;
            CreatedAt = DateTime.Now;
        }
    }

    class Program
    {
        static void Main()
        {            
            Handle.POST("/chat/signup", (Request req) =>
            {
                var param = HttpUtility.ParseQueryString(req.Body);

                string username = param["username"];
                string password = param["password"];

                bool userExists = (Db.SQL<Decimal>("SELECT count(u) FROM User as u WHERE u.Username = ?", username).First > 0);
                if (userExists) return 400;

                User u = null;
                Db.Transact(() =>
                {
                    u = new User
                    {
                        Username = username,
                        Password = password
                    };
                });
                return u.Username;
            });

            Handle.POST("/chat/login", (Request req) =>
            {
                var param = HttpUtility.ParseQueryString(req.Body);
               
                string username = param["username"];
                string password = param["password"];
                
                var u = Db.SQL<User>("SELECT u FROM User as u WHERE u.Username = ? AND u.Password = ?", username, password).First;
                if (u == null) return 403;
                return "token: " + u.Username;
            });

            Handle.GET("/chat/messages", (Request req) =>
            {
                // TODO: authentication should be done in a middleware
                string token = req["X-Auth-Token"];

                var u = Db.SQL<User>("SELECT u FROM User as u WHERE u.Username = ?", token).First; // yes token should not just be the username, but this is just a test so keep it stupid simple.
                if (u == null) return 403;

                var messages = Db.SlowSQL<Message>("SELECT m FROM Chat.Message as m ORDER BY m.CreatedAt LIMIT 10");
                var json = new MessagesMsg();
                json.Messages = messages;
                return json;
            });

            Handle.POST("/chat/message", (Request req) =>
            {
                // TODO: authentication should be done in a middleware
                string token = req["X-Auth-Token"];

                var u = Db.SQL<User>("SELECT u FROM User as u WHERE u.Username = ?", token).First; // yes token should not just be the username, but this is just a test so keep it stupid simple.
                if (u == null) return 403;

                Db.Transact(() =>
                {
                    var param = HttpUtility.ParseQueryString(req.Body);
                    new Message(param["message"], u);
                });
                return 200;
            });
            
        }
    }
}