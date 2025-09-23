using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeCommentAPI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var settings = new URLSettings();
            var server = new WebServer(settings);

            server.Start();
            Console.WriteLine("Press any key to stop the server");
            Console.ReadLine();
            server.Stop();

        }
    }
}
