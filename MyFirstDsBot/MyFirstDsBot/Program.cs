using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace MyFirstDsBot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
        private DiscordSocketClient discordClient;
        private CommandService commandService;
        private IServiceProvider services;
        public async Task RunBotAsync()
        {
            discordClient = new DiscordSocketClient();
            commandService = new CommandService();
            services = new ServiceCollection()
                .AddSingleton(discordClient)
                .AddSingleton(commandService)
                .BuildServiceProvider();

            string token = "Nzk3MjE2ODA1NjkyMTEyOTE4.X_jQAg.OJ83SZKqKFIXqMtsWk2IWCRbV_U";

            discordClient.Log += DiscordClient_Log;

            await RegisterCommandsAsync();

            await discordClient.LoginAsync(TokenType.Bot, token);
            await discordClient.StartAsync();

            await Task.Delay(-1);
        }

        private Task DiscordClient_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            discordClient.MessageReceived += HandleCommandAsync;
            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage message = (SocketUserMessage)arg;
            SocketCommandContext context = new SocketCommandContext(discordClient, message);
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                IResult result = await commandService.ExecuteAsync(context, argPos, services);
                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }
    }
}
