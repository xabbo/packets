using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using b7.Packets.Common.Messages;
using b7.Util;

namespace b7.Packets.Services
{
    public class MessageManager : IMessageManager
    {
        private readonly Dictionary<short, Header>
            _inValueMap = new(),
            _outValueMap = new();

        private readonly Dictionary<string, Header>
            _inNameMap = new(StringComparer.OrdinalIgnoreCase),
            _outNameMap = new(StringComparer.OrdinalIgnoreCase);

        public MessageManager(IConfiguration config)
        {
            Ini messages = Ini.Load(config.GetValue<string>("MessagesPath"));

            foreach (var (name, valueString) in messages["Incoming"])
            {
                short value = short.Parse(valueString);
                _inValueMap[value] =
                _inNameMap[name] = new Header(Destination.Client, value, name);
            }

            foreach (var (name, valueString) in messages["Outgoing"])
            {
                short value = short.Parse(valueString);
                _outValueMap[value] =
                _outNameMap[name] = new Header(Destination.Server, value, name);
            }
        }

        private IReadOnlyDictionary<short, Header> GetValueMap(Destination destination) => destination switch
        {
            Destination.Client => _inValueMap,
            Destination.Server => _outValueMap,
            _ => throw new Exception("Invalid destination")
        };

        private IReadOnlyDictionary<string, Header> GetNameMap(Destination destination) => destination switch
        {
            Destination.Client => _inNameMap,
            Destination.Server => _outNameMap,
            _ => throw new Exception("Invalid destination")
        };

        public bool TryGetHeader(Destination destination, short value, out Header header) => GetValueMap(destination).TryGetValue(value, out header);
        public bool TryGetHeader(Destination destination, string name, out Header header) => GetNameMap(destination).TryGetValue(name, out header);
    }
}
