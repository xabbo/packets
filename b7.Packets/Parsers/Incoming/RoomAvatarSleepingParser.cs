using System;

namespace b7.Packets.Parsers.Incoming
{
    [OutParser("RoomAvatarSleeping")]
    public class RoomAvatarSleepingParser : IMessageParser
    {
        public void Parse(IParserContext context)
        {
            context.ReadInt("entityIndex");
            context.ReadBool("isSleeping");
        }
    }
}
