using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibreCards.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var myId = new Guid();
            var serverUrl = args.Any() ? $"{args[0]}/cardsgame" : "https://localhost:5001/cardsgame";

            var hubConnection = new HubConnectionBuilder()
                .WithUrl(serverUrl)
                .Build();

            hubConnection.On<Guid>("PlayerJoined", id =>
            {
                Console.WriteLine($"Player Joined: {id}");
            });

            hubConnection.On<Guid>("IdAssigned", id =>
            {
                myId = id;
                Console.WriteLine($"Your ID is: {id}");
            });

            hubConnection.On<Guid>("PlayerLeft", id =>
            {
                Console.WriteLine($"Player Left: {id}");
            });

            hubConnection.On<IEnumerable<Guid>>("PlayerList", ids =>
            {
                Console.WriteLine($"Players: \t{string.Join("\n\t", ids)}");
            });

            await hubConnection.StartAsync();

            while (true)
            {
                Console.Write("> ");
                var msg = Console.ReadLine();

                if (msg.ToLower() == "exit")
                {
                    await hubConnection.SendAsync("Leave", myId);
                    await hubConnection.DisposeAsync();
                    Environment.Exit(0);
                }
                else if (msg.ToLower() == "join")
                {
                    await hubConnection.SendAsync("Join");
                    await hubConnection.SendAsync("GetPlayers");
                }

                Console.WriteLine(string.Empty);
            }
        }
    }
}
