using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SongRequest
{
    class Irc
    {
        private const string Server = "irc.twitch.tv";
        private const int Port = 6667;
        private StreamReader Reader = null;
        private StreamWriter Writer = null;
        private NetworkStream Ns = null;
        private TcpClient IrcConnection = null;
        private char[] ChatSeperator = { ' ' };
        private static Thread ReadStreamThread;
        private bool IsLineRead = true;
        private string IrcInput = "";
        public Queue<Message> Messeages = new Queue<Message>();

        public Irc(string channel, string nick, string password)
        {
            Connect(channel, nick, password);
            DataSend("jtvclient", null);
        }

        private void Connect(string channel, string nick, string password)
        {
            if (!nick.StartsWith("#"))
                nick = "#" + nick;
            try
            {
                IrcConnection = new TcpClient(Server, Port);
                Ns = IrcConnection.GetStream();
                Reader = new StreamReader(Ns);
                Writer = new StreamWriter(Ns);
                DataSend("PASS", password);
                DataSend("NICK", nick);
                DataSend("USER", nick);
                DataSend("JOIN", channel);
                //DataSend("jtvclient", null);
                

                ReadStreamThread = new Thread(new ThreadStart(this.StreamReader));
                ReadStreamThread.Start();
            }
            catch
            {
                Console.WriteLine("Uh-Oh");
            }
        }

        private void WordSplitter(string input)
        {
            try
            {
                var words = input.Split(ChatSeperator);
                var user = words[0].Remove(words[0].IndexOf('!')).TrimStart(':');
                words[0] = "";
                words[1] = "";
                words[2] = "";
                var text = string.Join(" ", words).Substring(4);
                Messeages.Enqueue(new Message { Text = text, User = user });

            }
            catch
            {
                Console.WriteLine("Uh-Oh");
            }
        }

        

        public void StreamReader()
        {
            while (true)
            {
                try
                {
                    if (Reader != null)
                    {
                        IrcInput = Reader.ReadLine();
                        if (IrcInput != null)
                        {
                            Console.WriteLine("IRC INPUT: " + IrcInput);
                            if (IrcInput.Contains("PRIVMSG"))
                            {
                                WordSplitter(IrcInput);
                            }
                            else if (IrcInput.Contains("PING"))
                            {
                                PingHandler(IrcInput);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error reading");
                }
                Thread.Sleep(200);
            }
        }
        

        private void PingHandler(string input)
        {
            var words = input.Split(ChatSeperator);
            if (words[0] == "PING")
            {
                DataSend("PONG", words[1]);
            }
        }

        public void DataSend(string cmd, string param)
        {
            if (param == null)
            {
                Writer.WriteLine(cmd);
                Writer.Flush();
            }
            else
            {
                Writer.WriteLine(cmd + ' ' + param);
                Writer.Flush();
            }
        }
    }
}
