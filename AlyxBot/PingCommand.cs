using Discord.Commands;
using System.Threading.Tasks;

namespace AlyxBot
{
    public class PingCommand : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingCmd()
        {
            await ReplyAsync("Pong!");
        }
    }
}
