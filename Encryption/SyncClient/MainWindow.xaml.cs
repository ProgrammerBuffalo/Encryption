using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

namespace SymmetricEncryptClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private IPAddress serverIp = IPAddress.Parse("127.0.0.1");
        private int serverPort = 3999;
        private Socket clientSocket;

        private string key = "BxG2xTHkhrYnLkmWy5Tf8wXQj4KZd1O9";

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

        public string CryptKey
        {
            get
            {
                return "Key: " + key;
            }
            set
            {
                currentState = value;
                NotifyPropertyChanged("Key");
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
                    Task.Run(() =>
                    {
                        Dispatcher.Invoke(async () =>
                        {
                            textBox.IsEnabled = false;
                            Cypher cypher = SymmetricEncryptor.EcryptAES(name + ": " + textBox.Text, key);
                            CurrentState = "Encrypting message..."; Progress += 33.3;
                            await Task.Delay(500);
                            byte[] data = SymmetricEncryptor.SerializeEncryptedData(cypher);
                            CurrentState = "Serializing message..."; Progress += 33.3;
                            await Task.Delay(500);
                            CurrentState = "Sending encrypted message..."; Progress += 33.3;
                            await Task.Delay(150);
                            clientSocket.Send(data);
                            richTxtBox.AppendText("You: " + textBox.Text + "\n");
                            textBox.Clear();
                            Progress = 0;
                            CurrentState = "";
                            textBox.IsEnabled = true;
                        });
                    });
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

                        Cypher cypher = SymmetricEncryptor.DeserializeEncryptedData(buffer);
                        string str = SymmetricEncryptor.DecryptAES(cypher, key);
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
