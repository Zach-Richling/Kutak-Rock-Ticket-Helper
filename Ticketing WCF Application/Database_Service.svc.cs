using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Ticketing_WCF_Application {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Database_Service : IDatabase_Service
    {

        MachineInfo IDatabase_Service.getMachineinfo(string input)
        {
            MachineInfo output = new MachineInfo();
            return output;
        }

        bool IDatabase_Service.setMachineinfo(string input)
        {
            return true;
        }
    }
}
