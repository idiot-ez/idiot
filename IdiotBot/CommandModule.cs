using Discord;
using Discord.Commands;
using IdiotBot.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IdiotBot.Modules
{
    public sealed class CommandModule : ModuleBase<SocketCommandContext>
    {
        // Dependency Injection will fill this value in for us
        public WikiFeetService WikiFeetService { get; set; }

        [Command("ping")]
        public Task PingAsync() => ReplyAsync("pong!");

        [Command("feet")]
        public async Task CatAsync(params string[] name)
        {
            if (name.Length != 2)
                await ReplyAsync("Incorrect name length.");

            (var isValid, var stream) = await WikiFeetService.GetPictureAsync(name[0], name[1]);
            if (!isValid)
                await ReplyAsync("Something went wrong.");

            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, $"{name[0]}_{name[1]}_feet.png");
        }

        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user = user ?? Context.User;

            await ReplyAsync(user.ToString());
        }
    }
}
