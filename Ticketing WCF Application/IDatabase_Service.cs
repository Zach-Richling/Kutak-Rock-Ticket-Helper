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
            UriTemplate = "/SetMachineInfo/{input}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool setMachineinfo(String input);
    }

    public class MachineInfo {
        public string ip = "";
        public string OSName = "";
        public string OSVersion = "";
        public string machineName = "";
        public string userName = "";
        public string manufacturer = "";
        public string model = "";
        public string BIOSNumber = "";
        public string BIOSVersion = "";
        public string macAddress = "";
        public string ramAmount = "";
    }
}
