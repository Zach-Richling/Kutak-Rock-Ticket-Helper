using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.Windows.Forms;

namespace Ticketing_WCF_Application
{
    public class HTTPHeaderAuth : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            WebHeaderCollection headers = request.Headers;
            foreach (string headerName in headers.AllKeys)
            {
                if (headerName == "Authorization" && headers[headerName] == "aJt5o3jQPOnkNuycYiuArPBPpPjwEHaR")
                {
                    return true;
                }
            }
            return false;
        }
    }
}