using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace Guardian_BOT.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        //! Ping BOT
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("pong");
        }

        //! ADMIN:
        [Command("filter")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeFilterInt(int filterIndex)
        {
            string filterTitle;
            switch(filterIndex)
            {
                case 0: filterTitle = "no";
                    break;

                case 1: filterTitle = "soft";
                    break;

                case 2: filterTitle = "fair";
                    break;

                case 3: filterTitle = "tough";
                    break;

                default:
                    filterTitle = "no";
                    filterIndex = 0;
                    break;
            }
            Global.CurrentCurseList = filterIndex;
            await Context.Channel.SendMessageAsync($"Chat filter has been set to \"{filterTitle}\" filter. Intensity: {filterIndex}/3");
            Console.WriteLine($"Filter strength set to: {filterIndex}/3, {filterTitle} filter.");
        }
        [Command("filter")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeFilterString(string filterTitle)
        {
            int filterIndex;
            switch(filterTitle)
            {
                case "none":
                    filterTitle = "no";
                    filterIndex = 0;
                    break;

                case "soft":
                    filterIndex = 1;
                    break;

                case "fair":
                    filterIndex = 2;
                    break;

                case "tough":
                    filterIndex = 3;
                    break;

                default:
                    filterTitle = "no";
                    filterIndex = 0;
                    break;
            }
            Global.CurrentCurseList = filterIndex;
            await Context.Channel.SendMessageAsync($"Chat filter has been set to \"{filterTitle}\" filter. Intensity: {filterIndex}/3");
            Console.WriteLine($"Filter strength set to: {filterIndex}/3, {filterTitle} filter.");
        }

        [Command("channel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeChannelOutput(ulong ID)
        {
            Global.modChannelID = ID;
            await Context.Channel.SendMessageAsync($"Changed Guardian output channel: {ID}");
        }

        [Command("poll")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CreatePoll(uint duration, char scale, [Remainder] string arg = "")
        {
            if (Context.Message.Content.Length < 1) return; //! If they didn't include context for a vote, don't function.

            //! Delete poll command message:
            await Context.Message.DeleteAsync();

            //! Create embed to host poll:
            var embed = new EmbedBuilder();
            embed.WithTitle("Poll");
            embed.WithDescription(arg);
            embed.WithColor(Global.GOG_Purple);
            embed.WithFooter($"poll created by {Context.Message.Author.Username}");

            //! Post poll in chat:
            var pollMessage = await Context.Channel.SendMessageAsync("", false, embed.Build());

            //! Add reactions to embed:
            IEmote check = new Emoji("\u2705"); //! ✅
            IEmote cross = new Emoji("\u274E"); //! ❎
            await pollMessage.AddReactionsAsync(new[] { check, cross });

            //! Convert time:
            switch(scale)
            {
                case 'd':
                    await Task.Delay((int)TimeSpan.FromDays(duration).TotalMilliseconds);
                    break;

                default:
                    await Context.Channel.SendMessageAsync("Unusable scale, defaulting to MINUTES");
                    await Task.Delay((int)TimeSpan.FromMinutes(duration).TotalMilliseconds);
                    break;
            }
            //! await reactions:
            await Task.Delay(10000);

            //! Update the message attributes:
            await pollMessage.UpdateAsync();

            //! Gather emoji inputs:
            int[] results = new[] { pollMessage.Reactions[check].ReactionCount, pollMessage.Reactions[cross].ReactionCount };

            //! Remove original poll
            await pollMessage.DeleteAsync();

            //! Create results embed and send
            var resultsEmbed = new EmbedBuilder()
            {
                Title = "Poll Results",
                Color = Global.GOG_Purple
            };
            resultsEmbed.AddField("Poll reason:", arg, true); //! Displays poll reason
            //! results[x] - 1 is to adjust for the bot's reaction!
            resultsEmbed.AddField("✅", results[0] - 1, true); //! Displays ✅ count
            resultsEmbed.AddField("❎", results[1] - 1, true); //! Displays ❎ count
            await Context.Channel.SendMessageAsync("", false, resultsEmbed.Build());
        }

        [Command("purge")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Purge(uint purgeCount = 10)
        {
            var messages = await Context.Channel.GetMessagesAsync((int)purgeCount + 1).FlattenAsync(); //await Context.Channel.GetMessagesAsync((int)purgeCount + 1).Flatten();
            await (Context.Channel as ITextChannel).DeleteMessagesAsync(messages);
            const int delay = 5000;
            var m = await this.ReplyAsync($"Purge completed. _This message will be deleted in {delay / 1000} seconds._");
            await Task.Delay(delay);
            await m.DeleteAsync();
        }

        //! COUNTER COMMANDS:

        [Command("gamergirl")]
        public async Task GGCounter()
        {
            //! Add 1:
            Global.GGCounter += 1;

            //! Embed count:
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "\"Is That A Gamer Girl??\"",
                Description = $"Has been said {Global.GGCounter} times!",
                Color = Global.GOG_Gold
            };

            //! Send embed:
            await Context.Channel.SendMessageAsync(null,false,embed.Build());
        }
        
        [Command("sneeze")]
        public async Task SnzCounter()
        {
            //! Add 1:
            Global.SnzCounter += 1;

            //! Embed count:
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "AAAAAACCCHHHOOOOO",
                Description = $"Someone has sneezed {Global.SnzCounter} times in-call!",
                Color = Global.GOG_Gold
            };

            //! Send embed:
            await Context.Channel.SendMessageAsync(null,false,embed.Build());
        }

        [Command("hacker")]
        public async Task HckCounter()
        {
            //! Add 1:
            Global.HckCounter += 1;

            //! Embed count:
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "I call BS!",
                Description = $"Someone has been called a hacker {Global.HckCounter} times in-game!",
                Color = Global.GOG_Gold
            };

            //! Send embed:
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
