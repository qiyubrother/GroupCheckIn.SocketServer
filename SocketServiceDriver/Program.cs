using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketService;
namespace SocketServiceDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            new CoreService().Start();
            Console.WriteLine("服务已启动.");
        }
    }
}
