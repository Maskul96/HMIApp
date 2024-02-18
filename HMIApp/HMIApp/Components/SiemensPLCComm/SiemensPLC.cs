﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static libnodave;

namespace HMIApp
{
    //Klasa do obsługi komunikacji z PLC oraz odczytu/zapisu danych z DB
    public class SiemensPLC : iSiemensPLC
    {
        //propercje
        public int Rack { get; private set; }
        public int Slot { get; private set; }
        public string IP { get; private set; }
        public int Port { get; private set; }
        public int Timeout_ms { get; private set; }


        //zmienne
        private const int kMaxErrorsToReset = 10000;
        private bool connected_;
        private int continuous_reading_errors_;
        private int continuous_writing_errors_;
        private libnodave.daveOSserialType dave_serial_;
        public libnodave.daveInterface dave_interface_;
        public libnodave.daveConnection dave_connection_;
        private object cs_;
        public Form1 obj;

        //konstruktor bezparametrowy
        public SiemensPLC()
        {

        }

        public SiemensPLC(Form1 obj)
        {
            this.obj = obj;
        }

        //konstruktor parametrowy
        public SiemensPLC(string IP, int Port, int Rack, int Slot, int Timeout_ms)
        {
            this.IP = IP;
            this.Port = Port;
            this.Rack = Rack;
            this.Slot = Slot;
            this.Timeout_ms = Timeout_ms;
            connected_ = false;
        }

        //Inicjalizacja połączenia
        public bool Init()
        {
            try
            {
                cs_ = new object();
                if (!connected_)
                {
                    dave_serial_.rfd = libnodave.openSocket(Port, IP);
                    dave_serial_.wfd = dave_serial_.rfd;
                    if (dave_serial_.rfd > 0)
                    {
                        dave_interface_ = new libnodave.daveInterface(dave_serial_, "", 0, libnodave.daveProtoISOTCP, 0);
                        dave_interface_.setTimeout(Timeout_ms);
                        if (dave_interface_.initAdapter() == 0)
                        {
                            dave_connection_ = new libnodave.daveConnection(dave_interface_, 0, Rack, Slot);
                            int num = dave_connection_.connectPLC();
                            if (num == 0)
                            {
                                //MessageBox.Show($"CONNECTED,IP: {IP} port: {Port} ");
                                Form1._Form1.textBox4.Text = $"CONNECTED,IP: {IP} port: {Port} ";
                                connected_ = true;
                                return true;
                            }
                            Form1._Form1.textBox4.Text = ($"Error connecting to PLC: {libnodave.daveStrerror(num)} ");
                            return false;
                        }
                        Form1._Form1.textBox4.Text = ($"Error initAdapter socket,IP: {IP} port: {Port}");
                        return false;
                    }
                    Form1._Form1.textBox4.Text = ("Error opening socket, IP: {IP} port: {Port}");
                    return false;
                }
                Form1._Form1.textBox4.Text = ("Already connected");
                return false;
            }
            catch (Exception ex)
            {
                Form1._Form1.textBox4.Text = ($"{ex.ToString()}");
                return false;
            }
        }

        //Zamknięcie połączenia
        public bool Close()
        {
            try
            {
                if (connected_)
                {
                    if (dave_connection_.disconnectPLC() == 0)
                    {
                        dave_interface_.disconnectAdapter();
                        libnodave.closeSocket(dave_serial_.rfd);
                        connected_ = false;
                        return true;
                    }
                    Form1._Form1.textBox4.Text = ("Error in disconnect with PLC");
                    return false;
                }
                Form1._Form1.textBox4.Text = ("PLC not connected");
                return false;
            }
            catch (Exception ex)
            {
                Form1._Form1.textBox4.Text = ($"{ex.ToString()}");
                return false;
            }
        }

        //Mozna zrobic tutaj delegaty do Read i Write?
        //Odczyt z DB
        public bool Read(int db, int start, int len, byte[] bytes)
        {

            try
            {
                //instrukcja lock zapewnia ze w dowolny momencie tylko jeden watek wykonuje jego tresc - zgodnie z dokumentacja biblioteki
                lock (cs_)
                {
                    int num = dave_connection_.readBytes(libnodave.daveDB, db, start, len, bytes);

                    if (num == 0)
                    {
                        continuous_reading_errors_ = 0;
                        return true;
                    }
                    AddContinuousReadingErrors();
                    Form1._Form1.textBox4.Text = ($"Error reading: DB: {db}, start: {start}, len: {len}, Error: {libnodave.daveStrerror(num)}");

                    return false;
                }
            }
            catch (Exception ex)
            {
                AddContinuousReadingErrors();
                Form1._Form1.textBox4.Text = ($"{ex.ToString()}");
                return false;
            }
        }



        //Zapis do DB Byte'ow
        public bool Write(int db, int start, int len, byte[] bytes)
        {
            try
            {
                lock (cs_)
                {
                    int num = dave_connection_.writeBytes(libnodave.daveDB, db, start, len, bytes);
                    if (num == 0)
                    {
                        continuous_writing_errors_ = 0;
                        return true;
                    }
                    AddContinuousWritingErrors();
                    Form1._Form1.textBox4.Text = ($"Error writing: DB: {db}, start: {start}, len: {len}, Error: {libnodave.daveStrerror(num)}");

                    return false;
                }
            }
            catch (Exception ex)
            {
                Form1._Form1.textBox4.Text = ("Blad tutaj");
                AddContinuousWritingErrors();
                Form1._Form1.textBox4.Text = ($"{ex.ToString()}");
                return false;
            }
        }

        public int continuous_reading_errors()
        {
            return continuous_reading_errors_;
        }

        public int continuous_writing_errors()
        {
            return continuous_writing_errors_;
        }

        private void AddContinuousReadingErrors()
        {
            if (continuous_reading_errors_ > 10000)
            {
                continuous_reading_errors_ = 0;
            }
            else
            {
                continuous_reading_errors_++;
            }
        }

        private void AddContinuousWritingErrors()
        {
            if (continuous_writing_errors_ > 10000)
            {
                continuous_writing_errors_ = 0;
            }
            else
            {
                continuous_writing_errors_++;
            }
        }
    }
}
