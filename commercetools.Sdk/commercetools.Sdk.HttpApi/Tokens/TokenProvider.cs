﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using commercetools.Sdk.HttpApi.Domain;
using commercetools.Sdk.Serialization;

namespace commercetools.Sdk.HttpApi.Tokens
{
    internal abstract class TokenProvider
    {
        private readonly ISerializerService serializerService;
        private readonly ITokenStoreManager tokenStoreManager;

        protected TokenProvider(IHttpClientFactory httpClientFactory, ITokenStoreManager tokenStoreManager, ISerializerService serializerService)
        {
            this.HttpClientFactory = httpClientFactory;
            this.tokenStoreManager = tokenStoreManager;
            this.serializerService = serializerService;
        }

        public Token Token
        {
            get
            {
                var token = this.tokenStoreManager.Token;
                if (token == null)
                {
                    lock (this.tokenStoreManager)
                    {
                        token = this.tokenStoreManager.Token;
                        if (token != null)
                        {
                            return token;
                        }

                        token = this.GetTokenAsync(this.GetRequestMessage()).Result;
                        this.tokenStoreManager.Token = token;
                    }

                    return token;
                }

                if (!token.Expired)
                {
                    return token;
                }

                lock (this.tokenStoreManager)
                {
                    token = this.tokenStoreManager.Token;
                    if (!token.Expired)
                    {
                        return token;
                    }

                    var requestMessage = string.IsNullOrEmpty(token.RefreshToken)
                        ? this.GetRequestMessage()
                        : this.GetRefreshTokenRequestMessage();

                    token = this.GetTokenAsync(requestMessage).Result;
                    this.tokenStoreManager.Token = token;
                    return token;
                }
            }
        }

        public IClientConfiguration ClientConfiguration { get; set; }

        protected IHttpClientFactory HttpClientFactory { get; }

        public abstract HttpRequestMessage GetRequestMessage();

        private HttpRequestMessage GetRefreshTokenRequestMessage()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            string requestUri = this.ClientConfiguration.AuthorizationBaseAddress + "oauth/token?grant_type=refresh_token";
            requestUri += $"&refresh_token={this.tokenStoreManager.Token.RefreshToken}";
            request.RequestUri = new Uri(requestUri);
            string credentials = $"{this.ClientConfiguration.ClientId}:{this.ClientConfiguration.ClientSecret}";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(credentials)));
            request.Method = HttpMethod.Post;
            return request;
        }

        private async Task<Token> GetTokenAsync(HttpRequestMessage requestMessage)
        {
            HttpClient client = this.HttpClientFactory.CreateClient("auth");
            var result = await client.SendAsync(requestMessage).ConfigureAwait(false);
            string content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                return this.serializerService.Deserialize<Token>(content);
            }

            HttpApiClientException generalClientException =
                new HttpApiClientException(result.ReasonPhrase, (int)result.StatusCode);
            throw generalClientException;
        }
    }
}
