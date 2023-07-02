using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;

namespace LogitechWheel
{
    internal class Program
    {
        static bool Running = false;
        static SerialPort serial = null;

        static void Main(string[] args)
        {
            try
            {
                Console.CancelKeyPress += (s, e) =>
                {
                    e.Cancel = true;
                    Running = false;
                };

                Start();

                Running = true;

                while (Running)
                {
                    Update();

                    Thread.Sleep(16);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ReadKey();
            }
            finally
            {
                Stop();
            }
        }

        static void Start()
        {
            Console.Write("Use serial(y/n) : ");
            string useSerial = Console.ReadLine();

            if (useSerial.ToLowerInvariant() == "y")
            {
                Console.Write("Port(ex: COM12) : ");
                string port = Console.ReadLine();

                Console.Write("Baudrate(ex: 9600) : ");
                int baudrate = int.Parse(Console.ReadLine());

                serial = new SerialPort(port, baudrate);
                serial.Open();
                Console.WriteLine("Port opened");
            }

            LogitechGSDK.LogiSteeringInitialize(false);

            Console.WriteLine("Started");
        }

        static void Update()
        {
            if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
            {
                var s = LogitechGSDK.LogiGetStateCSharp(0);
                Console.WriteLine($"#XAXIS,{s.lX}@");
                Console.WriteLine($"#YAXIS,{s.lY}@");
                Console.WriteLine($"#SLIDER,{s.rglSlider[0]}@");
                Console.WriteLine($"#BTN14,{s.rgbButtons[14]}@");
                Console.WriteLine($"#BTN15,{s.rgbButtons[15]}@");
                Console.WriteLine();

                if (serial != null)
                {
                    serial.Write($"#XAXIS,{s.lX}@");
                    serial.Write($"#YAXIS,{s.lY}@");
                    serial.Write($"#SLIDER,{s.rglSlider[0]}@");
                    serial.Write($"#BTN14,{s.rgbButtons[14]}@");
                    serial.Write($"#BTN15,{s.rgbButtons[15]}@");
                }
            }
        }

        static void Stop()
        {
            Console.WriteLine("Stopping");

            LogitechGSDK.LogiSteeringShutdown();

            if (serial != null)
            {
                serial.Close();
                serial.Dispose();
                serial = null;
            }
        }
    }
}
