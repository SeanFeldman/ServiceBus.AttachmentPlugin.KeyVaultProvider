namespace ServiceBus.AttachmentPlugin
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.KeyVault;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// 
    /// </summary>
    public class KeyVaultProvider : IProvideStorageConnectionString
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string secretIdentifier;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="secretIdentifier"></param>
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
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetConnectionString()
        {
            var client = new KeyVaultClient(GetToken);

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

    // TODO: remove when plugin has this contract
    internal interface IProvideStorageConnectionString
    {
        Task<string> GetConnectionString();
    }
}
