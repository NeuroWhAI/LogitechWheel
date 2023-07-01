using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogitechWheel
{
    internal class Program
    {
        static bool Running = false;

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

                Stop();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ReadKey();
            }
        }

        static void Start()
        {
            LogitechGSDK.LogiSteeringInitialize(false);
            Console.WriteLine("Started");
        }

        static void Update()
        {
            if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
            {
                var s = LogitechGSDK.LogiGetStateCSharp(0);
                Console.WriteLine($"X: ${s.lX:F3}, Y: ${s.lY:F3}, Z: ${s.lZ:F3}");
            }
        }

        static void Stop()
        {
            Console.WriteLine("Stopping");
            LogitechGSDK.LogiSteeringShutdown();
        }
    }
}
