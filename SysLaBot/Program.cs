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

namespace SysLaBot
{
    public class Program
    {
        public static DiscordSocketClient client;
        public static CommandService commands;
        public static IServiceProvider services;

        public static RemainderReader remainder;

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
                token = sr.ReadToEnd().Trim();
            }

            // タイマーの間隔(ミリ秒)
            // 1000ms
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            // タイマーの処理
            timer.Elapsed += Remainder;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            remainder = new RemainderReader("remainder.tsv");

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

            // コマンドかどうか判定（今回は、「/」で判定）
            if (!(message.HasCharPrefix('/', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) { return; }

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
            // #general
            //ulong id = 437074691098673164;
            // #bot-develop
            ulong id = 509687250582372352;
            var channel = client.GetChannel(id) as IMessageChannel;
            var embed = remainder.ApplicableEmbed();
            if (embed != null)
                await channel.SendMessageAsync(embed: embed);
        }
    }
}
