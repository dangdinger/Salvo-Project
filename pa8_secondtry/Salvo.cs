using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace pa8_secondtry
{
    public partial class Salvo : Form
    {
        private TcpClient client;
        private StreamReader read;
        private StreamWriter write;
        private String positionTake;
        private String positionShoot;
        private Random x = new Random();
        private Boolean turnnotDecide = true;
        private int turn = 0;
        private int turn2 = 0;
        private int hitCount = 0;
        private int hitCount2 = 0;

        private void decideFirst()
        {
            turn = x.Next(0, 2);
            if(turn == 1)
            {
                turn2 = 0;
            }
            else
            {
                turn2 = 1;
            }
            dataSender(turn.ToString());
            turnnotDecide = false;
        }

        public Salvo()
        {
            InitializeComponent();
        }

        private void startServer_Click(object sender, EventArgs e)
        {
            TcpListener listen = new TcpListener(IPAddress.Any, 12345);
            listen.Start();
            client = listen.AcceptTcpClient();
            read = new StreamReader(client.GetStream());
            write = new StreamWriter(client.GetStream());
            write.AutoFlush = true;

            backgroundWorker1.RunWorkerAsync();
            textBox1.Show();
            startServer.Hide();
            connectClient.Hide();
            label3.Hide();
            label4.Hide();
            IPBox.Hide();
            portBox.Hide();
            initializeShip();
            decideFirst();
            shootTip.Show();
        }

        private void connectClient_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            System.Threading.Thread.Sleep(600);
            IPEndPoint IP = new IPEndPoint(IPAddress.Parse(IPBox.Text), Int32.Parse(portBox.Text));

            try
            {
                client.Connect(IP);
                if (client.Connected)
                {
                    read = new StreamReader(client.GetStream());
                    write = new StreamWriter(client.GetStream());
                    write.AutoFlush = true;

                    backgroundWorker1.RunWorkerAsync();
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connectClient.Hide();
            startServer.Hide();
            label3.Hide();
            label4.Hide();
            IPBox.Hide();
            portBox.Hide();
            textBox1.Show();
            System.Threading.Thread.Sleep(500);
            initializeShip();
            decideFirst();
            shootTip.Show();
        }

        private void initializeShip()
        {
            int orientation = x.Next(0, 2);
            if (orientation == 0)               //vertical ship selection
            {
                int pos = x.Next(51, 66);
                foreach (Control y in Controls)
                {
                    if (y.TabIndex == pos)
                    {
                        y.BackColor = Color.Black;
                    }
                    if (y.TabIndex == (pos + 5))
                    {
                        y.BackColor = Color.Black;
                    }
                    if (y.TabIndex == (pos + 10))
                    {
                        y.BackColor = Color.Black;
                    }
                }
            }
            else
            {
                Boolean posnotFound = true;
                while (posnotFound)
                {
                    int pos2 = x.Next(51, 74);
                    if (pos2 % 5 != 0 && (pos2 + 1) % 5 != 0)
                    {
                        foreach (Control y in Controls)
                        {
                            if (y.TabIndex == pos2)
                            {
                                y.BackColor = Color.Black;
                            }
                            if (y.TabIndex == (pos2 + 1))
                            {
                                y.BackColor = Color.Black;
                            }
                            if (y.TabIndex == (pos2 + 2))
                            {
                                y.BackColor = Color.Black;
                            }
                        }
                        posnotFound = false;
                    }
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    while (turnnotDecide)
                    {
                        turn2 = Int32.Parse(read.ReadLine());
                        if(turn2 == 1)
                        {
                            turn = 0;
                        }
                        else
                        {
                            turn = 1;
                        }
                        turnnotDecide = false;
                    }
                    positionTake = read.ReadLine();
                    int choicePos = Int32.Parse(positionTake);
                    if(choicePos < 26)
                    {
                        foreach (Control x in Controls)
                        {
                            if (x.TabIndex == choicePos)
                            {
                                x.Invoke(new MethodInvoker(delegate ()
                                {
                                    x.BackColor = Color.Red;
                                    x.Enabled = false;
                                }));
                                textBox1.Invoke(new MethodInvoker(delegate ()
                                {
                                    textBox1.Text = "Hit!";
                                }));
                                hitCount2++;
                                if (hitCount2 == 3)
                                {
                                    foreach (Control y in Controls)
                                    {
                                        y.Invoke(new MethodInvoker(delegate ()
                                        {
                                            y.Enabled = false;
                                        }));
                                    }
                                    textBox1.Invoke(new MethodInvoker(delegate ()
                                    {
                                        textBox1.Text = "Game Over!";
                                    }));
                                }
                            }
                        }
                    }
                    else if(choicePos > 200)
                    {
                        choicePos -= 250;
                        foreach (Control x in Controls)
                        {
                            if (x.TabIndex == choicePos)
                            {
                                x.Invoke(new MethodInvoker(delegate ()
                                {
                                    x.BackColor = Color.Gray;
                                    x.Enabled = false;
                                }));
                                textBox1.Invoke(new MethodInvoker(delegate ()
                                {
                                    textBox1.Text = "Miss!";
                                }));
                            }
                        }
                    }
                    else
                    {
                        if (turn > turn2)
                        {
                            foreach (Control x in Controls)
                            {
                                if (x.TabIndex == choicePos)
                                {
                                    if (x.BackColor == Color.Blue)
                                    {
                                        x.Invoke(new MethodInvoker(delegate ()
                                        {
                                            x.BackColor = Color.Gray;
                                        }));
                                        textBox1.Invoke(new MethodInvoker(delegate ()
                                        {
                                            textBox1.Text = ("They missed!");
                                        }));
                                        dataSender((x.TabIndex+200).ToString());
                                        turn--;
                                        turn2++;
                                    }
                                    else if(x.BackColor == Color.Black)
                                    {
                                        x.Invoke(new MethodInvoker(delegate ()
                                        {
                                            x.BackColor = Color.Red;
                                        }));
                                        //hitCount++;
                                        //System.Threading.Thread.Sleep(5000);
                                        textBox1.Invoke(new MethodInvoker(delegate ()
                                        {
                                            textBox1.Text = ("You've been hit!");
                                        }));
                                        dataSender((x.TabIndex - 50).ToString());
                                        hitCount++;
                                        if (hitCount == 3)
                                        {
                                            foreach (Control y in Controls)
                                            {
                                                y.Invoke(new MethodInvoker(delegate ()
                                                {
                                                    y.Enabled = false;
                                                }));
                                            }
                                            textBox1.Invoke(new MethodInvoker(delegate ()
                                            {
                                                textBox1.Text = "Game Over!";
                                            }));
                                        }
                                        turn--;
                                        turn2++;
                                    }
                                }
                                positionTake = "";
                            }
                        }
                    }
                }
                    
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private async void dataSender(String x)
        {
            await write.WriteLineAsync(x);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Button y = sender as Button;

            positionShoot = (y.TabIndex + 50).ToString();
            dataSender(positionShoot);

            if (turn < turn2)
            {
                turn++;
                turn2--;
            }
        }
    }
    }

