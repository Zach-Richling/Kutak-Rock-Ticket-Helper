using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Timers;

namespace KutakRock {
    public class Ticket_Helper {

        public class MachineInfo
        {
            public string id = "0";
            public string ip = "";
            public string machineName = "";
            public string OSName = "";
            public string OSVersion = "";
            public string userName = "";
            public string manufacturer = "";
            public string model = "";
            public string BIOSNumber = "";
            public string BIOSVersion = "";
            public string macAddress = "";
            public string ramAmount = "";

        }
            private readonly Timer _timer;
        private string currentIp = new WebClient().DownloadString("https://api.ipify.org");
        WebRequest webRequest = WebRequest.Create(new Uri("www.google.com"));
        
        public Ticket_Helper(int repeat) {
            _timer = new Timer(repeat) { AutoReset = true};
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e) {
            ensureIPCorrectness();
        }

        private void pushIP()
        {

        }
        //Will return true if External IP changed.
        private bool ensureIPCorrectness() {
            string externalip = "";
            try {
                externalip = new WebClient().DownloadString("https://api.ipify.org");
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            if (externalip != currentIp) {
                Console.WriteLine("IP Changed from {0} to {1}.", currentIp, externalip);
                currentIp = externalip;
                Console.WriteLine("Current IP: " + currentIp);
                return true;
            } else {
                return false;
                Console.WriteLine("No change...");
            }
        }

        public void Start() {
            Console.WriteLine("Current IP: " + currentIp);
            Console.WriteLine("Current Machine Name: " + Environment.MachineName);
            _timer.Start();
        }

        public void Stop() {
            _timer.Stop();
        }

        public string getCurrentIP() {
            return currentIp;
        }

    }
}
