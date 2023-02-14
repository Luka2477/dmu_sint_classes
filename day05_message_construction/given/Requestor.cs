using System;
using System.Text;
using System.Messaging;

public class Requestor
{
    private MessageQueue requestQueue;
    private MessageQueue replyQueue;
    private String Airline;

    public Requestor(String requestQueueName, String replyQueueName, String Airlines)
    {
        requestQueue = new MessageQueue(requestQueueName);
        replyQueue = new MessageQueue(replyQueueName);
        this.Airline = Airlines;
        replyQueue.MessageReadPropertyFilter.SetAll();
        ((XmlMessageFormatter)replyQueue.Formatter).TargetTypeNames = new string[] { "System.String,mscorlib" };
    }

    public void Send()
    {
        Message requestMessage = new Message();

        requestMessage.Body = Airline;
        requestMessage.Label = Airline.Substring(0,2);

        requestMessage.ResponseQueue = replyQueue;
        requestQueue.Send(requestMessage);

        System.Diagnostics.Debug.WriteLine("Sent request");
        System.Diagnostics.Debug.WriteLine("\tTime:       {0}", DateTime.Now.ToString("HH:mm:ss.ffffff"));
        System.Diagnostics.Debug.WriteLine("\tMessage ID: {0}", requestMessage.Id);
        System.Diagnostics.Debug.WriteLine("\tCorrel. ID: {0}", requestMessage.CorrelationId);
        System.Diagnostics.Debug.WriteLine("\tReply to:   {0}", requestMessage.ResponseQueue.Path);
        System.Diagnostics.Debug.WriteLine("\tContents:   {0}", requestMessage.Body.ToString());
    }

    public void ReceiveSync()
    {
        Message replyMessage = replyQueue.Receive();

        System.Diagnostics.Debug.WriteLine("Received reply");
        System.Diagnostics.Debug.WriteLine("\tTime:       {0}", DateTime.Now.ToString("HH:mm:ss.ffffff"));
        System.Diagnostics.Debug.WriteLine("\tMessage ID: {0}", replyMessage.Id);
        System.Diagnostics.Debug.WriteLine("\tCorrel. ID: {0}", replyMessage.CorrelationId);
        System.Diagnostics.Debug.WriteLine("\tReply to:   {0}", "<n/a>");
        System.Diagnostics.Debug.WriteLine("\tContents:   {0}", replyMessage.Body.ToString());
    }
}
