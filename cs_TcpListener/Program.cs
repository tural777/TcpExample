using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace cs_TcpListener
{
    class Program
    {
        static void Main2(string[] args)
        {
            var ip = IPAddress.Parse("127.0.0.1");

            var listener = new TcpListener(ip, 45678);
            listener.Start(100);
            var client = listener.AcceptTcpClient();
            var stream = client.GetStream();

            var fs = new FileStream("Matata.png", FileMode.Create, FileAccess.Write);
            stream.CopyTo(fs);
            
            fs.Flush();
            stream.Flush();
            fs.Close();
            stream.Close();

            Console.WriteLine("Completed Receive");
        }


        static void Main(string[] args)
        {
            var ip = IPAddress.Parse("127.0.0.1");

            var listener = new TcpListener(ip, 45678);
            listener.Start(100);

            while (true)
            {
                var client = listener.AcceptTcpClient();
                var stream = client.GetStream();
                var br = new BinaryReader(stream);
                var bw = new BinaryWriter(stream);


                while (true)
                {
                    var input = br.ReadString();
                    var command = JsonSerializer.Deserialize<Command>(input);

                    if (command == null) continue;

                    Console.WriteLine(command.Text);
                    Console.WriteLine(command.Param);


                    switch (command.Text)
                    {
                        case Command.ProcList:
                            var processes = Process.GetProcesses();
                            bw.Write(JsonSerializer.Serialize(processes.Select(p => p.ProcessName)));
                            break;

                        case Command.Kill:
                            var procs = Process.GetProcessesByName(command.Param);

                            foreach (var proc in procs)
                            {
                                try { proc.Kill(); }
                                catch{ }
                            }
                            bw.Write(JsonSerializer.Serialize(true));
                            break;

                        case Command.Run:
                            Process.Start(command.Param);
                            bw.Write(JsonSerializer.Serialize(true));
                            break;
                    }

                }

            }
        }
    }


    public class Command
    {
        public const string ProcList = "PROCLIST";
        public const string Kill = "KILL";
        public const string Run = "RUN";

        public string Text { get; set; }
        public string Param { get; set; }
    }


}
