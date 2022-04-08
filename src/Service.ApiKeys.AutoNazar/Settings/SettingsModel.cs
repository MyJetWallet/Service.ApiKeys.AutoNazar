using MyJetWallet.Sdk.Service;
using MyYamlParser;
using Telegram.Bot.Types;

namespace Service.ApiKeys.AutoNazar.Settings
{
    public class SettingsModel
    {
        [YamlProperty("AutoNazar.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("AutoNazar.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("AutoNazar.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("AutoNazar.BotApiKey")]
        public string BotApiKey { get; set; }

        [YamlProperty("AutoNazar.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }

        [YamlProperty("AutoNazar.TelegramChatId")]
        public string TelegramChatId { get; set; }

        [YamlProperty("AutoNazar.CheckPeriodInSeconds")]
        public int CheckPeriodInSeconds { get; set; }

        [YamlProperty("AuthApi.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }
    }
}
