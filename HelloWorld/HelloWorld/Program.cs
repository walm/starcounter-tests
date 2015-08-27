using System;
using Starcounter;

namespace HelloWorld
{
    [Database]
    public class User
    {
        public string Name;
        public DateTime CreatedAt;

        public User(string name)
        {
            Name = name;
            CreatedAt = DateTime.Now;
        }
    }

    class Program
    {
        static void Main()
        {
            Handle.GET("/helloworld", () => "Hello World!");
            Handle.GET("/helloworld/{?}", (string name) => 
            {
                User u;
                u = Db.SQL<User>("SELECT u FROM User as u WHERE u.Name = ?", name).First;
                if (u == null)
                {
                    Db.Transact(() =>
                    {
                        u = new User(name);
                    });
                }
                return "Hello " + u.Name + " from " + u.CreatedAt.ToString() + "!";
            });
        }
    }
}