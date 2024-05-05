using Newtonsoft.Json;

namespace Trissiklikk.EditorTools
{
    public class DefineSymbolsData
    {
        [JsonProperty("SymbolName")]
        public string SymbolName;

        [JsonProperty("IsEnabled")]
        public bool IsEnabled;
    }
}