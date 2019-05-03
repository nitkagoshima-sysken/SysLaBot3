using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Discord;

namespace DiscordFirstBot
{
    public class Program
    {
        public static DiscordSocketClient client;
        public static CommandService commands;
        public static IServiceProvider services;

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// 起動時処理
        /// </summary>
        /// <returns></returns>
        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();
            client.MessageReceived += CommandRecieved;

            client.Log += Log;
            string token;
            // トークンを読み込みます。
            using (StreamReader sr = new StreamReader("token.dat"))
            {
                token = sr.ReadToEnd();
            }

            // タイマーの間隔(ミリ秒)
            // 1000ms
            System.Timers.Timer timer = new System.Timers.Timer(10000);
            // タイマーの処理
            timer.Elapsed += Remainder;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // タイマーを開始する
            timer.Start();

            await Task.Delay(-1);
        }

        /// <summary>
        /// メッセージの受信処理
        /// </summary>
        /// <param name="msgParam"></param>
        /// <returns></returns>
        private async Task CommandRecieved(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            Console.WriteLine("{0}/{1} @{2}: {3} ({4})",
                              (message.Channel as SocketGuildChannel).Guild.Name,
                              message.Channel.Name,
                              message.Author.Username,
                              message,
                              message.Timestamp.ToString("yyyy/MM/dd HH:mm:ss.fff zzz"));
            if (message == null) { return; }
            // コメントがユーザーかBotかの判定
            if (message.Author.IsBot) { return; }

            int argPos = 0;

            // コマンドかどうか判定（今回は、「!」で判定）
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) { return; }

            var context = new CommandContext(client, message);

            // 実行
            var result = await commands.ExecuteAsync(context, argPos, services);

            //実行できなかった場合
            if (!result.IsSuccess) { await context.Channel.SendMessageAsync(result.ErrorReason); }

        }

        /// <summary>
        /// コンソール表示処理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async void Remainder(Object sender, System.Timers.ElapsedEventArgs e)
        {
            await Announce();
        }

        public async Task Announce()
        {
            var embed = new EmbedBuilder()
            {
                // Embed property can be set within object initializer
                Color = Color.Blue,
                Title = "シス研の時間です",
                Description = "今日は木曜日です。\nプログラミングを楽しみましょう。",
                ThumbnailUrl = @"https://2.bp.blogspot.com/-JPa0Nzk_E8M/Vf-aIH2jsyI/AAAAAAAAyDc/2FG8dSNSk-k/s400/computer_girl.png"
            }.Build();
            //ulong id = 437074691098673164;
            // spam;
            ulong id = 509687250582372352;
            var channel = client.GetChannel(id) as IMessageChannel;
            await channel.SendMessageAsync(embed: embed);
        }
    }
}
