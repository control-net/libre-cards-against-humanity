using Microsoft.AspNetCore.SignalR.Client;
using LibreCards.Core.Entities.Client;

namespace LibreCards.ConsoleApp
{
    class Program
    {
        private static LocalGameModel _localGame;

        static async Task Main(string[] args)
        {
            _localGame = new LocalGameModel
            {
                Rerender = RenderGameState
            };

            var serverUrl = args.Any() ? $"{args[0]}/cardsgame" : "https://localhost:5001/cardsgame";

            var hubConnection = new HubConnectionBuilder()
                .WithUrl(serverUrl)
                .Build();

            hubConnection.On<Guid>("PlayerJoined", _localGame.OnPlayerJoined);

            hubConnection.On<Guid>("IdAssigned", _localGame.OnIdAssigned);

            hubConnection.On<Guid>("PlayerLeft", _localGame.OnPlayerLeft);

            hubConnection.On<IEnumerable<string>>("UpdateCards", _localGame.OnUpdateCards);

            hubConnection.On<IEnumerable<Guid>>("PlayerList", _localGame.OnPlayerList);

            hubConnection.On<GameModel>("GameStarted", _localGame.OnGameStarted);

            hubConnection.On<string, int>("UpdateTemplate", _localGame.OnUpdateTemplate);

            await hubConnection.StartAsync();

            RenderGameState();

            while (true)
            {
                var msg = Console.ReadLine();

                if (msg.ToLower() == "exit")
                {
                    await hubConnection.SendAsync("Leave", _localGame.MyId);
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
                    await hubConnection.SendAsync("GetMyCards", _localGame.MyId);
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

        private static void RenderGameState()
        {
            Console.Clear();

            if(!_localGame.JoinedLobby)
            {
                WriteLineInColor("[Not In Lobby]", ConsoleColor.Yellow);
                WriteLineInColor("Write 'join' to join the lobby...");
                return;
            }

            WriteLineInColor("[In Lobby]", ConsoleColor.Green);
            WriteInColor("Your ID: ");
            WriteLineInColor(_localGame.MyId.ToString(), ConsoleColor.Green);

            WriteLineInColor("\n\n");

            if (_localGame.GameInProgress)
            {
                WriteInColor("Template Card: ");
                WriteLineInColor(_localGame.CurrentTemplate, ConsoleColor.Yellow);

                if(_localGame.IsJudging)
                {
                    WriteLineInColor("[You are the card judge]", ConsoleColor.Green);
                    WriteLineInColor("Wait till other players play their cards...");
                }
                else
                {
                    WriteLineInColor("[You are a player]", ConsoleColor.Blue);
                    WriteLineInColor($"Pick {_localGame.CurrentTemplateSlots} card(s) to fill the template's blanks with...");

                    if(!_localGame.Cards.Any())
                    {
                        WriteLineInColor($"Type 'cards' to view your list of cards.");
                    }
                    else
                    {
                        WriteLineInColor(string.Join(", ", _localGame.Cards));
                    }
                }
            }
            else
            {
                WriteLineInColor($"[ Lobby waiting with {_localGame.LobbyPlayers.Count()} player(s)... ]");
            }
        }

        private static void WriteInColor(string message = "", ConsoleColor foregroundColor = ConsoleColor.White)
        {
            Console.ForegroundColor = foregroundColor;
            Console.Write(message, foregroundColor);
            Console.ResetColor();
        }

        private static void WriteLineInColor(string message = "", ConsoleColor foregroundColor = ConsoleColor.White)
            => WriteInColor(message + '\n', foregroundColor);
    }
}
