using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace ClassJournal.Services
{
    public class ConnectionService
    {
        public bool IsConnected { get; private set; } = false;

        public ConnectionService()
        {
            ConnectionLoopService();
        }

        public async void ConnectionLoopService()
        {
            int tries = 0;

            await Task.Run(() =>
            {
                while (true)
                {
                    IsConnected = CheckConnection();

                    if (!IsConnected)
                    {
                        tries = 0;
                    }

                    if (IsConnected && tries < 5)
                    {
                        tries++;
                    }

                    if (IsConnected && tries == 5)
                    {
                        Task.Delay(3000);
                    }
                    else
                    {
                        Task.Delay(500);
                    }
                }
            });
        }

        public bool CheckConnection()
        {
            string ip = Properties.Resources.ipAddress;

            Ping ping = new Ping();
            PingReply reply = ping.Send(ip);

            if (reply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }
    }
}
