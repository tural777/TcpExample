using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace cs_TcpClient
{
    class Program
    {

        static void Main2(string[] args)
        {
            var client = new TcpClient();
            client.Connect("127.0.0.1", 45678);

            var stream = client.GetStream();

            var fs = new FileStream("c# logo.png", FileMode.Open, FileAccess.Read);
            fs.CopyTo(stream);

            // File.ReadAllBytes("c# logo.png");

            fs.Flush();
            stream.Flush();
            fs.Close();
            stream.Close();

            Console.WriteLine("Completed Send");
        }




        static void Main(string[] args)
        {
            var client = new TcpClient();
            client.Connect("127.0.0.1", 45678);

            var stream = client.GetStream();
            var br = new BinaryReader(stream);
            var bw = new BinaryWriter(stream);


            while (true)
            {
                var str = Console.ReadLine().ToUpper();

                if (str == "HELP")
                {
                    Console.WriteLine(Command.ProcList);
                    Console.WriteLine($"{Command.Kill} <process_name>");
                    Console.WriteLine($"{Command.Run} <process_name>");
                    Console.WriteLine("HELP");
                    continue;
                }


                Command cmd = null;
                string response = null;


                var input = str.Split(' ');

                switch (input[0])
                {
                    case Command.ProcList:
                        cmd = new Command
                        {
                            Text = input[0]
                        };

                        bw.Write(JsonSerializer.Serialize(cmd));
                        response = br.ReadString();

                        var processes = JsonSerializer.Deserialize<string[]>(response);

                        foreach (var process in processes)
                        {
                            Console.WriteLine(process);
                        }

                        break;

                    case Command.Kill:

                        cmd = new Command
                        {
                            Text = Command.Kill,
                            Param = input[1]
                        };

                        bw.Write(JsonSerializer.Serialize(cmd));
                        response = br.ReadString();

                        var isSuccess = JsonSerializer.Deserialize<bool>(response);

                        Console.WriteLine(isSuccess ? "Killed" : "Error");
                        break;


                    case Command.Run:
                        cmd = new Command
                        {
                            Text = Command.Run,
                            Param = input[1]
                        };

                        bw.Write(JsonSerializer.Serialize(cmd));
                        response = br.ReadString();

                        isSuccess = JsonSerializer.Deserialize<bool>(response);

                        Console.WriteLine(isSuccess ? "Started" : "Error");
                        break;

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
