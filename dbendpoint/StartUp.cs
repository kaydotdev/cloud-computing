using Cassandra;
using Cassandra.Mapping;
using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(GameStore.Games.StartUp.StartUp))]
namespace GameStore.Games.StartUp
{
    public class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var hostName = Environment.GetEnvironmentVariable("ENV_HOST_NAME");
            var hostUsername = Environment.GetEnvironmentVariable("ENV_HOST_USERNAME");
            var hostPassword = Environment.GetEnvironmentVariable("ENV_HOST_PASSWORD");
            var hostContactPoint = Environment.GetEnvironmentVariable("ENV_CONTACT_POINT");
            var hostPort = Convert.ToInt32(Environment.GetEnvironmentVariable("ENV_HOST_PORT") ?? "0");
            
            var options = new SSLOptions(SslProtocols.Tls12, true, ValidateServerCertificate);
            options.SetHostNameResolver((ipAddress) => hostName);
            Cluster cluster = Cluster.Builder()
                .WithCredentials(hostUsername, hostPassword).WithPort(hostPort)
                .AddContactPoint(hostContactPoint).WithSSL(options).Build();
            
            builder.Services.AddScoped<ISession>(s => cluster.Connect("gamestore"));
        }

        private static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return sslPolicyErrors == SslPolicyErrors.None;
        }
    }
}