using LetsEncrypt.Core.Cryptography;
using LetsEncrypt.Core.Entities;
using LetsEncrypt.Core.Jws;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsEncrypt.Core
{
    public partial class AcmeClient
    {
        public async Task<List<Challenge>> GetDnsChallenges(Order order)
        {
            var result = new List<Challenge>();
            foreach (var authorizationLocation in order.Authorizations)
            {
                var authorization = await GetAuthorizationAsync(Account.Location, authorizationLocation);
                var chalanges = authorization.Challenges.Where(i => i.Type == ChallengeType.Dns01);

                foreach (var chalange in chalanges)
                {
                    chalange.VerificationKey = GetChalangeKey(chalange.Token);
                    chalange.VerificationValue = GetChalangeDnsText(chalange.Token);
                }

                result.AddRange(chalanges);
            }

            return result;
        }

        public async Task<Challenge> GetChallengeAsync(Challenge challenge)
        {
            var signedData = _jws.Sign(null, Account.Location, challenge.Url, Nonce);
            return await PostAsync<Challenge>(challenge.Url, signedData);
        }

        public async Task ValidateChallengeAsync(Challenge challenge)
        {
            var signedData = _jws.Sign(new { }, Account.Location, challenge.Url, Nonce);
            challenge = await PostAsync<Challenge>(challenge.Url, signedData);
        }

        // Private Methods

        private async Task<Authorization> GetAuthorizationAsync(Uri accountLocation, Uri authorizationId)
        {
            var signedData = _jws.Sign(null, accountLocation, authorizationId, Nonce);
            return await PostAsync<Authorization>(authorizationId, signedData);
        }

        //

        //private async Task<Authorization> DeactivateChallengeAsync(Uri location)
        //{
        //    var auth = new Authorization { Status = AuthorizationStatus.Deactivated };
        //    var signedData = _jws.Sign(auth, location, location, Nonce);
        //    return await PostAsync<Authorization>(location, signedData);
        //}

        private string GetChalangeKey(string token)
        {
            var jwkJson = JsonConvert.SerializeObject(Key.Jwk, Formatting.None, _jsonSettings);
            var jwkBytes = Encoding.UTF8.GetBytes(jwkJson);
            var jwkThumbprint = Sha256HashProvider.ComputeHash(jwkBytes);
            var jwkThumbprintEncoded = JwsConvert.ToBase64String(jwkThumbprint);
            return $"{token}.{jwkThumbprintEncoded}";
        }

        private string GetChalangeDnsText(string token)
        {
            var key = GetChalangeKey(token);
            var hashed = Sha256HashProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
            return JwsConvert.ToBase64String(hashed);
        }
    }
}