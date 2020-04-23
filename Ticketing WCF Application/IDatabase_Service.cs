using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Ticketing_WCF_Application
{
    [ServiceContract]
    public interface IDatabase_Service {
        [OperationContract]
        [WebInvoke(Method = "GET", 
            UriTemplate = "/GetMachineInfo/{machineName}", 
            RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json, 
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        MachineInfo getMachineinfo(String machineName);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/AddComputer/{ip}/{machineName}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        DatabaseOutput addComputer(String ip, String machineName);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/AddMachineInfo/{machineName}?machineInfo={machineInfo}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        string addMachineinfo(string machineName, string machineInfo);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/CreateTicket/{ticketName}/{ticketDesc}/{ticketSeverity}/{computerID}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        string createTicket(string ticketName, string ticketDesc, string ticketSeverity, string computerID);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/SendTicket/{ticketId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        string sendTicket(string ticketId);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/GetFAQ",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<FAQOutput> getFAQ();

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/CreateFAQ/{question}/{desc}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        int createFAQ(string question, string desc);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/CreateFAQAnswer/{questionId}/{answer}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        string createFAQAnswer(string questionId, string answer);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/GetComputer/{ip}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetComputer(string ip);
    }

    public class MachineInfo {
        public string id = "0";
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

    public class DatabaseOutput
    {
        public string id = "";
        public string info = "";
    }

    public class FAQOutput
    {
        public string question = "Test";
        public string description = "Test";
        public int id = 0;
        public int count = 0;
        public string[] answers;
    }
}
