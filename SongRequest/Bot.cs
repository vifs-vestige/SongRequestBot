using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongRequest
{
    class Bot
    {
        private Irc Irc;
        private List<String> Users;
        public Queue<Request> Requests;

        public Bot(Irc irc)
        {
            Irc = irc;
            Users = new List<string>();
            Requests = new Queue<Request>();
        }

        public void Update()
        {
            if(Irc.Messeages.Count > 0)
            {
                var temp = Irc.Messeages.Dequeue();
                if (!Users.Contains(temp.User))
                {
                    if (temp.Text.StartsWith("!sr"))
                    {
                        var words = temp.Text.Split(' ');
                        words[0] = "";
                        var song = string.Join(" ", words).TrimStart(' ');
                        if (Requests.Count(x => x.Song == song) == 0)
                        {
                            Requests.Enqueue(new Request() { User = temp.User, Song = song });
                            //Users.Add(temp.User);
                        }
                    }
                }
            }
        }

        public void RemoveUser(string user)
        {
            Users.Remove(user);
        }

        public void ClearUsers()
        {
            Users.Clear();
        }

    }
}
