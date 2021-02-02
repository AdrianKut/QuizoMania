using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker
{
    public class Database
    {
        string xd = "~/Database/DB_Server.mdf";
        public static readonly string path = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename="+AppDomain.CurrentDomain.BaseDirectory+"Database\\DB_server.mdf"+" ;Integrated Security=True;Connect Timeout=30";
    }
}
