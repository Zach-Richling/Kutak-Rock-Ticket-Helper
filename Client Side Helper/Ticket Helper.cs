using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Timers;
using System.Management;
using NetworkInterface;
using System.Management.Automation;
using System.Reflection.PortableExecutable;
using Microsoft.VisualBasic.Devices;

namespace KutakRock {
    public class Ticket_Helper {

        public class MachineInfo
        {
            public string id = "0"; //needs some sort of integration with DB @ Zach
            public string ip = "";  
            public string machineName = Environment.MachineName;
            public string OSName = System.Environment.OSVersion.Platform;
            public string OSVersion = System.Environment.OSVersion;
            public string userName = Environment.UserName;
            public string manufacturer = "";
            public string name = "";
            public string model = ""; // Not Done
            public string serial = "";
            public string BIOSNumber = ""; //Number vs version?
            public string BIOSVersion = "";
            public string macAddress = (
                                            from nic in NetworkInterface.GetAllNetworkInterfaces()
                                            where nic.OperationalStatus == OperationalStatus.Up
                                            select nic.GetPhysicalAddress().ToString()
                                        ).FirstOrDefault();
            public string ramAmount = Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory.ToString();

        }

        private readonly Timer _timer;
        private string currentIp = new WebClient().DownloadString("https://api.ipify.org");
        WebRequest webRequest = WebRequest.Create(new Uri("www.google.com"));
        
        //Used to gatehr information about the BIOS
        public void GetBiosInformation()
        {
            string Serial = "";
            string BiosVer = "";
            string ManuFac = "";
            string Name = "";

            try
            {
                ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT SerialNumber, BIOSVersion, Manufacturer, Name FROM Win32_BIOS");
                ManagementObjectCollection collection = MOS.Get();
                foreach (ManagementObject obj in collection)
                {
                    Serial = (string)obj["SerialNumber"];
                    BiosVer = (string)obj["BIOSVersion"];
                    ManuFac = (string)obj["Manufacturer"];
                    Name = (string)obj["Name"];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            MachineInfo.manufacturer = ManuFac;
            MachineInfo.BIOSVersion = BiosVer;
            MachineInfo.name = Name;
            MachineInfo.serial = Serial;
        }

     

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
