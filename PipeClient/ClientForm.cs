using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PipeClient
{
    public partial class ClientForm : Form
    {
        private const string NAME_OF_PIPE = "NamedPipeTest";
        public NamedPipeClient Client { get; set; }
        private Queue<string> m_fromServerQueue = new Queue<string>();

        public ClientForm()
        {
            InitializeComponent();
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

        private void m_timerClientReadFromPipe_Tick(object sender, EventArgs e)
        {
            try
            {
                if (m_fromServerQueue.Count > 0)
                {
                    txt_Client_Received.AppendText(m_fromServerQueue.Dequeue());
                    txt_Client_Received.AppendText(Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                m_timerClientReadFromPipe.Enabled = false;
                MessageBox.Show(ex.ToString());
            }
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
