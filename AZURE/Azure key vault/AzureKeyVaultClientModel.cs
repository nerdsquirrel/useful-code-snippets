using CSharpFunctionalExtensions;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureKeyVault.Model
{
    public class AzureKeyVaultClientModel
    {
        public string VaultAddress { get; }
        public string KeyVaultClientId { get; }
        public string KeyVaultClientKey { get; }
        private KeyVaultClient KeyVaultClient { get; set; }

        protected AzureKeyVaultClientModel(string vaultAddress, string vaultClientId, string vaultClientKey)
        {
            VaultAddress = vaultAddress;
            KeyVaultClientId = vaultClientId;
            KeyVaultClientKey = vaultClientKey;
        }

        public static Result<AzureKeyVaultClientModel> Create(string vaultAddress, string vaultClientId, string vaultClientKey)
        {
            if (string.IsNullOrWhiteSpace(vaultAddress) || string.IsNullOrWhiteSpace(vaultClientId) || string.IsNullOrWhiteSpace(vaultClientKey))
                return Result.Fail<AzureKeyVaultClientModel>($"keyVaultAddress/keyVaultClientId/keyVaultClientKey is null or empty");

            var configModel = new AzureKeyVaultClientModel(vaultAddress, vaultClientId, vaultClientKey);
            var clientCredential = new ClientCredential(configModel.KeyVaultClientId, configModel.KeyVaultClientKey);

            configModel.KeyVaultClient = new KeyVaultClient(async (authority, resource, scope) =>
             {
                 var authenticationContext = new AuthenticationContext(authority, null);
                 return (await authenticationContext.AcquireTokenAsync(resource, clientCredential)).AccessToken;
             });

            return Result.Ok(configModel);
        }

        public async Task<SecretBundle> GetSecretAsync(string secretName)
        {
            return await KeyVaultClient.GetSecretAsync(VaultAddress, secretName);
        }

        public async Task<SecretBundle> DeleteSecretAsync(string secretName)
        {
            return await KeyVaultClient.DeleteSecretAsync(VaultAddress, secretName);
        }

        public async Task<SecretBundle> SetSecretAsync(KeyValuePair<string, string> secretKeyValue)
        {
            return await KeyVaultClient.SetSecretAsync(VaultAddress, secretKeyValue.Key, secretKeyValue.Value);
        }
    }
}