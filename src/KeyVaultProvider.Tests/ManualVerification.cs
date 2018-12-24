using System;
using System.Threading.Tasks;

namespace KeyVaultProvider.Tests
{
    using Microsoft.Azure.ServiceBus;
    using Xunit;

    public class ManualVerification
    {
        [Fact(Skip = "Manual verification. Requires environment variable for MSI and KeyVault setup") ]
        public async Task Should_retrieve_secret_using_application_id_and_key_vault_secret()
        {
            var environmentVariable = Environment.GetEnvironmentVariable("AzureServicesAuthConnectionString", EnvironmentVariableTarget.Machine);
            //"RunAs=App;AppId=aabbccdd-1234-5678-90ab-ffeeddccbbaa;TenantId=aabbccdd-1234-5678-90ab-ffeeddccbbaa;AppKey=oiafQ#jafi0a9fu#kaofjas43ifj@jasdf09jlakjsd="
            var elements = environmentVariable.Split(';');
            var clientId = elements[1].Replace("AppId=", string.Empty);
            var clientSecret = elements[3].Replace("AppKey=", string.Empty);

            var provider = new KeyVaultProvider(clientId: clientId, clientSecret: clientSecret,
                secretIdentifier: "https://keyvaultplugin.vault.azure.net/secrets/storage-connection-string/39fa2e560eaf4b8eab7bf0e1a33d2357");
            var connectionString = await provider.GetConnectionString();
            Assert.Equal("UseDevelopmentStorage=true", connectionString);
        }

        [Fact(Skip = "Manual verification. Requires environment variable for MSI and KeyVault setup") ]
        public async Task Should_retrieve_secret_using_MSI()
        {
            var provider = new KeyVaultProvider("https://keyvaultplugin.vault.azure.net/secrets/storage-connection-string/39fa2e560eaf4b8eab7bf0e1a33d2357");
            var connectionString = await provider.GetConnectionString();
            Assert.Equal("UseDevelopmentStorage=true", connectionString);
        }
    }
}
