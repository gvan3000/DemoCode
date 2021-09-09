using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Threading.Tasks;

namespace OTAServices.Business.Functions.Helpers
{
    public class AzureServiceBusClient : IProvisioningServicesBusQueueClient, IOTAServicesBusTopicClient, IDeleteImsiCallbackResponseQueueClient
    {
        private string _connection;
        private string _queueOrTopicName;


        public AzureServiceBusClient(string connection, string queueOrTopicName)
        {
            _connection = connection;
            _queueOrTopicName = queueOrTopicName;
        }

        public async Task SendToQueue(string msg)
        {
            QueueClient client = new QueueClient(_connection, _queueOrTopicName);
            var message = new Message(Encoding.UTF8.GetBytes(msg));
            await client.SendAsync(message);
        }

        public async Task SendToNserviceBus(string msg, string messageType)
        {
            QueueClient client = new QueueClient(_connection, _queueOrTopicName);
            var message = new Message(Encoding.UTF8.GetBytes(msg));
            message.UserProperties.Add("NServiceBus.EnclosedMessageTypes", messageType);
            message.UserProperties.Add("NServiceBus.Transport.Encoding", "application/octect-stream");
            message.UserProperties.Add("NServiceBus.MessageIntent", "Send");
            await client.SendAsync(message);
        }

        public async Task SendToTopic(string msg)
        {
            TopicClient client = new TopicClient(_connection, _queueOrTopicName);
            var message = new Message(Encoding.UTF8.GetBytes(msg));
            await client.SendAsync(message);
        }
    }
}
