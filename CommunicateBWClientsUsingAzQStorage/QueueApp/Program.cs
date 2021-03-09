using System; // Namespace for Console output
using System.Configuration; // Namespace for ConfigurationManager
using System.Threading.Tasks; // Namespace for Task
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage

namespace QueueApp
{
    class Program
    {
        // Connection string

        private const string ConnectionString = "DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName=lava1articles;AccountKey=nLzVLdz9zxWmwM0rVHde9uJU4iPrstF3Eidy4fVUcKIO2zIgNI+AnGJ8WjHhH/j+ZlUHYZ1XoWlSPag6c4XJIA==";

        static void Main(string[] args)
        {
            string value = Console.ReadLine();
            SendArticleAsync(value).Wait();
            Console.WriteLine($"Sent message...: {value}");

            Console.WriteLine($"Retrieving messages....");

            string message = ReceiveArticleAsync().Result;
            Console.WriteLine(message);
            Console.WriteLine(message);
        }

        static async Task SendArticleAsync(string message)
        {
            QueueClient queueClient = new QueueClient(ConnectionString, "newsqueue");

            await queueClient.CreateIfNotExistsAsync();

            if (queueClient.Exists())
            {
                Console.WriteLine("The queue of news articles was created");
                await queueClient.SendMessageAsync(message);
            }
        }

        static async Task<string> ReceiveArticleAsync()
        {
            QueueClient queueClient = new QueueClient(ConnectionString, "newsqueue");

            if (queueClient.Exists())
            {
                QueueMessage[] queueMessages = await queueClient.ReceiveMessagesAsync();

                //Process the message in less than 30 seconds
                Console.WriteLine($"Dequeued message: '{queueMessages[0].MessageText}'");
                QueueMessage queueMessage = queueMessages[0];

                //QueueMessage[] retrievedMessage = await queueClient.ReceiveMessagesAsync();
                //Console.WriteLine($"Retrieved message with content '{retrievedMessage[0].MessageText}'");

                await queueClient.DeleteMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt);

                return queueMessage.MessageText;
            }
            return "<Queue empty or not created.>";
        }
    }
}
