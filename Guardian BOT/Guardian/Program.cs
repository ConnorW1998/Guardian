using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Guardian_BOT.Modules;
using Guardian_BOT;

namespace Guardian_BOT
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private FileReader _FileReader = new FileReader();
        private Global _Global = new Global();

        

        internal static List<string> CurseWords { get; set; }

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _Global.StartupInfo();

            string token = "Njc5ODk4NDEwMTE5NDYyOTI1.Xk4JCg.6pkyYVm9NDjXi7159zTp6NBQQac";

            //! USE THIS TO INSTANTIATE FILE WRITING:
            //_Global.FirstLaunch();

            _client.Log += _client_Log;

            await RegisterCommandAsync();

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleMessageAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        }

        private async Task HandleMessageAsync(SocketMessage arg)
        {
            string PREFIX = ">";

            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            var channel = message.Channel;
            if (message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix(PREFIX, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }

            //! Filter the message:
            if(CheckMessage(arg))
            {
                //! Delete bad message:
                await message.DeleteAsync();
            }
        }

        private bool CheckMessage(SocketMessage arg)
        {
            //! Contain context:
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            var channel = message.Channel;

            //! Grab current list of filtered words:
            List<string> currentFilterWords = Global.CurseDict[Global.CurrentCurseList];

            //! Generate time-stamp of message:
            var timeStampVAR = message.Timestamp.DateTime;
            string timeStampDay = $"{timeStampVAR.Day}/{timeStampVAR.Month}/{timeStampVAR.Year}";
            string timeStampTime = $"{timeStampVAR.Hour}:{timeStampVAR.Minute}:{timeStampVAR.Second}";

            //! To keep/use old check method (only reads 1 word, if message.content has more than 1 word, doesn't work)
            //! Basically just kept to reference.
            bool useOldCheckMethod = false;

            if(useOldCheckMethod)
            {
                //! OLD - DO NOT USE
                if (currentFilterWords.Contains(message.Content))
                { //! If the list contains the message (not good code):
                    //! console message layout: dd/mm/yyyy - hh:mm:ss - username: message
                    Console.WriteLine($"{timeStampDay} - {timeStampTime} - {message.Author.Username}'s blocked message: {message.Content}");
                    return true;
                }
            }
            else
            {
                //! NEW - ONLY USE THIS

                //! Generate an array of all the words:
                string[] MessageContent = message.Content.Split(' ',',','.','!','?','\'','\"');

                //! Create list to contain all the word(s) that the filter finds in the message:
                List<string> FoundWords = new List<string>();

                //! Search every string in the message.content, see if it's found in the list of curse words:
                foreach(string word in MessageContent)
                { //! For every word in the message...
                    if (currentFilterWords.Contains(word.ToLower()))
                    { //! See if the list contains it:
                        //! Add it to the list of FoundWords
                        FoundWords.Add(word);
                    }
                }

                if(FoundWords.Count >= 1)
                { //! If the list of FoundWords contains anything (count >= 1)...
                    //! Writeout the curse(s) and message details:

                    //! Console message layout: dd/mm/yyyy - hh:mm:ss - username: bad-word. message
                    Console.WriteLine($"{timeStampDay} - {timeStampTime} - {message.Author.Username}'s blocked message: {message.Content}");
                    Console.WriteLine($"Word(s) caught: ");
                    
                    //! Printout the individual words that were caught:
                    for(int i = 0; i < FoundWords.Count; i++)
                    {
                        Console.Write($"{FoundWords[i]}, ");
                    }
                    //! Let the bot know to remove the message and inform the admins:
                    //! Find the channel w/in the guild:
                    var modChannel = _client.GetChannel(Global.modChannelID) as IMessageChannel;
                    //! Let the mods know!
                    modChannel.SendMessageAsync($"{message.Author.Username}'s blocked message: {message.Content}");
                    return true;
                }
            }
            //! Nothing was caught! Let the bot know that it doesn't need to take action: 
            return false;
        }
    }
}
