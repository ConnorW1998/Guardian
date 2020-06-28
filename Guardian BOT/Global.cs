using System;
using System.Collections.Generic;
using System.Text;
using Guardian_BOT.Modules;

namespace Guardian_BOT
{
    class Global
    {
        static readonly FileReader fileReader = new FileReader();
        internal static Dictionary<int,List<string>> CurseDict { get; set; }
        internal static int CurrentCurseList { get; set; }
        internal static ulong modChannelID { get; set; }
        internal static Discord.Color GOG_Gold { get; set; }
        internal static Discord.Color GOG_Purple { get; set; }
        internal static int GGCounter { get; set; }
        internal static int SnzCounter { get; set; }
        internal static int HckCounter { get; set; }

        internal void StartupInfo()
        {
            CurseDict = fileReader.DESERIALISE_DICT_LIST("Resources/Curses.json");
            CurrentCurseList = 1;
            //!modChannelID = 643313827915759617; //!GOG's MOD CHANNEL
            modChannelID = 694015797340667914; // CONCRETE's MOD CHANNEL

            GGCounter = fileReader.DESERIALISE_DICT_STRING("Resources/Counter.json")["GG"];
            SnzCounter = fileReader.DESERIALISE_DICT_STRING("Resources/Counter.json")["Sneeze"];
            HckCounter = fileReader.DESERIALISE_DICT_STRING("Resources/Counter.json")["Hacker"];

            GOG_Gold = new Discord.Color(250, 164, 5);
            GOG_Purple = new Discord.Color(177, 31, 224);
        }

        internal void FirstLaunch()
        {
            Dictionary<string, int> counterDict = new Dictionary<string, int>
            {
                { "GG", 0 },
                { "Sneeze", 0 },
                { "Hacker", 0 }
            };
            fileReader.SERIALISE_DICT_STRING(counterDict, "Resources/Counter.json");
        }
    }
}
