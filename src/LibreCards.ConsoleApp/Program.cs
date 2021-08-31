using LibreCards.ConsoleApp.Models;
using Microsoft.AspNetCore.SignalR.Client;

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

            hubConnection.On<IEnumerable<string>>("UpdateCards", cards =>
            {
                Console.WriteLine($"YOUR CARDS ARE: {string.Join(", ", cards)}");
            });

            hubConnection.On<IEnumerable<Guid>>("PlayerList", ids =>
            {
                Console.WriteLine($"Players: \t{string.Join("\n\t", ids)}");
            });

            hubConnection.On<GameModel>("GameStarted", game =>
            {
                Console.WriteLine(">>> Game Started <<<");
                Console.WriteLine($"Judge ID: {game.JudgeId}");
                Console.WriteLine(game.JudgeId == myId ? "You are the JUDGE" : "You are NOT the JUDGE");
            });

            hubConnection.On<string, int>("UpdateTemplate", (template, numOfSlots) =>
            {
                Console.WriteLine($"The current template:\n{template}\n- The template has {numOfSlots} slot(s).");
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
                else if (msg.ToLower() == "cards")
                {
                    await hubConnection.SendAsync("GetMyCards", myId);
                }
                else if (msg.ToLower() == "start")
                {
                    await hubConnection.SendAsync("StartGame");
                }
                else if (msg.ToLower() == "template")
                {
                    await hubConnection.SendAsync("RequestTemplate");
                }

                Console.WriteLine(string.Empty);
            }
        }
    }
}
