using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordFirstBot
{
    public class Messages : ModuleBase
    {
        [Command("echo")]
        [Alias("say")]
        public async Task EchoAsync(string message)
        {
            await ReplyAsync(message);
        }

        [Command("emote")]
        public async Task EmoteAsync(string message)
        {
            var emote = Program.client.Guilds
            .SelectMany(x => x.Emotes)
            .FirstOrDefault(x => x.Name.IndexOf(message, StringComparison.OrdinalIgnoreCase) != -1);
            if (emote == null)
            {
                await ReplyAsync($"残念ながら、\":{message}:\"に該当するサーバ絵文字は見つかりませんでした。");
            }
            else
            {
                await ReplyAsync($"{emote.ToString()}");
            }
        }

        [Command("bold")]
        public async Task BoldAsync(string message)
        {
            await ReplyAsync($"**{message}**");
        }

        [Command("italic")]
        public async Task ItalicAsync(string message)
        {
            await ReplyAsync($"*{message}*");
        }

        [Command("neko")]
        public async Task NekoAsync()
        {
            await ReplyAsync("にゃーん");
        }

        [Command("neko")]
        public async Task NekoAsync(string message)
        {
            await ReplyAsync($"{message}にゃーん");
        }
    }
}
