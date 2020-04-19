using System;
using System.Net;
using System.Timers;
using System.Management.Automation;
using System.Net.NetworkInformation;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace KutakRock {
    public class Ticket_Helper {
        public class MachineInfo
        {
            public string id            = "0"; 
            public string OSName        = "";
            public string OSVersion     = "";
            public string userName      = "";//
            public string manufacturer  = "";//
            public string model         = "";//
            public string serial        = "";//
            public string BIOSVersion   = "";
            public string macAddress    = "";
            public string ramAmount     = "";
        }

        private readonly Timer _timer;
        private string currentIP;
        private string machineName;
        private MachineInfo MachInf;

        public Ticket_Helper(int repeat)
        {
            _timer = new Timer(repeat) { AutoReset = true };
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            MachInf = new MachineInfo();
        }

        public void Start()
        {
            _timer.Start();
            //query IP and MAchine name, update machine info if changed.
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private string GetCurrentIP()
        {
            return currentIP;
        }

        //Logic here for every time the timer loops
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //If the IP has changed
            if (EnsureIPCorrectness() == true)
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
                MachInf.OSName = Environment.OSVersion.Platform.ToString();
                MachInf.OSVersion = Environment.OSVersion.ToString();
                MachInf.userName = Environment.UserName.ToString();
                MachInf.macAddress = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                      where nic.OperationalStatus == OperationalStatus.Up
                                      select nic.GetPhysicalAddress().ToString()
                                        ).FirstOrDefault();
                GetInfo();
            }
            Console.WriteLine(JsonConvert.SerializeObject(MachInf));
            //Do logic here
        }

        private void GetInfo()
        {
            //Computer Model Information
            PowerShell ModelShell = PowerShell.Create().AddScript("Get-CimInstance -ClassName Win32_ComputerSystem");
            foreach (PSObject obj in ModelShell.Invoke())
            {
                MachInf.model = obj.Properties["Model"].Value.ToString();
                MachInf.userName = obj.Properties["PrimaryOwnerName"].Value.ToString();
                MachInf.manufacturer = obj.Properties["Manufacturer"].Value.ToString();
                MachInf.ramAmount= obj.Properties["TotalPhysicalMemory"].Value.ToString();
            }

            //Computer BIOS Information
            PowerShell BiosShell = PowerShell.Create().AddScript("Get-CimInstance -ClassName Win32_BIOS");
            foreach (PSObject obj in BiosShell.Invoke())
            {
                MachInf.model = obj.Properties["SerialNumber"].Value.ToString();
                MachInf.BIOSVersion = obj.Properties["BIOSVersion"].Value.ToString();
            }
        }

        private bool EnsureIPCorrectness()
        {
            string externalip = "";
            try
            {
                externalip = new WebClient().DownloadString("https://api.ipify.org");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (externalip != currentIP)
            {
                Console.WriteLine("IP Changed from {0} to {1}.", currentIP, externalip);
                currentIP = externalip;
                Console.WriteLine("Current IP: " + currentIP);
                return true;
            }
            else
            {
                Console.WriteLine("No change...");
                return false;
            }
        }

    }
}