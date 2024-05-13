using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.WebSockets;
using WebSocketSharp.Server;

namespace IT3B_Chat.Server
{
 /// <summary>
 /// Interaction logic for MainWindow.xaml
 /// </summary>
 public partial class MainWindow : Window
 {
  public MainWindow()
  {
   InitializeComponent();
  }

        private WebSocketServer server;
        private List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();


        public MainWindow()
        {
            InitializeComponent();
            FleckLog.Level = LogLevel.Debug;
        }
        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            StartServer();
        }

        private void BtnStopServer_Click(object sender, RoutedEventArgs e)
        {
            StopServer();
        }

        private void StartServer()
        {
            server = new WebSocketServer("ws://0.0.0.0:8181");
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Dispatcher.Invoke(() => listBoxLog.Items.Add("Client Connected!"));
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    Dispatcher.Invoke(() => listBoxLog.Items.Add("Client Disconnected!"));
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                    Dispatcher.Invoke(() => listBoxLog.Items.Add("Received: " + message));
                    foreach (var s in allSockets)
                    {
                        s.Send("Echo: " + message);
                    }
                };
            });

            listBoxLog.Items.Add("Server started at ws://0.0.0.0:8181");
        }


        private void StopServer()
        {
            foreach (var socket in allSockets.ToList())
            {
                socket.Close();
            }
            server.Dispose();
            listBoxLog.Items.Add("Server stopped.");
        }
    }
}
}