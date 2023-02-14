using System;
using System.Messaging;

class Replier
{

    private MessageQueue invalidQueue;

    public Replier(String requestQueueName, String invalidQueueName)
    {
        MessageQueue requestQueue = new MessageQueue(requestQueueName);
        invalidQueue = new MessageQueue(invalidQueueName);

        requestQueue.MessageReadPropertyFilter.SetAll();
        ((XmlMessageFormatter)requestQueue.Formatter).TargetTypeNames = new string[] { "System.String,mscorlib" };

        try
        {
            requestQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnReceiveCompleted);
            requestQueue.BeginReceive();
        }
        catch (Exception)
        {
            System.Diagnostics.Debug.WriteLine("Noget er galt");
        }
    }

    public void OnReceiveCompleted(Object source, ReceiveCompletedEventArgs asyncResult)
    {
        MessageQueue requestQueue = (MessageQueue)source;
        Message requestMessage = requestQueue.EndReceive(asyncResult.AsyncResult);

        try
        {
            System.Diagnostics.Debug.WriteLine("Received request");
            System.Diagnostics.Debug.WriteLine("\tTime:       {0}", DateTime.Now.ToString("HH:mm:ss.ffffff"));
            System.Diagnostics.Debug.WriteLine("\tMessage ID: {0}", requestMessage.Id);
            System.Diagnostics.Debug.WriteLine("\tCorrel. ID: {0}", "<n/a>");
            System.Diagnostics.Debug.WriteLine("\tReply to:   {0}", requestMessage.ResponseQueue.Path);
            System.Diagnostics.Debug.WriteLine("\tContents:   {0}", requestMessage.Body.ToString());

            string contents = requestMessage.Body.ToString();
            MessageQueue replyQueue = requestMessage.ResponseQueue;
            Message replyMessage = new Message();
            string label = requestMessage.Label;
            switch (label)
            {
                case "SK":
                    contents = "13:45"; 
                    break;
                case "KL":
                    contents = "14:25"; 
                    break;
                case "SW":
                    contents = "15:40"; 
                    break;
            }
            replyMessage.Body = contents;
            replyMessage.CorrelationId = requestMessage.Id;
            replyQueue.Send(replyMessage);

            System.Diagnostics.Debug.WriteLine("Sent reply");
            System.Diagnostics.Debug.WriteLine("\tTime:       {0}", DateTime.Now.ToString("HH:mm:ss.ffffff"));
            System.Diagnostics.Debug.WriteLine("\tMessage ID: {0}", replyMessage.Id);
            System.Diagnostics.Debug.WriteLine("\tCorrel. ID: {0}", replyMessage.CorrelationId);
            System.Diagnostics.Debug.WriteLine("\tReply to:   {0}", "<n/a>");
            System.Diagnostics.Debug.WriteLine("\tContents:   {0}", replyMessage.Body.ToString());
        }
        catch (Exception)
        {
            System.Diagnostics.Debug.WriteLine("Invalid message detected");
            System.Diagnostics.Debug.WriteLine("\tType:       {0}", requestMessage.BodyType);
            System.Diagnostics.Debug.WriteLine("\tTime:       {0}", DateTime.Now.ToString("HH:mm:ss.ffffff"));
            System.Diagnostics.Debug.WriteLine("\tMessage ID: {0}", requestMessage.Id);
            System.Diagnostics.Debug.WriteLine("\tCorrel. ID: {0}", "<n/a>");
            System.Diagnostics.Debug.WriteLine("\tReply to:   {0}", "<n/a>");

            requestMessage.CorrelationId = requestMessage.Id;

            invalidQueue.Send(requestMessage);

            System.Diagnostics.Debug.WriteLine("Sent to invalid message queue");
            System.Diagnostics.Debug.WriteLine("\tType:       {0}", requestMessage.BodyType);
            System.Diagnostics.Debug.WriteLine("\tTime:       {0}", DateTime.Now.ToString("HH:mm:ss.ffffff"));
            System.Diagnostics.Debug.WriteLine("\tMessage ID: {0}", requestMessage.Id);
            System.Diagnostics.Debug.WriteLine("\tCorrel. ID: {0}", requestMessage.CorrelationId);
            System.Diagnostics.Debug.WriteLine("\tReply to:   {0}", requestMessage.ResponseQueue.Path);
        }

        requestQueue.BeginReceive();
    }
}

