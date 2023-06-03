using Discord.Commands;
using OpenAI_API;
using OpenAI_API.Completions;
using System.Text;

public class AskCommand : ModuleBase<SocketCommandContext>
{
    private readonly string _openAiApiKey = "";

    [Command("ask")]
    public async Task AskAsync([Remainder] string question)
    {
        var answer = await GetAnswerAsync(question);
        await ReplyAsync(answer);
    }

    private async Task<string> GetAnswerAsync(string question)
    {
        var openAiEndpoint = "https://api.openai.com/v1/engines/davinci/completions";
        var prompt = $"Question: {question}\nAnswer: ";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _openAiApiKey);

            var requestBody = new
            {
                prompt = prompt,
                temperature = 0.6,
                max_tokens = 50
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(openAiEndpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var answer = ExtractAnswerFromCompletion(responseContent);
                return answer;
            }
            else
            {
                Console.WriteLine(response);
                return "An error occurred while fetching the answer.";
            }
        }
    }

    private string ExtractAnswerFromCompletion(string completion)
    {
        // Parse the completion and extract the answer
        var splitText = completion.Split("\n");
        if (splitText.Length > 1)
        {
            return splitText[1].Trim();
        }

        return "Unable to retrieve an answer.";
    }
}
