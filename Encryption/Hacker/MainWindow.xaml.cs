using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hacker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPAddress serverIp = IPAddress.Parse("127.0.0.1");
        private int serverPort = 3999;
        private Socket clientSocket;

        // simulating that the hacker got the key
        private string key = "BxG2xTHkhrYnLkmWy5Tf8wXQj4KZd1O9";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            txtKey.Text = key;
            (sender as Button).IsEnabled = false;
            clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(serverIp, serverPort));

                Task.Run(() =>
                {
                    while (clientSocket.Connected)
                    {
                        byte[] buffer = new byte[1024000];
                        int length = clientSocket.Receive(buffer);
                        
                        Dispatcher.Invoke(() => txtEncrData.AppendText(Convert.ToBase64String(buffer.Take(length).ToArray())));
                        string str = DecryptionUtil.DecryptAES(buffer, key);
                        Dispatcher.Invoke(() => txtDecrData.AppendText(str + "\n"));
                    }
                });
            }
            catch
            {
                clientSocket.Close();
                MessageBox.Show("Something wrong with network, please re-open app");
            }
        }
    }
}
