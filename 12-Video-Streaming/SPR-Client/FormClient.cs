using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
////////////////////////////
using MyNetwork;

namespace SPR_Client
{
    public partial class FormClient : Form
    {
        public FormClient()
        {
            InitializeComponent();
        }
        Net socket = new Net();
        private void Form1_Load(object sender, EventArgs e)
        {
            string ip = "127.0.0.1";
            int port = 6061;
			socket.ClientMessageReceived += socket_ClientMessageReceived;
            socket.createClient(ip, port,true,6060);

        }
        private List<MyNetwork.Message> currentStream = new List<MyNetwork.Message>();
		void socket_ClientMessageReceived(MyNetwork.Message obj)
		{
			var bw = new BackgroundWorker();
            bw.RunWorkerCompleted += (o, args) =>
            {
                if (obj == null) return;
                if (obj.MessageHeader == "[TEXT]")
                {
                    var str = UnicodeEncoding.Unicode.GetString(obj.Data);
                    listBox1.Items.Add("BroadCast: " + str);
                }
                else
                if (obj.MessageHeader == "[VIDEO]")
                {
                     currentStream.Add(obj);
                    if (obj.FinalOfBufferedPacket)
                    {
                        var bytes = new List<byte>();
                        var remMessages = new List<MyNetwork.Message>();
                        foreach (var b in currentStream.Where(m=>m.MessageID == obj.MessageID).OrderBy(m=>m.TimeStamp))
                        {
                            bytes.AddRange(b.Data);
                            //if (!remMessages.Contains(obj))
                            //{
                            //    remMessages.Add(obj);
                            //}
                        }
                        try
                        {
                            var img = Net.GetImageFromBytes(bytes.ToArray());
                            if (img != null)
                            {
                                currentStream.RemoveAll(r => r.MessageID == obj.MessageID);
                                pictureBox1.Image = img;
                               // pictureBox1.Refresh();
                            }
                        }
                        catch { }
                            
                        
                        //currentStream.Clear();
                       
                        
                    }
                    
                }
                
            };
			bw.RunWorkerAsync();
			//listBox1.Invoke(new Net.pointer_to_funcation(s => listBox1.Items.Add("BroadCast: " + obj)));			
		}

        private void button1_Click(object sender, EventArgs e)
        {
            //Send
            string msg = textBox1.Text;
            socket.sendMsg(msg);
            listBox1.Items.Add("YOU: " + msg);
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Receive
            string msg = socket.receiveMsg();
            listBox1.Items.Add("HIM: " + msg);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Send File
            openFileDialog1.ShowDialog();
            string file = openFileDialog1.FileName;
            socket.sendFile(file);
            MessageBox.Show("Done");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Recive File
            saveFileDialog1.ShowDialog();
            string file = saveFileDialog1.FileName;
            socket.receiveFile(file);
            MessageBox.Show("Done");
        }

        IWebCam webCam = null;
        private void button5_Click(object sender, EventArgs e)
        {
            socket.receiveImg(this, received_img);
            //if (webCam == null)
            //{
            //    webCam = new IWebCam(this.Handle);
            //    timer1.Start();
            //}
        }
        public void received_img(Image img)
        {
            pictureBox1.Image = img;
            pictureBox1.Refresh();
            socket.receiveImg(this, received_img);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Image img = webCam.iWebCam_Image;
            if (img != null)
            {
                pictureBox1.Image = img;
                socket.sendImg(img);
            }
        }
    }
}
