using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PipeClient
{
    public partial class ClientForm : Form
    {
        private const string NAME_OF_PIPE = "NamedPipeTest";
        public NamedPipeClient Client { get; set; }
        private Queue<string> m_fromServerQueue = new Queue<string>();
        VideoPlayer video1;
        VideoPlayer video2;
        VideoPlayer video3;
        static int resolutionWidth = 1920;
        static int resolutionHight = 1080;
        int vid1X = 0;
        int vid2X = 0;
        int vid3X = 0;

        public ClientForm()
        {
            InitializeComponent();
            getScreenSize();
            StartClient();
        }

        private void StartClient()
        {
            Client = new NamedPipeClient(NAME_OF_PIPE);
            Client.OnReceivedMessage += new EventHandler<ReceivedMessageEventArgs>(Client_OnReceivedMessage);
            Client.Start();
        }

        void Client_OnReceivedMessage(object sender, ReceivedMessageEventArgs e)
        {
            m_fromServerQueue.Enqueue(e.Message); 
        }

        private void StopClient()
        {
            if (this.Client != null)
            {
                Client.Stop();
            }
        }

        private void m_timerClientReadFromPipe_Tick(object sender, EventArgs e) //polling the 
        {
            try
            {
                if (m_fromServerQueue.Count > 0)
                {
                    for (int i = 0; i < m_fromServerQueue.Count; i++)
                    {
                        string messageIn = m_fromServerQueue.Dequeue();
                        if (messageIn.Contains("video"))
                        {
                            string[] messageParts = messageIn.Replace("\r\n", "").Split('='); //

                            if (File.Exists(messageParts[1]))
                            {
                                if (video1 != null)
                                {
                                    InitialisePlayers(messageParts[1]);
                                }
                                else
                                {
                                    newVideo(messageParts[1]);
                                }
                            }
                        }
                    }
                    //handle the JSON message, do what action it requires, then call writemessagetoserver with your return confirmation message
                    txt_Client_Received.AppendText(messageIn);
                    txt_Client_Received.AppendText(Environment.NewLine);
                } 
            }
            catch (Exception ex)
            {
                m_timerClientReadFromPipe.Enabled = false;
                MessageBox.Show(ex.ToString());
            }
        }

        private void InitialisePlayers(string path)
        {
            video1 = new VideoPlayer(path);
            video2 = new VideoPlayer(path);
            video3 = new VideoPlayer(path);
            video1.Width = resolutionWidth;
            video1.Height = resolutionHight;
            video2.Width = resolutionWidth;
            video2.Height = resolutionHight;
            video3.Width = resolutionWidth;
            video3.Height = resolutionHight;
            video1.StartPosition = FormStartPosition.Manual;
            video1.Location = new Point(vid1X, 0);
            video2.StartPosition = FormStartPosition.Manual;
            video2.Location = new Point(vid2X, 0);
            video3.StartPosition = FormStartPosition.Manual;
            video3.Location = new Point(vid3X, 0);
            video1.Show();
            video2.Show();
            video3.Show();
        }

        private void newVideo(string path)
        {
            video1.newVideo(path);
            video2.newVideo(path);
            video3.newVideo(path);
        }
        private void getScreenSize()
        {
            Screen thisScreen = Screen.PrimaryScreen;
            resolutionHight = thisScreen.WorkingArea.Height;
            resolutionWidth = thisScreen.WorkingArea.Width;
            vid2X = resolutionWidth;
            vid3X = 2 * resolutionWidth;
        }

        private void WriteMessageToServer()
        {
            string message = DateTime.Now.ToString() + ": " + txt_Client_Message.Text.ToString();
            txt_Client_Sent.AppendText(message);
            txt_Client_Sent.AppendText(Environment.NewLine);
            this.Client.Write(message);
        }

        private void btn_Client_Send_Click(object sender, EventArgs e)
        {
            try
            {
                WriteMessageToServer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
