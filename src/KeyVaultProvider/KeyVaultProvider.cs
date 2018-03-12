using Microsoft.Azure.Services.AppAuthentication;

namespace ServiceBus.AttachmentPlugin
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.KeyVault;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// KeyVault connection string provider.
    /// </summary>
    public class KeyVaultProvider : IProvideStorageConnectionString
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string secretIdentifier;

        /// <summary>
        /// KeyVault connection string provider using application/client ID, client secret, and KeyVault secret identifier.
        /// </summary>
        /// <param name="clientId">Application ID found in Azure Active Directory (App Registration).</param>
        /// <param name="clientSecret">KeyVault key secret value (available upon key creation only). </param>
        /// <param name="secretIdentifier">KeyVault secret identifier.
        /// <example>Possible formats:
        /// Latest value https://key-vault-name.vault.azure.net/secrets/secret-name
        /// Specific version https://key-vault-name.vault.azure.net/secrets/secret-name/version-id
        /// </example>
        /// </param>
        public KeyVaultProvider(string clientId, string clientSecret, string secretIdentifier)
        {
            Guard.AgainstEmpty(nameof(clientId), clientId);
            Guard.AgainstEmpty(nameof(clientSecret), clientSecret);
            Guard.AgainstEmpty(nameof(secretIdentifier), secretIdentifier);
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.secretIdentifier = secretIdentifier;
        }

        /// <summary>
        /// KeyVault connection string provider to work with Managed Service Idenity (MSI).
        /// <remarks>
        /// For MSI to work, AAD Application has to be granted Access Policy with Secret Get permission under the required KeyVault.
        /// To work with MSI in a local development environment or where MSI agent is not pre-installed, an environment variable "AzureServicesAuthConnectionString" is required.
        /// For more details, see https://azure.microsoft.com/en-us/resources/samples/app-service-msi-keyvault-dotnet/
        /// </remarks>
        /// </summary>
        /// <param name="secretIdentifier">KeyVault secret identifier.
        /// <example>Possible formats:
        /// Latest value https://key-vault-name.vault.azure.net/secrets/secret-name
        /// Specific version https://key-vault-name.vault.azure.net/secrets/secret-name/version-id
        /// </example>
        /// </param>
        public KeyVaultProvider(string secretIdentifier)
        {
            Guard.AgainstEmpty(nameof(secretIdentifier), secretIdentifier);

            this.secretIdentifier = secretIdentifier;
        }

        /// <summary>
        /// Retrieve Storage connection string using KeyVault.
        /// </summary>
        public async Task<string> GetConnectionString()
        {
            KeyVaultClient client;

            if (string.IsNullOrEmpty(clientId))
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultTokenCallback = azureServiceTokenProvider.KeyVaultTokenCallback;

                client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(keyVaultTokenCallback));
            }
            else
            {
                client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
            }

            // possible KeyVaultErrorException : https://docs.microsoft.com/en-us/azure/key-vault/key-vault-dotnet2api-release-notes#exceptions
            // message is always in exception.Body.Error.Message && .Code (for a string code)
            var secretBundle = await client.GetSecretAsync(secretIdentifier)
                .ConfigureAwait(false);

            return secretBundle.Value;
        }

        async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);

            var clientCredential = new ClientCredential(clientId, clientSecret);

            var authenticationResult = await authContext.AcquireTokenAsync(resource, clientCredential)
                .ConfigureAwait(false);

            if (authenticationResult == null)
            {
                throw new InvalidOperationException("Failed to retrieve access token for Key Vault");
            }

            return authenticationResult.AccessToken;
        }
    }
}
