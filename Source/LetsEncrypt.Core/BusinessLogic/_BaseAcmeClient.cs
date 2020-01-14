using LetsEncrypt.Core.Entities;
using LetsEncrypt.Core.Json;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LetsEncrypt.Core
{
    public class BaseAcmeClient
    {
        private const string RESPONCE_HEADER_KEY_NONCE = "Replay-Nonce";
        private const string MIME_TYPE_JOSE_JSON = "application/jose+json";

        private readonly static JsonSerializerSettings jsonSettings = JsonSettings.CreateSettings();
        private readonly static Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(() => new HttpClient());
        private HttpClient Http { get => _httpClient.Value; }

        private Directory _directory { get; set; }
        private string _nonce { get; set; }

        protected Directory Directory
        {
            get
            {
                if (_directory == null)
                {
                    throw new Exception("Client is not initialized correctly, please call method .InitAsync() first!");
                }
                return _directory;
            }
            set { _directory = value; }
        }

        protected string Nonce
        {
            get
            {
                if (string.IsNullOrEmpty(_nonce))
                {
                    throw new Exception("Client is not initialized correctly, please call method .InitAsync() first!");
                }
                return _nonce;
            }
            set { _nonce = value; }
        }

        // Ctor

        public BaseAcmeClient()
        {
        }

        // Public Methods

        public async Task InitAsync(Uri directoryUri)
        {
            if (_directory == null)
            {
                _directory = await GetAsync<Directory>(directoryUri);
            }

            if (_nonce == null)
            {
                _nonce = await GetNonceAsync();
            }
        }

        // Protected Methods

        protected async Task<T> GetAsync<T>(Uri uri) where T : BaseEntity, new()
        {
            using (var response = await Http.GetAsync(uri))
            {
                return await ProcessResponseAsync<T>(response);
            }
        }

        protected async Task<T> PostAsync<T>(Uri uri, object data) where T : BaseEntity, new()
        {
            var dataJson = JsonConvert.SerializeObject(data, Formatting.None, jsonSettings);
            var content = new StringContent(dataJson, Encoding.UTF8, MIME_TYPE_JOSE_JSON);

            content.Headers.ContentType.CharSet = null;
            using (var response = await Http.PostAsync(uri, content))
            {
                return await ProcessResponseAsync<T>(response);
            }
        }

        // Private Methods

        private async Task<string> GetNonceAsync()
        {
            var response = await Http.SendAsync(new HttpRequestMessage
            {
                RequestUri = Directory.NewNonce,
                Method = HttpMethod.Head,
            });

            if (!response.Headers.TryGetValues(RESPONCE_HEADER_KEY_NONCE, out var values))
            {
                throw new Exception("Retrieval of new nonce failed! Url: " + Directory.NewNonce);
            }

            return values.FirstOrDefault();
        }

        private async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response) where T : BaseEntity, new()
        {
            var entity = new T();
            var error = default(AcmeError);

            if (response.Headers.Contains(RESPONCE_HEADER_KEY_NONCE))
            {
                Nonce = response.Headers.GetValues(RESPONCE_HEADER_KEY_NONCE).FirstOrDefault();
            }

            var content = await response.Content.ReadAsStringAsync();

            if (IsJsonMediaType(response.Content?.Headers.ContentType?.MediaType))
            {
                if (response.IsSuccessStatusCode)
                {
                    entity = JsonConvert.DeserializeObject<T>(content);
                }
                else
                {
                    error = JsonConvert.DeserializeObject<AcmeError>(content);
                }
            }
            else
            {
                entity.UnknownContent = content;
            }

            if (entity == null)
            {
                entity = new T();
            }

            entity.Location = response.Headers.Location;
            entity.Error = error;

            if (entity.Error != null)
            {
                throw new Exception("Error: " + entity.Error.Detail);
            }

            return entity;
        }

        private static bool IsJsonMediaType(string mediaType)
        {
            if (mediaType != null && mediaType.StartsWith("application/"))
            {
                return mediaType
                    .Substring("application/".Length)
                    .Split('+')
                    .Any(t => t == "json");
            }

            return false;
        }
    }
}