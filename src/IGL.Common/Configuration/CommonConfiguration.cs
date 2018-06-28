namespace IGL.Configuration
{
    public class CommonConfiguration
    {
        private static CommonConfiguration _instance;

        public static CommonConfiguration Instance
        {
            get => _instance ?? (_instance = new CommonConfiguration
            {
                EncryptionConfiguration =
                    new EncryptionConfiguration {IsEncryptionEnabled = false, Salt = string.Empty},
                SerializationConfiguration = new SerializationConfiguration {IsJsonEnabled = false},
                GameId = 0,
                PlayerId = ""
            });
            set => _instance = value;
        }

        public EncryptionConfiguration EncryptionConfiguration { get; set; }
        public SerializationConfiguration SerializationConfiguration { get; set; }

        /// <summary>
        ///     A global default for GameId for all GamePackets
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        ///     A global default for PlayerId for all GamePackets
        /// </summary>
        public string PlayerId { get; set; }
    }
}