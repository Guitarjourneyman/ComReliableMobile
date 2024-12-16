using Microsoft.Extensions.Logging;
using Student;
using System.Threading;

namespace ComReliableMobile
{
    public partial class MainPage : ContentPage
    {
        private CancellationTokenSource _cancellationTokenSource;

        //KAU 
        public static bool startThread = true;
        private static int receivedNum = 1;
        
        public const int MESSAGE_LENGTH = 60;
        // UDP 패킷 수 설정 
        // 전체 패킷 수 (필요에 맞게 수정)
        public const int TOTAL_PACKETS = 61;

        private UDPer_client_Kau studentManager;
        public MainPage()
        {
            InitializeComponent();
            studentManager = new UDPer_client_Kau(LogMessage);

            // 브로드캐스트 주소 설정 (필요에 따라 변경)
            studentManager.SetBroadcastAddress("192.168.0.255");

            studentManager.OnReceiveMessage += OnMessageReceived;
        }
        private async void StartUdpCheck()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // 주기적으로 UDP 상태를 체크하는 작업 시작
                await Task.Run(() => PeriodicUDP_PacketCheck(_cancellationTokenSource.Token));
            }
            catch (OperationCanceledException)
            {
                // 작업이 취소된 경우 처리
                LogMessage("UDP Packet Check Canceled.");
            }
            catch (Exception ex)
            {
                LogMessage($"Error: {ex.Message}");
            }
        }

        private void StopUdpCheck()
        {
            _cancellationTokenSource?.Cancel(); // 작업 취소
        }

        private void PeriodicUDP_PacketCheck(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // UDP 패킷 체크
                studentManager.UDP_PacketCheck();

                // 1초 대기
                Thread.Sleep(100);
            }
        }
        private void StartUdpCheck_Clicked(object sender, EventArgs e)
        {
            StartUdpCheck();
            LogMessage("UDP Packet Check Started.");
        }

        private void StopUdpCheck_Clicked(object sender, EventArgs e)
        {
            StopUdpCheck();
            LogMessage("UDP Packet Check Stopped.");
        }

        private void StartUdp_Clicked(object sender, EventArgs e)
        {
            studentManager.Start();
            LogMessage("UDP Listening Started");
        }

        private void StopUdp_Clicked(object sender, EventArgs e)
        {
            studentManager.Stop();
            LogMessage("UDP Stopped");
        }
        private void StartCheckUDPPackets() {
            studentManager.UDP_PacketCheck();
        }
        private void OnMessageReceived(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LogMessage($"[RECEIVED] [{receivedNum}] {message}");
                receivedNum++;  
            });
        }

        private void LogMessage(string message)
        {
            LogLabel.Text += $"\n{message}";
        }
    }

}
