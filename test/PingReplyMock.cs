using System.Net.NetworkInformation;

namespace SetupToolTest
{
    internal class PingReplyMock : PingReply
    {
        private IPStatus timedOut;
        public PingReplyMock()
        {
            
        }

        public PingReplyMock(IPStatus timedOut)
            :this()
        {
            this.timedOut = timedOut;
        }
    }
}