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
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace KutakRock {
    public class Ticket_Helper {

        public class MachineInfo
        {
            public string id            = "0"; 
            public string ip            = "";  
            public string machineName   = "";
            public string OSName        = "";
            public string OSVersion     = "";
            public string userName      = "";
            public string manufacturer  = "";
            public string name          = "";
            public string model         = ""; 
            public string serial        = "";
            public string BIOSNumber    = ""; //Number vs version?
            public string BIOSVersion   = "";
            public string macAddress    = "";
            public string ramAmount     = "";

        }

        WebRequest machineName = WebRequest.Create("http://kutakrockwcf.azurewebsites.net/Database_Service.svc/GetMachineInfo/MACHINENAME");

        HttpWebRequest httpMachineName = (HttpWebRequest)machineName;                                       /////
        httpMachineName.PreAuthenticate = true;                                                             ///// Zach pls
        httpMachineName.Headers.Add("Authorization", "Basic" + "aJt5o3jQPOnkNuycYiuArPBPpPjwEHaR");         /////

        string test = Environment.MachineName;

        string MachConf = httpMachineName.ToString();

        if(MachConf=){ //If MACHINENAME endpoint returns 0, indicating the machine does not exist in the DB...

            MachineInfo MachInf     = new MachineInfo();

            MachInf id              = "0"; //needs some sort of integration with DB @ Zach
            MachInf ip              = "";  
            MachInf machineName     = Environment.MachineName;
            MachInf OSName          = Environment.OSVersion.Platform;
            MachInf OSVersion       = Environment.OSVersion;
            MachInf userName        = Environment.UserName;
            MachInf manufacturer    = "";
            MachInf name            = "";
            MachInf model           = ""; 
            MachInf serial          = "";
            MachInf BIOSNumber      = ""; //Number vs version?
            MachInf BIOSVersion     = "";
            MachInf macAddress      = (from nic in NetworkInterface.GetAllNetworkInterfaces() 
                                        where nic.OperationalStatus == OperationalStatus.Up 
                                        select nic.GetPhysicalAddress().ToString()
                                        ).FirstOrDefault();
            MachInf ramAmount       = Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory.ToString();
        
            public void getModel()
            {
                PowerShell ModelShell   = PowerShell.Create("get-process").AddCommand("$ModelVar | Get-CimInstance -ClassName Win32_ComputerSystem"); ////////////////////////////////////////$ModelVar?
                ModelShell.invoke();
                MachInf model           = ModelShell.ToString();
            }

            //Used to gatehr information about the BIOS
            public void GetBiosInformation()
            {
                string Serial   = "";
                string BiosVer  = "";
                string ManuFac  = "";
                string Name     = "";

                try
                {
                    ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT SerialNumber, BIOSVersion, Manufacturer, Name FROM Win32_BIOS");
                    ManagementObjectCollection collection = MOS.Get();
                    foreach (ManagementObject obj in collection)
                    {
                        Serial  = (string)obj["SerialNumber"];
                        BiosVer = (string)obj["BIOSVersion"];
                        ManuFac = (string)obj["Manufacturer"];
                        Name    = (string)obj["Name"];
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                MachInf manufacturer = ManuFac;
                MachInf BIOSVersion  = BiosVer;
                MachInf name         = Name;
                MachInf serial       = Serial;
            }

        }


        WebRequest machineName = WebRequest.Create("http://kutakrockwcf.azurewebsites.net/Database_Service.svc/AddComputer/EXTERNAL_IP/MACHINE_NAME"); //push to this endpoint once info has been gathered

        private readonly Timer _timer;
        private string currentIp    = new WebClient().DownloadString("https://api.ipify.org");
        WebRequest webRequest       = WebRequest.Create(new Uri("www.google.com"));
        
        

        public Ticket_Helper(int repeat) {
            _timer = new Timer(repeat) { AutoReset = true};
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        }

        public void OnTimedEvent(object source, ElapsedEventArgs e) {
            ensureIPCorrectness();
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

        private void pushIP() {

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

        public void pushMachineInfo() {
            string MockJson = JsonConvert.SerializeObject<MachineInfo>(MachInf);
            WebRequest machineName = WebRequest.Create("http://kutakrockwcf.azurewebsites.net/Database_Service.svc/AddMachineInfo/MACHINE_NAME?machineInfo=SERIALIZED_STRING_OF_MACHINE_INFO_OBJECT_IN_JSON_FORMAT"); //push the jsonMachInf to this endpoint
        }





    }
}
