using System;

namespace TcpExample.Infrastructure.Config
{
    [Serializable]
    public class ConnectionConfig
    {
        public string EndpointIp { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 9000;
    }
}
