using LetsEncrypt.Client.Cryptography;
using LetsEncrypt.Client.Entities;
using LetsEncrypt.Client.Jws;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsEncrypt.Client
{
    public partial class AcmeClient
    {
        public async Task<List<Challenge>> GetDnsChallenges(Account account, Order order)
        {
            var result = new List<Challenge>();
            foreach (var authorizationLocation in order.Authorizations)
            {
                var authorization = await GetAuthorizationAsync(account, authorizationLocation);
                var chalanges = authorization.Challenges.Where(i => i.Type == ChallengeType.Dns01);

                foreach (var chalange in chalanges)
                {
                    chalange.DnsKey = "_acme-challenge." + authorization.Identifier.Value.Replace("*.", string.Empty);
                    chalange.VerificationKey = GetChalangeKey(account, chalange.Token);
                    chalange.VerificationValue = GetChalangeDnsText(account, chalange.Token);
                }

                result.AddRange(chalanges);
            }

            return result;
        }

        public async Task<Challenge> GetChallengeAsync(Account account, Challenge challenge)
        {
            var nonce = await GetNonceAsync();

            var signedData = account.Signer.Sign(null, account.Location, challenge.Url, nonce);
            return await PostAsync<Challenge>(challenge.Url, signedData);
        }

        public async Task ValidateChallengeAsync(Account account, Challenge challenge)
        {
            var nonce = await GetNonceAsync();

            var signedData = account.Signer.Sign(new { }, account.Location, challenge.Url, nonce);
            challenge = await PostAsync<Challenge>(challenge.Url, signedData);
        }

        // Private Methods

        private async Task<Authorization> GetAuthorizationAsync(Account account, Uri authorizationId)
        {
            var nonce = await GetNonceAsync();

            var signedData = account.Signer.Sign(null, account.Location, authorizationId, nonce);
            return await PostAsync<Authorization>(authorizationId, signedData);
        }

        //

        //private async Task<Authorization> DeactivateChallengeAsync(Uri location)
        //{
        //    var auth = new Authorization { Status = AuthorizationStatus.Deactivated };
        //    var signedData = _jws.Sign(auth, location, location, Nonce);
        //    return await PostAsync<Authorization>(location, signedData);
        //}

        private string GetChalangeKey(Account account, string token)
        {
            var jwkJson = JsonConvert.SerializeObject(account.Key.Jwk, Formatting.None, _jsonSettings);
            var jwkBytes = Encoding.UTF8.GetBytes(jwkJson);
            var jwkThumbprint = Sha256HashProvider.ComputeHash(jwkBytes);
            var jwkThumbprintEncoded = JwsConvert.ToBase64String(jwkThumbprint);
            return $"{token}.{jwkThumbprintEncoded}";
        }

        private string GetChalangeDnsText(Account account, string token)
        {
            var key = GetChalangeKey(account, token);
            var hashed = Sha256HashProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
            return JwsConvert.ToBase64String(hashed);
        }
    }
}