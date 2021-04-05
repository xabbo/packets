using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace b7.Packets.Util
{
    public class Netstat
    {
        public static IEnumerable<Info> GetConnections(string? protocol = null)
        {
            var processes = Process.GetProcesses().ToDictionary(process => process.Id);

            ProcessStartInfo startInfo = new()
            {
                FileName = "netstat",
                Arguments = protocol is null ? "-ano" : $"-ano -p {protocol}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };

            Process netstat = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start the process");

            string? line;
            while ((line = netstat.StandardOutput.ReadLine()) != null)
            {
                string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 5) continue;

                string
                    proto = split[0],
                    localAddress = split[1],
                    remoteAddress = split[2],
                    state = split[3];

                processes.TryGetValue(int.Parse(split[4]), out Process? process);

                if (protocol?.Equals(proto, StringComparison.OrdinalIgnoreCase) == false)
                    continue;

                int
                    localIndex = localAddress.LastIndexOf(':'),
                    remoteIndex = remoteAddress.LastIndexOf(':');

                yield return new Info
                {
                    Protocol = proto,
                    LocalHost = localAddress[..localIndex],
                    LocalPort = int.Parse(localAddress[(localIndex + 1)..]),
                    RemoteHost = remoteAddress[..remoteIndex],
                    RemotePort = int.Parse(remoteAddress[(remoteIndex + 1)..]),
                    State = state,
                    Process = process
                };
            }

            yield break;
        }

        public static IEnumerable<Info> GetTcpListeners() => GetConnections("tcp").Where(x => x.State == "LISTENING");

        public class Info
        {
            public string Protocol { get; init; }
            public string LocalHost { get; init; }
            public int LocalPort { get; init; }
            public string RemoteHost { get; init; }
            public int RemotePort { get; init; }
            public string State { get; init; }
            public Process? Process { get; init; }
        }
    }
}
