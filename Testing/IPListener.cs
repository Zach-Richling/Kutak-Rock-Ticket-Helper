using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Timers;

namespace Testing {
    public class IPListener {

        private readonly Timer _timer;
        private string currentIp = new WebClient().DownloadString("https://api.ipify.org");
        public IPListener(int repeat) {
            _timer = new Timer(repeat) { AutoReset = true};
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e) {
            executeInstruction(ingestInstruction());
            ensureIPCorrectness();
        }

        private void ensureIPCorrectness() {
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
            } else {
                Console.WriteLine("No change...");
            }
        }

        private string ingestInstruction() {
            return "";
        }

        private void executeInstruction(String instruction) {
            if (instruction.Equals("")) {
                Console.WriteLine("No instruction received.");
            } else {
                Console.WriteLine("Instruction Received");
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
