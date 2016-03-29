using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Summoner.Util;

namespace Summoner.Rest
{
    public abstract class SummonerRestClient
    {
        public SummonerRestClient(SummonerRestClientConfiguration config)
        {
            Assert.NotNull(config, "config");
            Assert.NotNull(config.Uri, "config.Uri");

            this.Config = config;
        }

        public SummonerRestClientConfiguration Config
        {
            get;
            private set;
        }

        public T Execute<T>(IRestRequest request) where T : new()
        {
            RestClient client = this.CreateClient(request);
            IRestResponse<T> response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(response.StatusDescription + ": " + response.Content);
            }

            return response.Data;
        }

        public void Execute(IRestRequest request)
        {
            RestClient client = this.CreateClient(request);
            IRestResponse response = client.Execute(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(response.StatusDescription + ": " + response.Content);
            }
        }

        private RestClient CreateClient(IRestRequest request)
        {
            RestClient client = new RestClient(Config.Uri);

            if (Config.UserAgent != null)
            {
                client.UserAgent = Config.UserAgent;
            }

            switch (Config.Authenticator)
            {
                case "network":
                    request.Credentials =
                        Config.IntegratedSecurity
                        ? CredentialCache.DefaultNetworkCredentials
                        : new NetworkCredential(Config.Username, Config.Password);
                    break;

                case "basic":
                default:
                    client.Authenticator = new HttpBasicAuthenticator(Config.Username, Config.Password);
                    break;
            }

            return client;
        }
    }
}
