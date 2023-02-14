using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Messaging;

namespace RequestReply
{
    class Program
    {
        static void Main(string[] args)
        {
            String Request = @".\Private$\BluffCityRequestQueueAIC";
            String ReplySAS = @".\Private$\BluffCityReplyQueueSAS";
            String ReplySW = @".\Private$\BluffCityReplyQueueSW";
            String ReplyKLM = @".\Private$\BluffCityReplyQueueKLM";
            String Invalid = @".\Private$\InvalidQueue";

            Requestor SAS = new Requestor(Request, ReplySAS, "SK249");
            Requestor SW = new Requestor(Request, ReplySW, "SW1423");
            Requestor KLM = new Requestor(Request, ReplyKLM, "KLM582");
            Replier AIC = new Replier(Request, Invalid);
            SAS.Send();
            SAS.ReceiveSync();
            KLM.Send();
            KLM.ReceiveSync();
            SW.Send();
            SW.ReceiveSync();
        }
    }
}
