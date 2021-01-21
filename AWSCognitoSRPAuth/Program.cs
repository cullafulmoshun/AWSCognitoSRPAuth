using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace AWSCognitoSRPAuth
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var awsConfig = configuration.GetSection("AWSCognito");

            var regionId = awsConfig["RegionId"];
            var poolId = awsConfig["PoolId"];
            var clientId = awsConfig["AppClientId"];            
            var username = awsConfig["SvcUsername"];
            var password = awsConfig["SvcPassword"];

            var region = RegionEndpoint.GetBySystemName(regionId);
            var provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), region);
            CognitoUserPool userPool = new CognitoUserPool(poolId, clientId, provider);
            CognitoUser user = new CognitoUser(username, clientId, userPool, provider);
            InitiateSrpAuthRequest authRequest = new InitiateSrpAuthRequest()
            {
                Password = password
            };

            var authResponse = await user.StartWithSrpAuthAsync(authRequest).ConfigureAwait(false);
            var accessToken = authResponse.AuthenticationResult.AccessToken;
            var idToken = authResponse.AuthenticationResult.IdToken;
            var expires = authResponse.AuthenticationResult.ExpiresIn;

            Console.WriteLine($"ACCESS_TOKEN: {accessToken}");
            Console.WriteLine("");
            Console.WriteLine($"ID_TOKEN: {idToken}");
            Console.WriteLine("");
            Console.WriteLine($"ExpiresIn: {expires}");
            Console.ReadKey();
        }
    }
}
