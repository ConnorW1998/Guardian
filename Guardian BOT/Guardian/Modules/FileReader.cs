using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace Guardian_BOT.Modules
{
    public class FileReader
    {
        //! Dictionary<string,int>:
        public Dictionary<string,int> DESERIALISE_DICT_STRING(string path)
        {
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
        }
        public void SERIALISE_DICT_STRING(Dictionary<string, int> dictionary, string path)
        {
            string json = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        //! Dictionary<int,List<string>>
        public Dictionary<int, List<string>> DESERIALISE_DICT_LIST(string path)
        {
            if (!File.Exists(path)) return null;
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Dictionary<int,List<string>>>(json);
        }
        public void SERIALISE_DICT_LIST(Dictionary<int, List<string>> DictList, string path)
        {
            string json = JsonConvert.SerializeObject(DictList, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
