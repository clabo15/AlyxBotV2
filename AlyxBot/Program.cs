using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AlyxBot
{
    public class Program
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public Program(CommandService commands, DiscordSocketClient client, IServiceProvider services)
        {
            _commands = commands;
            _client = client;
            _services = services;
        }

        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var config = new DiscordSocketConfig
                    {
                        GatewayIntents =
                            GatewayIntents.Guilds |
                            GatewayIntents.GuildMessages |
                            GatewayIntents.DirectMessages |
                            GatewayIntents.MessageContent |
                            GatewayIntents.GuildMessageReactions |
                            GatewayIntents.DirectMessageReactions
                    };
                    services.AddSingleton(new DiscordSocketClient(config));
                    services.AddSingleton<CommandService>();
                    services.AddSingleton<Program>();
                })
                .Build();

            host.Services.GetRequiredService<Program>().RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {
            _client.Log += Log;

            try
            {
                await RegisterCommandsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to register commands: {e.Message}");
                throw;
            }

            await _client.LoginAsync(TokenType.Bot, "");

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message))
            {
                Console.WriteLine("Received message is not a user message.");
                return;
            }

            // Ignore messages with no content or from bots.
            if (string.IsNullOrWhiteSpace(message.Content) || message.Author.IsBot) return;

            int argPos = 0;
            if (!message.HasStringPrefix("!", ref argPos)) return;

            Console.WriteLine($"User {message.Author.Username}#{message.Author.Discriminator} used command {message.Content}");            

            var context = new SocketCommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
        }


    }
}
