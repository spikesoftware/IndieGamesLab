using IGL;
using System;
using System.Collections.Generic;

namespace IGL.Common.Tests.Helpers
{
    internal class SampleGenerator
    {
        public static GamePacket GamePacketTest1()
        {
            GamePacket packet = new GamePacket
            {
                GameId = 999999,
                Correlation = Guid.NewGuid().ToString(),
                PacketNumber = 1
            };

            return packet;
        }

        public static GameEvent GameEventTest1()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();

            // at the end of a level a number of statistics are uploaded            
            properties.Add("stat_kill_creepy", "3");
            properties.Add("stat_case_unlocked", "1");
            properties.Add("stat_level_1_complete", "1");
            properties.Add("stat_avg_level_1", "360");   // the total elapsed time in seconds
            properties.Add("stat_average_level_speed", "330");    // the played elapsed time in seconds taking pause into considertion

            var event1 = new GameEvent
            {                
                Properties = properties
            };

            return event1;
        }
        public static GameEvent GameEventTestLarge()
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();

            for (int i = 0; i < 300; i++)
            {
                properties.Add("stat_" + i.ToString(), new string('x', 500));
            }

            var event1 = new GameEvent
            {
                Properties = properties
            };

            return event1;
        }


    }

    internal class Dummy
    {
        public int MyProperty { get; set; }
        public List<int> MyIntList { get; set; }
        public List<string> MyStringList { get; set; }
    }
}
