using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace IGL.Configuration
{
    public class CommonConfiguration
    {
        static CommonConfiguration _instance;
        public static CommonConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        var section = ConfigurationManager.GetSection("gamePacketConfigurationGroup/gamePacketConfiguration");

                        if (section != null)
                        {                         
                            var config = (GamePacketConfigurationSection)section;

                            _instance = new CommonConfiguration
                            {
                                EncryptionConfiguration = new EncryptionConfiguration { IsEncryptionEnabled = config.EncryptionConfiguration.IsEncryptionEnabled, Salt = config.EncryptionConfiguration.Salt },
                                SerializationConfiguration = new SerializationConfiguration { IsJsonEnabled = config.SerializationConfiguration.IsJsonEnabled },
                                BackboneConfiguration = new BackboneConfiguration { },
                                GameId = config.GameId,                                
                                PlayerId = config.PlayerId,                                
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        // TODO: logging
                        System.Diagnostics.Trace.TraceError(ex.Message);                        
                    }

                    // supply a default
                    if (_instance == null)
                        _instance = new CommonConfiguration
                        {
                            EncryptionConfiguration = new EncryptionConfiguration { IsEncryptionEnabled = false, Salt = string.Empty },
                            SerializationConfiguration = new SerializationConfiguration { IsJsonEnabled = false },
                            BackboneConfiguration = new BackboneConfiguration { },
                            GameId = 0,
                            PlayerId = "",
                        };
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public EncryptionConfiguration EncryptionConfiguration { get; set; }
        public SerializationConfiguration SerializationConfiguration { get; set; }
        public BackboneConfiguration BackboneConfiguration { get; set; }

        /// <summary>
        /// A global default for GameId for all GamePackets
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// A global default for PlayerId for all GamePackets
        /// </summary>
        public string PlayerId { get; set; }                
    }
}
