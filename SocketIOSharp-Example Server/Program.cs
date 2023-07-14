﻿using Newtonsoft.Json.Linq;
using SocketIOSharp.Common;
using SocketIOSharp.Server;
using System;

namespace SocketIOSharp.Example.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SocketIOServer server = new SocketIOServer(new SocketIOServerOption(9001, VerificationTimeout: 1001,AllowEIO3:true)))
            {
                Console.WriteLine("Listening on " + server.Option.Port);
                server.OnConnecting((obj) =>
                {
                    var token = obj.Item1["token"];
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} The authentication fails and the connection is denied");
                    }
                    server.AddVerificationResult(obj.Item2, string.IsNullOrWhiteSpace(token));
                });
                server.OnConnection((socket) =>
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Client connected!");

                    socket.On("input", (data) =>
                    {
                        foreach (JToken token in data)
                        {
                            Console.Write(token + " ");
                        }

                        Console.WriteLine();
                        socket.Emit("echo", data);
                    });

                    socket.On(SocketIOEvent.DISCONNECT, () =>
                    {
                        Console.WriteLine("Client disconnected!");
                    });

                    socket.Emit("echo", new byte[] { 0, 1, 2, 3, 4, 5 });
                });
                server.Start();
                Console.WriteLine("Input /exit to exit program.");
                string line;

                while (!(line = Console.ReadLine())?.Trim()?.ToLower()?.Equals("/exit") ?? false)
                {
                    server.Emit("echo", line);
                }
            }

            Console.WriteLine("Press enter to continue...");
            Console.Read();
        }
    }
}
