using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class DiscordBot
{
    private readonly DiscordSocketClient discordClient;
    private readonly CommandService commandService;
    private readonly IServiceProvider serviceProvider;

    public DiscordBot(string discordBotToken)
    {
        // Initialize the Discord bot
        discordClient = new DiscordSocketClient();
        commandService = new CommandService();
        serviceProvider = null; // You may provide your own service provider for dependency injection

        // Register events
        discordClient.Log += Log;
        discordClient.Ready += Ready;

        // Register command modules
        discordClient.MessageReceived += MessageReceived;

        // Login and start the bot
        discordClient.LoginAsync(TokenType.Bot, discordBotToken).Wait();
        discordClient.StartAsync().Wait();
    }

    private Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        return Task.CompletedTask;
    }

    private async Task Ready()
    {
        Console.WriteLine("Bot is connected and ready!");
    }

    private async Task MessageReceived(SocketMessage arg)
    {
        // Ignore system messages and messages from other bots
        if (!(arg is IUserMessage userMessage) || userMessage.Author.IsBot)
            return;

        // Process commands
        var context = new SocketCommandContext(discordClient, userMessage);
        var commandResult = await commandService.ExecuteAsync(context, 0, serviceProvider);
    }

    public void NotifyRegistration(string username, string email)
    {
        // You can implement logic here to send a message to a specific channel or user on Discord
        // notifying about the registration.
        // Example: discordClient.GetGuild(guildId).GetTextChannel(channelId).SendMessageAsync($"New registration: {username}, {email}");
    }
}
