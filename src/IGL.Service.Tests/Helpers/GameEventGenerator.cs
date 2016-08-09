using System;
using System.Collections.Generic;

namespace IGL.Service.Tests.Helpers
{
    internal static class GameEventGenerator
    {
        public static GameEvent GenerateValidGameEvent()
        {
             Dictionary<string, string> properties = new Dictionary<string, string>();

            // at the end of a level a number of statistics are uploaded
            properties.Add("stat_kill_roamer", "1");
            properties.Add("stat_kill_creepy", "3");
            properties.Add("stat_case_unlocked", "1");
            //properties.Add("stat_level_1_complete", "1");
            //properties.Add("stat_avg_level_1", "360");   // the total elapsed time in seconds
            //properties.Add("stat_average_level_speed", "330");    // the played elapsed time in seconds taking pause into considertion
            
            var event1 = new GameEvent
            {
                GameId = 342550,
                EventId = 100,   // identifies a level was completed                
                Properties = properties
            };

            Dictionary<string, string> additional = new Dictionary<string, string>();

            additional.Add("BUILD_ID", "65580");
            additional.Add("EVENT_DATE", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
            additional.Add("SESSION_ID", Guid.NewGuid().ToString());
            additional.Add(GamePacket.VERSION, GamePacket.Namespace);

            return event1;
        }
    }
}
