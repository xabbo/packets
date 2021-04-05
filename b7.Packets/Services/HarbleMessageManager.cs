using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using b7.Packets.Common.Messages;
using b7.Packets.Serialization;
using Microsoft.Extensions.Configuration;

namespace b7.Packets.Services
{
    public class HarbleMessageManager : IMessageManager
    {
        private readonly IConfiguration _config;
        private readonly HarbleMessages _messages;

        public HarbleMessageManager(IConfiguration config)
        {
            _config = config;

            string filePath = _config.GetValue("Messages:Path", "messages.json");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Could not find messages file", filePath);

            _messages = JsonSerializer.Deserialize<HarbleMessages>(File.ReadAllText(filePath), new JsonSerializerOptions()
            {
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            })
                ?? throw new Exception("Failed to load messages");
        }

        public bool TryGetHeader(Destination destination, short value, out Header header)
        {
            List<HarbleMessage> list = destination == Destination.Client ? _messages.Incoming : _messages.Outgoing;
            HarbleMessage? message = list.FirstOrDefault(x => x.Id == value);

            if (message is null)
            {
                header = Header.Unknown;
                return false;
            }
            else
            {
                header = new Header(destination, message.Id, message.Name);
                return true;
            }
        }

        public bool TryGetHeader(Destination destination, string name, out Header header)
        {
            List<HarbleMessage> list = destination == Destination.Client ? _messages.Incoming : _messages.Outgoing;
            HarbleMessage? message = list.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (message is null)
            {
                header = Header.Unknown;
                return false;
            }
            else
            {
                header = new Header(destination, message.Id, message.Name);
                return true;
            }
        }
    }
}
