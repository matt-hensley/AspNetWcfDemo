using System;

namespace AspNetWcfDemo
{
    public class TestService : ITestService
    {
        public StatusResponse Ping(StatusRequest request)
        {
            return new StatusResponse
            {
                ServerTime = DateTimeOffset.UtcNow,
            };
        }
    }
}
