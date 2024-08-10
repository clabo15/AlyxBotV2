using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlyxBot;

public class RoastCommand : ModuleBase<SocketCommandContext>
{
    private const string ApiUrl = "https://evilinsult.com/generate_insult.php?lang=en&type=json";
   

    [Command("roast")]
    public async Task RoastCmd([Remainder] string user)
    {
        if (user == "user#1111" || user.ToLower() == "user" || user.Contains("user"))
        {
            await ReplyAsync("Ha you thought");
            return;
        }
        var insult = await GetRandomInsultAsync();
        var response = $"{user} {insult}";

        await ReplyAsync(response);
    }

    private async Task<string> GetRandomInsultAsync()
    {
        using (var client = new HttpClient())
        {
            var json = await client.GetStringAsync(ApiUrl);
            var roast = JsonConvert.DeserializeObject<Roast>(json);

            if(roast != null && !string.IsNullOrEmpty(roast.Insult)) 
            {
                return roast.Insult;
            }
            return "No insults can be generated.";
            
        }
    }
}

public class Roast
{
    public int Number { get; set; }
    public string Language { get; set; } = "en";
    public string Insult { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public string Shown { get; set; } = string.Empty;
    public string CreatedBy { get; set;} = string.Empty;
    public int Active { get; set; }
    public string Comment { get; set; } = string.Empty;
}

