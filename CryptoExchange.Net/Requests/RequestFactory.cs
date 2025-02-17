﻿using System;
using System.Net;
using System.Net.Http;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;

namespace CryptoExchange.Net.Requests
{
    /// <summary>
    /// Request factory
    /// </summary>
    public class RequestFactory : IRequestFactory,IDisposable
    {
        private HttpClient? httpClient;        

        /// <inheritdoc />
        public void Configure(TimeSpan requestTimeout, ApiProxy? proxy, HttpClient? client = null)
        {
            if (client == null)
            {
                HttpMessageHandler handler = new HttpClientHandler()
                {
                    Proxy = proxy == null ? null : new WebProxy
                    {
                        Address = new Uri($"{proxy.Host}:{proxy.Port}"),
                        Credentials = proxy.Password == null ? null : new NetworkCredential(proxy.Login, proxy.Password)
                    }
                };

                httpClient = new HttpClient(handler) { Timeout = requestTimeout };
            }
            else
            {
                httpClient = client;                
            }
        }

        /// <inheritdoc />
        public IRequest Create(HttpMethod method, Uri uri, int requestId)
        {
            if (httpClient == null)
                throw new InvalidOperationException("Cant create request before configuring http client");

            return new Request(new HttpRequestMessage(method, uri), httpClient, requestId);
        }

        public void Dispose()
        {
            if(httpClient!=null)
            {
                httpClient.Dispose();
            }
        }
    }
}
