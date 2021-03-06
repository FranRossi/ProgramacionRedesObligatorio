using CommonProtocol.NetworkUtils;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Common.Settings.Interfaces;

namespace ConsoleClient
{
    public class ClientNetworkStreamHandler : NetworkStreamHandler
    {
        private readonly TcpClient _tcpClient;
        private static readonly ISettingsManager SettingsMgr = new SettingsManager();

        public ClientNetworkStreamHandler(IPEndPoint clientIpEndPoint) :
            base()
        {
            _tcpClient = new TcpClient(clientIpEndPoint);
        }

        public async Task ConnectClient()
        {
            await _tcpClient.ConnectAsync(
                IPAddress.Parse(SettingsMgr.ReadSetting(ClientConfig.ServerIpConfigKey)),
                int.Parse(SettingsMgr.ReadSetting(ClientConfig.SeverPortConfigKey))).ConfigureAwait(false);
            _networkStream = _tcpClient.GetStream();
        }
    }
}
