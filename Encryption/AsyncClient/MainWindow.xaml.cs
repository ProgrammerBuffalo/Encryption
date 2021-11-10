using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AsymmetricEncryptClient
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private IPAddress serverIp = IPAddress.Parse("127.0.0.1");
        private int serverPort = 3999;
        private Socket clientSocket;

        public event PropertyChangedEventHandler PropertyChanged;

        private string currentState;
        private double progress;
        private string name;

        public string CurrentState
        {
            get
            {
                return currentState;
            }
            set
            {
                currentState = value;
                NotifyPropertyChanged("CurrentState");
            }
        }

        public double Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = Math.Round(value);
                NotifyPropertyChanged("Progress");
            }
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (e.Key == Key.Enter)
            {

                if ((clientSocket?.Connected).GetValueOrDefault())
                {
                    byte[] data = AsymmetricEncryptor.EncryptRSA(textBox.Text);
                    clientSocket.Send(data);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            name = txtName.Text;

            (sender as Button).IsEnabled = false;
            txtName.IsEnabled = false;

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

                        string str = AsymmetricEncryptor.DecryptRSA(buffer);
                        Dispatcher.Invoke(() => richTxtBox.AppendText(str + "\n"));
                    }
                });

            }
            catch
            {
                clientSocket.Close();
                MessageBox.Show("Something wrong with network, please re-open app");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            clientSocket.Close();
        }
    }
}
