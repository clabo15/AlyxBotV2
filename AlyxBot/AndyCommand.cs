using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlyxBot;

public class AndyCommand : ModuleBase<SocketCommandContext>
{
    [Command("andy")]
    public async Task AndyCmd()
    {
        try
        {
            var images = Directory.GetFiles("Andy", "*.png");
            var random = new Random();
            var randomImage = images[random.Next(images.Length)];

            await ReplyAsync("Look at this chad!");
            await Context.Channel.SendFileAsync(randomImage);
        }
        catch (Exception ex)
        {
            await ReplyAsync($"An error occurred: {ex.Message}");
        }
    }
}

