using Discord;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysLaBot
{
    public class RemainderReader
    {
        public struct Remainder
        {
            public DateTimeOffset DateTimeOffset { get; set; }
            public Repetition Repetition { get; set; }
            public Embed Embed { get; set; }
        }

        public List<Remainder> Remainders { get; set; } = new List<Remainder>();

        public RemainderReader(string path)
        {
            string data;
            using (StreamReader sr = new StreamReader(path))
            {
                data = sr.ReadToEnd();
            }
            var lines = data.Replace("\r\n", "\n").Split('\n');
            foreach (var line in lines)
            {
                if (line == string.Empty)
                {
                    continue;
                }
                var item = line.Split('\t');
                var remainder = new Remainder();
                remainder.DateTimeOffset = DateTimeOffset.Parse(item[0]);
                remainder.Repetition = (Repetition)Array.FindIndex(
                    new string[] {
                        "Every year",
                        "Every month",
                        "Every week",
                        "Every day",
                        "Once only",
                    },
                    x => x == item[1]);
                remainder.Embed = new EmbedBuilder()
                {
                    // Embed property can be set within object initializer
                    Color = new Color(uint.Parse(item[2].Replace("#", ""), NumberStyles.AllowHexSpecifier)),
                    Title = item[3],
                    Description = item[4].Replace(@"\n", "\n"),
                    ThumbnailUrl = item[5],
                }.Build();
                Remainders.Add(remainder);
            }
        }

        public Embed ApplicableEmbed()
        {
            foreach (var remainder in Remainders)
            {
                if (remainder.DateTimeOffset.Hour == DateTimeOffset.Now.Hour &&
                    remainder.DateTimeOffset.Minute == DateTimeOffset.Now.Minute &&
                    remainder.DateTimeOffset.Second == DateTimeOffset.Now.Second)
                    switch (remainder.Repetition)
                    {
                        case Repetition.EveryYear:
                            if (remainder.DateTimeOffset.Month == DateTimeOffset.Now.Month &&
                                remainder.DateTimeOffset.Day == DateTimeOffset.Now.Day)
                                return remainder.Embed;
                            continue;
                        case Repetition.EveryMonth:
                            if (remainder.DateTimeOffset.Day == DateTimeOffset.Now.Day)
                                return remainder.Embed;
                            continue;
                        case Repetition.EveryWeek:
                            if (remainder.DateTimeOffset.DayOfWeek == DateTimeOffset.Now.DayOfWeek)
                                return remainder.Embed;
                            continue;
                        case Repetition.EveryDay:
                            return remainder.Embed;
                        case Repetition.OnceOnly:
                            if (remainder.DateTimeOffset.Year == DateTimeOffset.Now.Year &&
                                remainder.DateTimeOffset.Month == DateTimeOffset.Now.Month &&
                                remainder.DateTimeOffset.Day == DateTimeOffset.Now.Day)
                                return remainder.Embed;
                            continue;
                    }
            }
            return null;
        }
    }
}
