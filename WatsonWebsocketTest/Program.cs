using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WatsonWebsocket;

namespace WatsonWebsocketTest
{
    class Program
    {
        static string _Hostname = "localhost";
        static int _Port = 9000;
        static Guid _ClientGuid ;

        static void Main(string[] args)
        {
            using (WatsonWsServer wss = new WatsonWsServer(_Hostname, _Port, false))
            {
                wss.ClientConnected += (s, e) => Console.WriteLine("Client connected: " + e.Client.Guid);
                wss.ClientDisconnected += (s, e) => Console.WriteLine("Client disconnected: " + e.Client.Guid);
                wss.MessageReceived += (s, e) =>
                {
                    Console.WriteLine("Server message received from " + e.Client.Guid + ": " + Encoding.UTF8.GetString(e.Data));
                    _ClientGuid = e.Client.Guid;
                };

                wss.Start();

                Thread.Sleep(2500);

                using (WatsonWsClient wsc = new WatsonWsClient(_Hostname, _Port, false))
                {
                    wsc.ServerConnected += (s, e) => Console.WriteLine("Client connected to server");
                    wsc.ServerDisconnected += (s, e) => Console.WriteLine("Client disconnected from server");
                    wsc.MessageReceived += (s, e) => Console.WriteLine("Client received message from server: " + Encoding.UTF8.GetString(e.Data));
                    wsc.Start();

                    Thread.Sleep(2500);

                    Console.WriteLine("Sending message from client to server...");
                    wsc.SendAsync("Hello from client").Wait();

                    Thread.Sleep(2500);

                    Console.WriteLine("Sending message from server to client...");
                    wss.SendAsync(_ClientGuid, "Hello from server").Wait();

                    Console.WriteLine("Press ENTER to exit");
                    Console.ReadLine();
                }
            }
        }
    }
}
