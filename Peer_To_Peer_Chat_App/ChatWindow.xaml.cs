using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
using System.Windows.Shapes;
using Peer_To_Peer_Chat_App.Network;
using Peer_To_Peer_Chat_App.Network.Exceptions;

namespace Peer_To_Peer_Chat_App
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private readonly ICommunicator _communicator;
        private Thread receiveThread;
        public ChatWindow(string localEndPointStr, string remoteEndPointStr, ICommunicator communicator)
        {
            InitializeComponent();
            Title = $"{localEndPointStr} (you) - {remoteEndPointStr} (remote)";
            _communicator = communicator;
            RunReceiveThread();
        }
        private void StopReceiveThread()
        {
            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Abort();
            }
        }

        private void RunReceiveThread()
        {
            if (receiveThread != null)
            {
                throw new InvalidOperationException("receive thread alreay run");
            }
            receiveThread = new Thread(ReceiveCycle);
            receiveThread.Start();
        }

        private void ReceiveCycle()
        {
            while (true)
            {
                try
                {
                    string message = _communicator.Receive();
                    if (message == null)
                    {
                        continue;
                    }
                    Dispatcher.Invoke(() => chatListBox.Items.Add($"remote> {message}"));
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (SocketDisconnectedException)
                {
                    Dispatcher.Invoke(() =>
                    {
                        messageTextBox.IsEnabled = false;
                        sendMessageButton.IsEnabled = false;
                        MessageBox.Show("Удаленный хост отключился", "До свидания", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                    return;
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            messageTextBox.IsEnabled = false;
                            sendMessageButton.IsEnabled = false;
                            MessageBox.Show("Удаленный хост отключился неожиданно", "До свидания", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                        return;
                    }
                    throw ex;
                }
            }
        }

        private void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = messageTextBox.Text;
                _communicator.Send(message);
                chatListBox.Items.Add($"you> {message}");
                messageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка отправки сообщения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopReceiveThread();
        }

    }
}
