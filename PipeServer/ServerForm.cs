using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace PipeServer
{
    public partial class ServerForm : Form
    {
        private const string NAME_OF_PIPE = "NamedPipeTest";
        public NamedPipeServer Server { get; set; }
        delegate void ReceivedMessageDelegate(object sender, ReceivedMessageEventArgs e);
        private Queue<string> m_fromClientQueue = new Queue<string>();

        public ServerForm()
        {
            InitializeComponent();
            StartServer();
        }

        private void StartServer()
        {
            Server = new NamedPipeServer(NAME_OF_PIPE);
            Server.OnReceivedMessage += new EventHandler<ReceivedMessageEventArgs>(Server_OnReceivedMessage);
            Server.Start();
        }

        void Server_OnReceivedMessage(object sender, ReceivedMessageEventArgs e)
        {
            m_fromClientQueue.Enqueue(e.Message);
        }

        private void StopServer()
        {
            if (this.Server != null)
            {
                Server.Stop();
            }
        }

        private void m_timerServerReadFromPipe_Tick(object sender, EventArgs e)
        {
            if (m_fromClientQueue.Count > 0)
            {
                string value = m_fromClientQueue.Dequeue();

                txt_Server_Received.AppendText(value);
                //txt_Server_Received.AppendText(Environment.NewLine);
            }
        }

        private void WriteMessageToClient()
        {
            string message = DateTime.Now.ToString() + ": " + txt_Server_Message.Text.ToString();
            txt_Server_Sent.AppendText(message);
            txt_Server_Sent.AppendText(Environment.NewLine);
            this.Server.Write(message);
        }

        private void btn_Client_Send_Click(object sender, EventArgs e)
        {
            try
            {
                WriteMessageToClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            } 
        }

    }
}
