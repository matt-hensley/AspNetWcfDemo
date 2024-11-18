using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace AspNetWcfDemo
{
    [DataContract]
    public class StatusRequest
    {
        [DataMember]
        public string Status { get; set; }

        public StatusRequest(string status)
        {
            Status = status;
        }
    }

    [DataContract]
    public class StatusResponse
    {
        [DataMember]
        public DateTimeOffset ServerTime { get; set; }
    }

    [ServiceContract]
    // Add the following line to enable automatic tracing of WCF service calls
    // this replaces web.config behavior configuration
    //[OpenTelemetry.Instrumentation.Wcf.TelemetryContractBehavior]
    public interface ITestService
    {
        // WCF test client does not support async methods
        [OperationContract]
        StatusResponse Ping(StatusRequest request);
    }
}
