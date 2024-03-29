﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        
        public Socket clientSocket;
        public string strName;

        public delegate string getNameDelegate();
        public delegate void UjFormDelegate(); 

        public MainWindow()
        {
            InitializeComponent();
        }

        public string getLoginName() 
        {
            return this.textBox1.Text;
        }

        public string getIP()
        {
            return this.textBox2.Text;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string l_ip;
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);                
                //IPAddress ipAddress = IPAddress.Parse(this.textBox2.Text);

                //getNameDelegate IP = new getNameDelegate(getIP);
                //l_ip = (string)this.Dispatcher.Invoke(IP, null);
                IPAddress ipAddress = IPAddress.Parse(this.textBox2.Text);
                //Server is listening on port 1000
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 1000);

                //Connect to the server
                //clientSocket.Connect(ipEndPoint);
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient");
            } 
        }

        private void OnReceive(IAsyncResult ar)
        { }

        private void OnSend(IAsyncResult ar)
        {
            try
            {
                
                clientSocket.EndSend(ar);
                byte[] byteData = new byte[1024];
                
                //Várunk a válaszra
                clientSocket.Receive(byteData,0,1024,SocketFlags.None);
                
                Data msg = new Data(byteData);
                
                UjFormDelegate pForm = new UjFormDelegate(UjForm);
                this.Dispatcher.Invoke(pForm, null);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient");
            }
        }

        private void UjForm()
        {
            CliensMessage uj_form;
            uj_form = new CliensMessage(clientSocket,textBox1.Text);
            uj_form.Show();
            Close();
        }

        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);

                //We are connected so we login into the server
                string l_fhName;
                Data msgToSend = new Data();
                msgToSend.cmdCommand = Command.Login;

                //l_fhName = this.textBox1.Text;
                getNameDelegate fhName = new getNameDelegate(getLoginName);
                l_fhName = (string)this.textBox1.Dispatcher.Invoke(fhName, null);

                
                msgToSend.strName = l_fhName;
                msgToSend.strMessage = null;

                byte[] b = msgToSend.ToByte();

                //Send the message to the server
                clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
           

    }
}
