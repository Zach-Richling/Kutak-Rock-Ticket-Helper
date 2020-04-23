using System;
using System.Net;
using System.Timers;
using System.Management.Automation;
using System.Net.NetworkInformation;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace KutakRock {
    public class Ticket_Helper {
        public class MachineInfo
        {
            public string id            = "0"; 
            public string OSName        = "";
            public string OSVersion     = "";
            public string userName      = "";
            public string manufacturer  = "";
            public string model         = "";
            public string serial        = "";
            public string BIOSVersion   = "";
            public string macAddress    = "";
            public string ramAmount     = "";
        }

        private readonly Timer _timer;
        private string currentIP;
        private string machineName;
        private MachineInfo MachInf;
        private string baseAddress = @"http://kutakrockwcf.azurewebsites.net/";

        public Ticket_Helper(int repeat)
        {
            _timer = new Timer(repeat) { AutoReset = true };
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            MachInf = new MachineInfo();
        }

        public void Start()
        {
            _timer.Start();

            //Get Machine Name
            machineName = Environment.MachineName;

            //Update IP if needed
            EnsureIPCorrectness();

            //Add computer to DB if it doesnt exist
            AddComputer();

            //Update machine info locally
            UpdateInfo();

            //Adds machine information to the DB
            AddMachineInfo();
        }

        public void AddComputer()
        {
            //Send request to WCF Service to create an entry for the computer if one doesnt exist
            WebRequest request = WebRequest.Create(baseAddress + @"Database_Service.svc/AddComputer/" + currentIP + @"/" + machineName);
            request.Headers.Add("Authorization", "aJt5o3jQPOnkNuycYiuArPBPpPjwEHaR");
            WebResponse response = request.GetResponse();

            //Read response from web request
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader responseReader = new StreamReader(responseStream);
                var JSONString = JObject.Parse(responseReader.ReadToEnd());
                var databaseOutput = JObject.Parse(JSONString["addComputerResult"].ToString());
                MachInf.id = databaseOutput["id"].ToString();
            }
        }

        public void AddMachineInfo()
        {
            //Send request to WCF to update machine info for this computer
            Console.WriteLine(baseAddress + @"Database_Service.svc/AddMachineInfo/" + machineName + "?machineInfo=" + JsonConvert.SerializeObject(MachInf));
            
            WebRequest addRequest = WebRequest.Create(baseAddress + @"Database_Service.svc/AddMachineInfo/"+ machineName +"?machineInfo=" + JsonConvert.SerializeObject(MachInf));
            addRequest.Headers.Add("Authorization", "aJt5o3jQPOnkNuycYiuArPBPpPjwEHaR");
            WebResponse addResponse = addRequest.GetResponse();

            //Read response from web request
            using (Stream responseStream = addResponse.GetResponseStream())
            {
                StreamReader responseReader = new StreamReader(responseStream);
                Console.WriteLine(responseReader.ReadToEnd());
            }
        }

        public void Stop()
        {
            _timer.Stop();
        }

        //Logic here for every time the timer loops
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (EnsureIPCorrectness() == true)
            {
                UpdateInfo();
                AddComputer();
                AddMachineInfo();
                Console.WriteLine(JsonConvert.SerializeObject(MachInf));
            }
        }

        private void UpdateInfo()
        {
            //Computer Model Information
            PowerShell ModelShell = PowerShell.Create().AddScript("Get-CimInstance -ClassName Win32_ComputerSystem");
            foreach (PSObject obj in ModelShell.Invoke())
            {
                MachInf.model = obj.Properties["Model"].Value.ToString();
                MachInf.userName = obj.Properties["PrimaryOwnerName"].Value.ToString();
                MachInf.manufacturer = obj.Properties["Manufacturer"].Value.ToString();
                long ram = Convert.ToInt64(obj.Properties["TotalPhysicalMemory"].Value) / 1000000000;
                MachInf.ramAmount = ram.ToString();
            }

            //Computer BIOS Information
            PowerShell BiosShell = PowerShell.Create().AddScript("Get-CimInstance -ClassName Win32_BIOS");
            foreach (PSObject obj in BiosShell.Invoke())
            {
                MachInf.serial = obj.Properties["SerialNumber"].Value.ToString();
                MachInf.BIOSVersion = obj.Properties["SMBIOSBIOSVersion"].Value.ToString();
            }

            //OS Name and Version
            PowerShell OSShell = PowerShell.Create().AddScript("Get-Ciminstance -ClassName Win32_OperatingSystem | SELECT Caption, Version");
            foreach (PSObject obj in OSShell.Invoke())
            {
                MachInf.OSName = obj.Properties["Caption"].Value.ToString();
                MachInf.OSVersion = obj.Properties["Version"].Value.ToString();
            }

            //Username and MacAddress
            MachInf.userName = Environment.UserName.ToString();
            MachInf.macAddress = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                  where nic.OperationalStatus == OperationalStatus.Up
                                  select nic.GetPhysicalAddress().ToString()
                                    ).FirstOrDefault();
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