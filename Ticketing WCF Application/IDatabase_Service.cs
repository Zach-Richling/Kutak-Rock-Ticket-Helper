using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Ticketing_WCF_Application {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IDatabase_Service {
        [OperationContract]
        [WebInvoke(Method = "GET", 
            UriTemplate = "/GetMachineInfo/{input}", 
            RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json, 
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        MachineInfo getMachineinfo(String input);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/AddComputer/{ip}/{machineName}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        string addComputer(String ip, String machineName);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/AddMachineInfo/{machineName}?machineInfo={machineInfo}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        string addMachineinfo(string machineName, string machineInfo);
    }

    public class MachineInfo {
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

        public override string ToString()
        {
            string newline = Environment.NewLine;
            return ip + newline + OSName + newline + OSVersion + newline + machineName + newline + userName + newline + manufacturer + newline + model
                + BIOSNumber + newline + BIOSVersion + newline + macAddress + newline + ramAmount + newline;
        }
    }
}
