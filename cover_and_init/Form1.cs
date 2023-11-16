using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YuanLi_Logger;

namespace cover_and_init
{
    public partial class Form1 : Form
    {
        Logger Logger = new Logger("Logger");
        int still_log = 0;
        const int log_ferquence = 30; //Interval = 1000ms
        private bool isConnect = false;
        private string receivcommad;
        private bool isTesting;

        public Form1()
        {
            InitializeComponent();
        }
        #region socket 參數
        // Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        ClientCommunication clientCommunication;
        int port = 1000;
        string ip_address = "192.168.0.3";
        IPAddress address;
        //  List<string> contorl_command = new List<string>();
        #endregion

        #region 控制遮擋螢幕參數
        form_hide Form_Hide_1 = new form_hide();
        form_hide Form_Hide_2 = new form_hide();
        int Cover_Start_X1 = 100, Cover_Start_Y1 = 100, Cover_End_X1 = 500, Cover_End_Y1 = 400;
        int Cover_Start_X2 = 100, Cover_Start_Y2 = 100, Cover_End_X2 = 500, Cover_End_Y2 = 400;
        #endregion

        #region 滑鼠自動點集參數
        Point Sp1, Sp2, Sp3, Sp5;
        #endregion

        #region 視窗放大控制參數
        string OLS_Name = "", Windows_Name = "";
        #endregion

        #region 控制手動量測案鍵
        Form_hand_measurement_button form_btn = new Form_hand_measurement_button();

        #endregion

        bool form_min = false;

        private void Form1_Load(object sender, EventArgs e)
        {
            Logger.Write_Logger("Start Program");
            //防止開啟第二次
            if (System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                Logger.Write_Logger("This is second program");
                this.Close();
            }
            else
            {
                button_Open_Hide_Click(sender, e);
                button_Close_Hide_Click(sender, e);

                button_Open_Hide_2_Click(sender, e);
                button_Close_Hide_2_Click(sender, e);

                // load ip address
                if (File.Exists("test.txt"))
                {
                    StreamReader test_ip = new StreamReader("test.txt");
                    string test_ip_str = test_ip.ReadLine();
                    test_ip.Close();
                    address = String_To_IP_Address(test_ip_str);
                    isTesting = true;
                    Logger.Write_Logger("Test Mode");
                }
                else
                {
                    address = String_To_IP_Address(ip_address);
                }
                label_connect_type.Text = "not connect";

                Logger.Write_Logger("Started");
                clientCommunication = new ClientCommunication();

                clientCommunication.ReceiverMessage += Process_command;
                clientCommunication.ReceiverException += ReceiverException;
                ClientConnect();
                Form1.CheckForIllegalCrossThreadCalls = false;//為了讓 UI可以被別的執行續更新
                Form_hand_measurement_button.CheckForIllegalCrossThreadCalls = false;//為了讓 UI可以被別的執行續更新
                form_hide.CheckForIllegalCrossThreadCalls = false;

                if (!isTesting)
                    this.Visible = false;
                //socket.Connect(address, port);


            }
        }
        //TCP  Client端連線Server
        private void ClientConnect()
        {
            Task.Run(async () =>
            {
                Logger.Write_Logger("Connecting...");

                while (!isConnect)
                {

                    UpdateLabel("Connecting...", label_connect_type);
                    await Task.Delay(1000);
                    try
                    {
                        clientCommunication.Open(address, port);
                    }
                    catch (Exception)
                    {

                        Logger.Write_Logger("Connect Error...");
                        UpdateLabel("Try to connect socket server...", label_connect_type);
                        await Task.Delay(2000);

                        continue;
                    }

                    isConnect = true;
                    Logger.Write_Logger("Connected...");
                    UpdateLabel("Connected...", label_connect_type);

                    //  if (this.WindowState == FormWindowState.Minimized && !form_min)
                    {
                        form_min = true;
                        if (!isTesting)
                            Form1_SizeChanged(EventArgs.Empty, EventArgs.Empty);

                        Logger.Write_Logger("Minimized");
                    }
                }

            });


        }

        /*
         * private void timer_connect_socket_Tick(object sender, EventArgs e)
         {
             still_log++;
             if (this.WindowState == FormWindowState.Minimized && !form_min)
             {
                 form_min = true;
                 Form1_SizeChanged(sender, e);

                 Logger.Write_Logger("Minimized");
             }
             if (!backgroundWorker_connect_socket.IsBusy)
             {
                 try
                 {
                     //檢查socket 是否連線(已偵測斷線重連)
                     if (SocketConnected(socket))
                     {
                         label_connect_type.Text = "Connect";
                         timer_Receive.Enabled = true;
                         if(still_log > log_ferquence)
                         {
                             Logger.Write_Logger("Is Connected");
                             still_log = 0;
                         }
                     }
                     else
                     {
                         Logger.Write_Logger("Try to connect socket server");
                         backgroundWorker_connect_socket.RunWorkerAsync();
                     }
                 }
                 catch (Exception ee)
                 {
                 //    socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                     label_connect_type.Text = "Try to connect socket server...";
                     Logger.Write_Error_Logger("Try to connect socket server...");
                     //timer_Receive.Enabled = false;
                 }
             }
         }*/
        private void button_Open_Hide_Click(object sender, EventArgs e)
        {
            update_value();

            Form_Hide_1.Location = new Point(Cover_Start_X1, Cover_Start_Y1);
            Form_Hide_1.Size = new Size(
                Cover_End_X1 - Cover_Start_X1,
                Cover_End_Y1 - Cover_Start_Y1);

            Form_Hide_1.TopMost = true;
            Form_Hide_1.Visible = true;
            //Form_Hide_1.label_X.Visible = true;
            //Form_Hide_1.label_Y.Visible = true;
            timer_Mouse_Point.Enabled = true;

            Logger.Write_Logger("button_Open_Hide_Click");

        }
        private void button_Close_Hide_Click(object sender, EventArgs e)
        {
            //timer_Mouse_Point.Enabled = false;
            Form_Hide_1.Visible = false;
            //Form_Hide_1.label_X.Visible = false;
            //Form_Hide_1.label_Y.Visible = false;

            Logger.Write_Logger("button_Close_Hide_Click");

        }
        private void button_Open_Hide_2_Click(object sender, EventArgs e)
        {
            update_value();
            Form_Hide_2.Location = new Point(Cover_Start_X2, Cover_Start_Y2);
            Form_Hide_2.Size = new Size(
                Cover_End_X2 - Cover_Start_X2,
                Cover_End_Y2 - Cover_Start_Y2);

            Form_Hide_2.TopMost = true;
            Form_Hide_2.Visible = true;
            //Form_Hide_2.label_X.Visible = true;
            //Form_Hide_2.label_Y.Visible = true;
            timer_Mouse_Point.Enabled = true;

            Logger.Write_Logger("button_Open_Hide_2_Click");

        }
        private void button_Close_Hide_2_Click(object sender, EventArgs e)
        {
            //timer_Mouse_Point.Enabled = false;
            Form_Hide_2.Visible = false;
            //Form_Hide_2.label_X.Visible = false;
            //Form_Hide_2.label_Y.Visible = false;

            Logger.Write_Logger("button_Close_Hide_2_Click");

        }
        private void timer_Mouse_Point_Tick(object sender, EventArgs e)
        {
            //Form_Hide_1.label_X.Text = "X : " + Convert.ToString(Cursor.Position.X);
            //Form_Hide_1.label_Y.Text = "Y : " + Convert.ToString(Cursor.Position.Y);

            //Form_Hide_2.label_X.Text = "X : " + Convert.ToString(Cursor.Position.X);
            //Form_Hide_2.label_Y.Text = "Y : " + Convert.ToString(Cursor.Position.Y);
        }

        private void Process_command(string commad)
        {
            try
            {
                //監聽socket server端傳送過來的指令              
                if (OLS_Name != "")
                {
                    Maximum_Window(OLS_Name);
                }
                //  string commad = contorl_command.First();
                Logger.Write_Logger("client receive command:" + commad);

                if (commad.Length > 30)
                {
                    string[] split_str = commad.Split(',');
                    int sp1X = 0, sp1Y = 0, sp2X = 0, sp2Y = 0, sp3X = 0, sp3Y = 0, sp5X = 0, sp5Y = 0;
                    foreach (string str in split_str)
                    {
                        string[] info = str.Split(':');
                        Logger.Write_Logger($"{info[0]} : Start");
                        switch (info[0])
                        {
                            case "LUX1":
                                textBox_Cover_Start_X1.Text = info[1];
                                Cover_Start_X1 = Convert.ToInt32(info[1]);
                                break;
                            case "RDX1":
                                textBox_Cover_End_X1.Text = info[1];
                                Cover_End_X1 = Convert.ToInt32(info[1]);
                                break;
                            case "LUY1":
                                textBox_Cover_Start_Y1.Text = info[1];
                                Cover_Start_Y1 = Convert.ToInt32(info[1]);
                                break;
                            case "RDY1":
                                textBox_Cover_End_Y1.Text = info[1];
                                Cover_End_Y1 = Convert.ToInt32(info[1]);
                                break;
                            case "LUX2":
                                textBox_Cover_Start_X2.Text = info[1];
                                Cover_Start_X1 = Convert.ToInt32(info[1]);
                                break;
                            case "RDX2":
                                textBox_Cover_End_X2.Text = info[1];
                                Cover_End_X1 = Convert.ToInt32(info[1]);
                                break;
                            case "LUY2":
                                textBox_Cover_Start_Y2.Text = info[1];
                                Cover_Start_Y1 = Convert.ToInt32(info[1]);
                                break;
                            case "RDY2":
                                textBox_Cover_End_Y2.Text = info[1];
                                Cover_End_Y1 = Convert.ToInt32(info[1]);
                                break;
                            case "S1X":
                                textBox_Step_1_X.Text = info[1];
                                sp1X = Convert.ToInt32(info[1]);
                                break;
                            case "S1Y":
                                textBox_Step_1_Y.Text = info[1];
                                sp1Y = Convert.ToInt32(info[1]);
                                break;
                            case "S2X":
                                textBox_Step_2_X.Text = info[1];
                                sp2X = Convert.ToInt32(info[1]);
                                break;
                            case "S2Y":
                                textBox_Step_2_Y.Text = info[1];
                                sp2Y = Convert.ToInt32(info[1]);
                                break;
                            case "S3X":
                                textBox_Step_3_X.Text = info[1];
                                sp3X = Convert.ToInt32(info[1]);
                                break;
                            case "S3Y":
                                textBox_Step_3_Y.Text = info[1];
                                sp3Y = Convert.ToInt32(info[1]);
                                break;
                            case "S4s":
                                textBox_Step_4_Delay.Text = info[1];
                                break;
                            case "S5X":
                                textBox_Step_5_X.Text = info[1];
                                sp5X = Convert.ToInt32(info[1]);
                                break;
                            case "S5Y":
                                textBox_Step_5_Y.Text = info[1];
                                sp5Y = Convert.ToInt32(info[1]);
                                break;
                            case "S6s":
                                textBox_Step_6_Delay.Text = info[1];
                                break;
                            case "CS1":
                                checkBox_Step_1.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                break;
                            case "CS2":
                                checkBox_Step_2.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                break;
                            case "CS3":
                                checkBox_Step_3.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                break;
                            case "CS4":
                                checkBox_Step_4.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                break;
                            case "CS5":
                                checkBox_Step_5.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                break;
                            case "CS6":
                                checkBox_Step_6.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                break;
                            case "OLSNAME":
                                OLS_Name = info[1];
                                break;
                            case "WINDOWSNAME":
                                Windows_Name = info[1];
                                break;
                            case "HBX":
                                txt_hand_measurement_X.Text = info[1];
                                break;
                            case "HBY":
                                txt_hand_measurement_Y.Text = info[1];
                                break;
                            case "HBH":
                                txt_hand_measurement_H.Text = info[1];
                                break;
                            case "HBW":
                                txt_hand_measurement_W.Text = info[1];
                                break;
                        }
                        Logger.Write_Logger($"{info[0]} : Stop");
                    }
                    Sp1 = new Point(sp1X, sp1Y);
                    Sp2 = new Point(sp2X, sp2Y);
                    Sp3 = new Point(sp3X, sp3Y);
                    Sp5 = new Point(sp5X, sp5Y);
                }
                else
                {
                    Logger.Write_Logger($"{commad} : Start");



                    switch (commad)
                    {
                        case "open_1":

                            button_Open_Hide_Click(commad, EventArgs.Empty);

                            break;
                        case "close_1":

                            button_Close_Hide_Click(commad, EventArgs.Empty);

                            break;
                        case "open_2":
                            button_Open_Hide_2_Click(commad, EventArgs.Empty);
                            break;
                        case "close_2":
                            button_Close_Hide_2_Click(commad, EventArgs.Empty);
                            break;
                        case "SP1":
                            Cursor.Position = Sp1;
                            LeftClick();
                            break;
                        case "SP2":
                            Cursor.Position = Sp2;
                            LeftClick();
                            break;
                        case "SP3":
                            Cursor.Position = Sp3;
                            LeftClick();
                            break;
                        case "SP5":
                            Cursor.Position = Sp5;
                            LeftClick();
                            break;
                        case "open_hb":
                            btn_open_hand_measurement_button_Click(commad, EventArgs.Empty);
                            break;
                        case "close_hb":
                            btn_close_hand_measurement_button_Click(commad, EventArgs.Empty);
                            break;


                        case "close_2open_hb":
                            button_Close_Hide_2_Click(commad, EventArgs.Empty);
                            Thread.Sleep(200);
                            btn_open_hand_measurement_button_Click(commad, EventArgs.Empty);
                            break;
                        case "open_hbclose_2":
                            button_Close_Hide_2_Click(commad, EventArgs.Empty);

                            btn_open_hand_measurement_button_Click(commad, EventArgs.Empty);
                            break;


                        case "heartbeat":
                            clientCommunication.Send("onLine");
                            break;


                        default:

                            if (commad.Contains("close_2"))
                                button_Close_Hide_2_Click(commad, EventArgs.Empty);

                            Thread.Sleep(200);

                            if (commad.Contains("open_hb"))
                                btn_open_hand_measurement_button_Click(commad, EventArgs.Empty);
                            break;
                    }



                    Logger.Write_Logger($"{commad} : Stop");
                }
                //   contorl_command.RemoveAt(0);

            }
            catch (Exception ex)
            {
                textBox_Receive.Text += ex.ToString();
                Logger.Write_Error_Logger(ex.ToString());
                //   contorl_command.RemoveAt(0);
            }


        }


        private void ReceiverException(Exception ex)
        {
            isConnect = false;
            Logger.Write_Error_Logger(ex.ToString());
            UpdateLabel("not connect...", label_connect_type);
            form_min = false;
            Form1_SizeReChanged(EventArgs.Empty, EventArgs.Empty);
            clientCommunication.Dispose();
            clientCommunication.Restart();
            ClientConnect();

        }


        /*private void timer_Receive_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorker_Receive_Data.IsBusy)
            {
                backgroundWorker_Receive_Data.RunWorkerAsync();
            }
            
        }*/

        /* private void backgroundWorker_Receive_Data_DoWork(object sender, DoWorkEventArgs e)
         {
             try
             {
                 byte[] Receive_data = new byte[256];
                 socket.Receive(Receive_data);
                 string receive_data = "";
                 for (int i = 0; i < 256; i++)
                 {
                     if (Receive_data[i] == 0)
                         break;
                     else
                     {
                         receive_data += Convert.ToString(Convert.ToChar(Receive_data[i]));
                     }
                 }
                 //接收資料進行對應動作
                 contorl_command.Add(receive_data);

                 Logger.Write_Logger("Real time receive : " + receive_data);

                 receive_data += "\r\n";
                 UpdateTextbox("Receive: " + receive_data, textBox_Receive);
             }
             catch (Exception error)
             {
                 UpdateTextbox(error.Message.ToString() + "\r\n", textBox_Receive);
                 Logger.Write_Error_Logger("Error " + error.Message.ToString());
                 int aaa = error.HResult;
                 if (aaa == -2147467259)
                 {
                     Logger.Write_Error_Logger("Connect Lose, Creat New Connect. " + error.Message.ToString());
                     socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                 }
                 timer_Receive.Enabled = false;
                 //timer_connect_socket.Enabled = true;
             }
         }*/

        /*private void backgroundWorker_connect_socket_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //client連線指定server
                socket.Connect(address, port);
                Logger.Write_Logger("Connect socket server success");
            }
            catch (Exception ee)
            {
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                UpdateLabel("Try to connect socket server...", label_connect_type);
                Logger.Write_Error_Logger("Try to connect socket server..." + ee.Message.ToString());
                timer_Receive.Enabled = false;
            }
        }*/

        #region Sub Function
        byte[] StringToByteArray(string str)
        {
            byte[] send_data = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                send_data[i] = Convert.ToByte(str[i]);
            }
            return send_data;
        }
        #region UI update
        private delegate void UpdateUITextbox(string value, TextBox ctl);

        private void UpdateTextbox(string value, TextBox ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUITextbox uu = new UpdateUITextbox(UpdateTextbox);
                this.BeginInvoke(uu, value, ctl);
            }
            else
            {
                ctl.Text += value;
            }
        }


        private delegate void UpdateUIFormHandShow(Form ctl);

        private void UpdateFormHandShow(Form ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUIFormHandShow uu = new UpdateUIFormHandShow(UpdateFormHandShow);
                this.BeginInvoke(uu, ctl);
            }
            else
            {
                int form_btn_X = Convert.ToInt32(txt_hand_measurement_X.Text);
                int form_btn_Y = Convert.ToInt32(txt_hand_measurement_Y.Text);
                int form_btn_H = Convert.ToInt32(txt_hand_measurement_H.Text);
                int form_btn_W = Convert.ToInt32(txt_hand_measurement_W.Text);
                ctl.Location = new Point(form_btn_X, form_btn_Y);
                ctl.Height = form_btn_H;
                ctl.Width = form_btn_W;
                ctl.TopMost = true;
                ctl.Visible = true;
                Common.is_hand_mode = false;
            }
        }
        private delegate void UpdateUIFormHandHide(bool value, Form ctl);

        private void UpdateFormHandVisible(bool value, Form ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUIFormHandHide uu = new UpdateUIFormHandHide(UpdateFormHandVisible);
                this.BeginInvoke(uu, ctl);
            }
            else
            {
                ctl.Visible = value;
            }
        }
        private delegate void UpdateUILabel(string value, Label ctl);

        private void UpdateLabel(string value, Label ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUILabel uu = new UpdateUILabel(UpdateLabel);
                this.BeginInvoke(uu, value, ctl);
            }
            else
            {
                ctl.Text = value;
            }
        }

        private delegate void UpdateUIFormOpen_Hide(Point point, Size size, Form form);

        private void UpdateFormOpen_Hide(Point point, Size size, Form form)
        {
            if (this.InvokeRequired)
            {
                UpdateUIFormOpen_Hide uu = new UpdateUIFormOpen_Hide(UpdateFormOpen_Hide);
                this.BeginInvoke(uu, point, size);
            }
            else
            {
                form.Location = point;
                form.Size = size;
            }
        }
        #endregion

        IPAddress String_To_IP_Address(string ip_address)
        {
            byte[] add = new byte[4];
            int i = 0;
            foreach (string item in ip_address.Split('.'))
            {
                add[i++] = Convert.ToByte(item);
            }
            IPAddress address = new IPAddress(add);
            return address;
        }
        private void timer_check_Tick(object sender, EventArgs e)
        {
            if (Common.is_hand_mode)
            {
                Common.is_hand_mode = false;
                //傳送手動模式開啟到SPIL 
                //string send_str = "hand measurement on";
                // var a = StringToByteArray("hand measurement on");
                clientCommunication.Send("hand measurement on");
            }
        }

        /*  private void timer_process_command_Tick(object sender, EventArgs e)
          {
              try
              {
                  //監聽socket server端傳送過來的指令
                  if (contorl_command.Count > 0)
                  {
                      if (OLS_Name != "")
                      {
                          Maximum_Window(OLS_Name);
                      }
                      string commad = contorl_command.First();
                      Logger.Write_Logger("client receive command:" + commad);

                      if (commad.Length > 30)
                      {
                          string[] split_str = commad.Split(',');
                          int sp1X = 0, sp1Y = 0, sp2X = 0, sp2Y = 0, sp3X = 0, sp3Y = 0, sp5X = 0, sp5Y = 0;
                          foreach (string str in split_str)
                          {
                              string[] info = str.Split(':');
                              Logger.Write_Logger($"{info[0]} : Start");
                              switch (info[0])
                              {
                                  case "LUX1":
                                      textBox_Cover_Start_X1.Text = info[1];
                                      Cover_Start_X1 = Convert.ToInt32(info[1]);
                                      break;
                                  case "RDX1":
                                      textBox_Cover_End_X1.Text = info[1];
                                      Cover_End_X1 = Convert.ToInt32(info[1]);
                                      break;
                                  case "LUY1":
                                      textBox_Cover_Start_Y1.Text = info[1];
                                      Cover_Start_Y1 = Convert.ToInt32(info[1]);
                                      break;
                                  case "RDY1":
                                      textBox_Cover_End_Y1.Text = info[1];
                                      Cover_End_Y1 = Convert.ToInt32(info[1]);
                                      break;
                                  case "LUX2":
                                      textBox_Cover_Start_X2.Text = info[1];
                                      Cover_Start_X1 = Convert.ToInt32(info[1]);
                                      break;
                                  case "RDX2":
                                      textBox_Cover_End_X2.Text = info[1];
                                      Cover_End_X1 = Convert.ToInt32(info[1]);
                                      break;
                                  case "LUY2":
                                      textBox_Cover_Start_Y2.Text = info[1];
                                      Cover_Start_Y1 = Convert.ToInt32(info[1]);
                                      break;
                                  case "RDY2":
                                      textBox_Cover_End_Y2.Text = info[1];
                                      Cover_End_Y1 = Convert.ToInt32(info[1]);
                                      break;
                                  case "S1X":
                                      textBox_Step_1_X.Text = info[1];
                                      sp1X = Convert.ToInt32(info[1]);
                                      break;
                                  case "S1Y":
                                      textBox_Step_1_Y.Text = info[1];
                                      sp1Y = Convert.ToInt32(info[1]);
                                      break;
                                  case "S2X":
                                      textBox_Step_2_X.Text = info[1];
                                      sp2X = Convert.ToInt32(info[1]);
                                      break;
                                  case "S2Y":
                                      textBox_Step_2_Y.Text = info[1];
                                      sp2Y = Convert.ToInt32(info[1]);
                                      break;
                                  case "S3X":
                                      textBox_Step_3_X.Text = info[1];
                                      sp3X = Convert.ToInt32(info[1]);
                                      break;
                                  case "S3Y":
                                      textBox_Step_3_Y.Text = info[1];
                                      sp3Y = Convert.ToInt32(info[1]);
                                      break;
                                  case "S4s":
                                      textBox_Step_4_Delay.Text = info[1];
                                      break;
                                  case "S5X":
                                      textBox_Step_5_X.Text = info[1];
                                      sp5X = Convert.ToInt32(info[1]);
                                      break;
                                  case "S5Y":
                                      textBox_Step_5_Y.Text = info[1];
                                      sp5Y = Convert.ToInt32(info[1]);
                                      break;
                                  case "S6s":
                                      textBox_Step_6_Delay.Text = info[1];
                                      break;
                                  case "CS1":
                                      checkBox_Step_1.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                      break;
                                  case "CS2":
                                      checkBox_Step_2.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                      break;
                                  case "CS3":
                                      checkBox_Step_3.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                      break;
                                  case "CS4":
                                      checkBox_Step_4.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                      break;
                                  case "CS5":
                                      checkBox_Step_5.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                      break;
                                  case "CS6":
                                      checkBox_Step_6.Checked = Convert.ToBoolean(Convert.ToInt32(info[1]));
                                      break;
                                  case "OLSNAME":
                                      OLS_Name = info[1];
                                      break;
                                  case "WINDOWSNAME":
                                      Windows_Name = info[1];
                                      break;
                                  case "HBX":
                                      txt_hand_measurement_X.Text = info[1];
                                      break;
                                  case "HBY":
                                      txt_hand_measurement_Y.Text = info[1];
                                      break;
                                  case "HBH":
                                      txt_hand_measurement_H.Text = info[1];
                                      break;
                                  case "HBW":
                                      txt_hand_measurement_W.Text = info[1];
                                      break;
                              }
                              Logger.Write_Logger($"{info[0]} : Stop");
                          }
                          Sp1 = new Point(sp1X, sp1Y);
                          Sp2 = new Point(sp2X, sp2Y);
                          Sp3 = new Point(sp3X, sp3Y);
                          Sp5 = new Point(sp5X, sp5Y);
                      }
                      else
                      {
                          Logger.Write_Logger($"{commad} : Start");
                          switch (commad)
                          {
                              case "open_1":

                                  button_Open_Hide_Click(sender, e);

                                  break;
                              case "close_1":

                                  button_Close_Hide_Click(sender, e);

                                  break;
                              case "open_2":
                                  button_Open_Hide_2_Click(sender, e);
                                  break;
                              case "close_2":
                                  button_Close_Hide_2_Click(sender, e);
                                  break;
                              case "SP1":
                                  Cursor.Position = Sp1;
                                  LeftClick();
                                  break;
                              case "SP2":
                                  Cursor.Position = Sp2;
                                  LeftClick();
                                  break;
                              case "SP3":
                                  Cursor.Position = Sp3;
                                  LeftClick();
                                  break;
                              case "SP5":
                                  Cursor.Position = Sp5;
                                  LeftClick();
                                  break;
                              case "open_hb":
                                  btn_open_hand_measurement_button_Click(sender, e);
                                  break;
                              case "close_hb":
                                  btn_close_hand_measurement_button_Click(sender, e);
                                  break;
                          }
                          Logger.Write_Logger($"{commad} : Stop");
                      }
                      contorl_command.RemoveAt(0);
                  }
              }
              catch (Exception ex)
              {
                  textBox_Receive.Text += ex.ToString();
                  Logger.Write_Error_Logger(ex.ToString());
                  contorl_command.RemoveAt(0);
              }

          }
        */


        /// <summary>
        /// 檢查socket連線是否連接
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        bool SocketConnected(Socket s)
        {

            var isConnected = s.Connected;

            return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
        }
        /// <summary>
        /// 更新視窗遮擋座標參數
        /// </summary>
        void update_value()
        {
            Cover_Start_X1 = Convert.ToInt32(textBox_Cover_Start_X1.Text);
            Cover_Start_Y1 = Convert.ToInt32(textBox_Cover_Start_Y1.Text);
            Cover_End_X1 = Convert.ToInt32(textBox_Cover_End_X1.Text);
            Cover_End_Y1 = Convert.ToInt32(textBox_Cover_End_Y1.Text);
            Cover_Start_X2 = Convert.ToInt32(textBox_Cover_Start_X2.Text);
            Cover_Start_Y2 = Convert.ToInt32(textBox_Cover_Start_Y2.Text);
            Cover_End_X2 = Convert.ToInt32(textBox_Cover_End_X2.Text);
            Cover_End_Y2 = Convert.ToInt32(textBox_Cover_End_Y2.Text);
        }


        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        public const uint WM_SYSCOMMAND2 = 0x0112;
        public const uint SC_MAXIMIZE2 = 0xF030;
        /// <summary>
        /// 最大化應用程式視窗
        /// </summary>
        /// <param name="process_name">視窗名稱</param>
        void Maximum_Window(string process_name)
        {
            Process[] processes = Process.GetProcessesByName(process_name);
            if (processes.Length > 0)
            {
                for (int i = 0; i < processes.Length; i++)
                {
                    IntPtr handle = processes[i].MainWindowHandle;
                    SendMessage(handle, WM_SYSCOMMAND2, new IntPtr(SC_MAXIMIZE2), IntPtr.Zero); // 最大化
                    SwitchToThisWindow(handle, true);	// 激活
                }
            }
            //
        }
        private void btn_close_hand_measurement_button_Click(object sender, EventArgs e)
        {
            // UpdateFormHandVisible(false, form_btn);
            form_btn.Visible = false;
        }

        private void btn_open_hand_measurement_button_Click(object sender, EventArgs e)
        {
            int aaa = Thread.CurrentThread.ManagedThreadId;
            UpdateFormHandShow(form_btn);
            //int form_btn_X = Convert.ToInt32(txt_hand_measurement_X.Text);
            //int form_btn_Y = Convert.ToInt32(txt_hand_measurement_Y.Text);
            //int form_btn_H = Convert.ToInt32(txt_hand_measurement_H.Text);
            //int form_btn_W = Convert.ToInt32(txt_hand_measurement_W.Text);
            //form_btn.Location = new Point(form_btn_X, form_btn_Y);
            //form_btn.Height = form_btn_H;
            //form_btn.Width = form_btn_W;
            //form_btn.TopMost = true;
            //form_btn.Visible = true;
            //Common.is_hand_mode = false;
        }
        #region mouse
        [DllImport("user32.dll", SetLastError = true)]
        public static extern Int32 SendInput(Int32 cInputs, ref INPUT pInputs, Int32 cbSize);

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 28)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public INPUTTYPE dwType;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public KEYBOARDINPUT ki;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MOUSEINPUT
        {
            public Int32 dx;
            public Int32 dy;
            public Int32 mouseData;
            public MOUSEFLAG dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct KEYBOARDINPUT
        {
            public Int16 wVk;
            public Int16 wScan;
            public KEYBOARDFLAG dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HARDWAREINPUT
        {
            public Int32 uMsg;
            public Int16 wParamL;
            public Int16 wParamH;
        }

        public enum INPUTTYPE : int
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }




        [Flags()]
        public enum MOUSEFLAG : int
        {
            MOVE = 0x1,
            LEFTDOWN = 0x2,
            LEFTUP = 0x4,
            RIGHTDOWN = 0x8,
            RIGHTUP = 0x10,
            MIDDLEDOWN = 0x20,
            MIDDLEUP = 0x40,
            XDOWN = 0x80,
            XUP = 0x100,
            VIRTUALDESK = 0x400,
            WHEEL = 0x800,
            ABSOLUTE = 0x8000
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.notifyIcon_smallest.Visible = false;
            notifyIcon_smallest.Dispose();
        }



        [Flags()]
        public enum KEYBOARDFLAG : int
        {
            EXTENDEDKEY = 1,
            KEYUP = 2,
            UNICODE = 4,
            SCANCODE = 8
        }
        //
        //
        public void LeftDown()
        {
            INPUT leftdown = new INPUT();

            leftdown.dwType = 0;
            leftdown.mi = new MOUSEINPUT();
            leftdown.mi.dwExtraInfo = IntPtr.Zero;
            leftdown.mi.dx = 0;
            leftdown.mi.dy = 0;
            leftdown.mi.time = 0;
            leftdown.mi.mouseData = 0;
            leftdown.mi.dwFlags = MOUSEFLAG.LEFTDOWN;

            SendInput(1, ref leftdown, Marshal.SizeOf(typeof(INPUT)));
        }
        public void LeftUp()
        {
            INPUT leftup = new INPUT();

            leftup.dwType = 0;
            leftup.mi = new MOUSEINPUT();
            leftup.mi.dwExtraInfo = IntPtr.Zero;
            leftup.mi.dx = 0;
            leftup.mi.dy = 0;
            leftup.mi.time = 0;
            leftup.mi.mouseData = 0;
            leftup.mi.dwFlags = MOUSEFLAG.LEFTUP;

            SendInput(1, ref leftup, Marshal.SizeOf(typeof(INPUT)));
        }
        public void LeftClick()
        {
            LeftDown();
            Thread.Sleep(20);
            LeftUp();

            Logger.Write_Logger("LeftClick");

        }
        public void RightDown()
        {
            INPUT leftdown = new INPUT();

            leftdown.dwType = 0;
            leftdown.mi = new MOUSEINPUT();
            leftdown.mi.dwExtraInfo = IntPtr.Zero;
            leftdown.mi.dx = 0;
            leftdown.mi.dy = 0;
            leftdown.mi.time = 0;
            leftdown.mi.mouseData = 0;
            leftdown.mi.dwFlags = MOUSEFLAG.RIGHTDOWN;

            SendInput(1, ref leftdown, Marshal.SizeOf(typeof(INPUT)));
        }
        public void RightUp()
        {
            INPUT leftup = new INPUT();

            leftup.dwType = 0;
            leftup.mi = new MOUSEINPUT();
            leftup.mi.dwExtraInfo = IntPtr.Zero;
            leftup.mi.dx = 0;
            leftup.mi.dy = 0;
            leftup.mi.time = 0;
            leftup.mi.mouseData = 0;
            leftup.mi.dwFlags = MOUSEFLAG.RIGHTUP;

            SendInput(1, ref leftup, Marshal.SizeOf(typeof(INPUT)));
        }
        public void RightClick()
        {
            RightDown();
            Thread.Sleep(20);
            RightUp();

            Logger.Write_Logger("RightClick");

        }
        public void LeftDoubleClick()
        {
            LeftClick();
            Thread.Sleep(50);
            LeftClick();

            Logger.Write_Logger("LeftDoubleClick");

        }
        #endregion

        #endregion

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                this.notifyIcon_smallest.Visible = true;
            }
        }

        private void Form1_SizeReChanged(object sender, EventArgs e)
        {

            this.Visible = true;


        }
    }



    public class ClientCommunication
    {
        private TcpClient tcpClient;
        private bool isReceiver;
        private string message;
        private NetworkStream stream;
        private byte[] buffer = new byte[1024];
        private Task receiverTask = Task.CompletedTask;
        private IPAddress ipAddress;
        private int port;

        public ClientCommunication()
        {

            tcpClient = new TcpClient();

        }

        public ClientCommunication(IPAddress ipAddress, int port)
        {

            tcpClient = new TcpClient();
            this.ipAddress = ipAddress;
            this.port = port;

            tcpClient.Connect(ipAddress, port);
            isReceiver = true;
            receiverTask = Task.Run(Receiver);

        }
        public bool IsReceiver { get => isReceiver; }
        public event Action<string> ReceiverMessage;
        public event Action<Exception> ReceiverException;

        public void Open(IPAddress ipAddress, int port)
        {
            try
            {
                this.ipAddress = ipAddress;
                this.port = port;

                if (!tcpClient.Connected)
                    tcpClient.Connect(ipAddress, port);
                isReceiver = true;
                receiverTask = Task.Run(Receiver);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void Close()
        {
            stream.Close();
            if (isReceiver)
                isReceiver = false;


            tcpClient.Close();
        }
        public void Dispose()
        {

            Close();
            tcpClient.Dispose();


        }
        public void Restart()
        {
            tcpClient = new TcpClient();
        }


        public void Send(string message)
        {

            NetworkStream stream = tcpClient.GetStream();

            // 傳送指定的 Home 訊息給 B 機器        
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);

        }

        private async Task Receiver()
        {

            try
            {
                isReceiver = true;

                while (isReceiver)
                {
                    stream = tcpClient.GetStream();

                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    // 將接收到的訊息轉換為字串
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Console.WriteLine("接收到訊息: " + message);

                    if (message == "errTest123") throw new Exception($"Error Test . ThreadID :{Thread.CurrentThread.ManagedThreadId }");
                    // 執行相應的動作，例如切換 Home 狀態
                    ReceiverMessage?.Invoke(message);


                    await Task.Delay(200);
                }
                // 關閉客戶端連線
                tcpClient.Close();



            }
            catch (Exception error)
            {
                isReceiver = false;
                ReceiverException?.Invoke(error);

            }
        }

    }
}
