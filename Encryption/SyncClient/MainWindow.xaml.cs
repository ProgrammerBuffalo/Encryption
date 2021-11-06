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

namespace SyncClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPAddress serverIp = IPAddress.Parse("127.0.0.1");
        private int serverPort = 3999;
        private Socket clientSocket;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
                        Dispatcher.Invoke(() =>
                        {
                            string str = SymmetricEncryptor.DecryptAES(buffer);
                            richTxtBox.AppendText(str + "\n");
                        });
                    }
                });
            }
            catch
            {
                clientSocket.Close();
                MessageBox.Show("Something wrong with network, please re-open app");
            }
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if(e.Key == Key.Enter)
            {
                if ((clientSocket?.Connected).GetValueOrDefault())
                {
                    clientSocket.Send(SymmetricEncryptor.EcryptAES(textBox.Text));
                    textBox.Clear();
                }
            }
        }
    }
}
