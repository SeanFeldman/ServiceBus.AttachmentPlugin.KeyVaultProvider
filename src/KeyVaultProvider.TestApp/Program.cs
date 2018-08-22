using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using ServiceBus.AttachmentPlugin;

namespace KeyVaultProviderTestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString");
            var managementClient = new ManagementClient(connectionString);
            if (!await managementClient.QueueExistsAsync("attachments"))
            {
                await managementClient.CreateQueueAsync("attachments");
            }

            var queueClient = new QueueClient(connectionString, "attachments", ReceiveMode.ReceiveAndDelete);

            var configuration = new AzureStorageAttachmentConfiguration(new KeyVaultProvider("https://keyvaultplugin.vault.azure.net/secrets/storage-connection-string"));

            queueClient.RegisterAzureStorageAttachmentPlugin(configuration);
            await queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes("hello")));

            var messageReceiver = new MessageReceiver(connectionString, "attachments", ReceiveMode.ReceiveAndDelete);
            messageReceiver.RegisterAzureStorageAttachmentPlugin(configuration);

            var message = await messageReceiver.ReceiveAsync(TimeSpan.FromSeconds(3));

            Console.WriteLine($"Received message with body: {Encoding.UTF8.GetString(message.Body)}");

            Console.ReadLine();
        }
    }
}