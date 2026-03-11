using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace appAhnenforschungBackEnd.Configutation
{
    static class ReadSettings
    {

        public static IConfigurationRoot Configuration;

        public static string UrlTickenValidation()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif

            Configuration = builder.Build();
            return Configuration["TockenValidation:Url"];

        }

        public static string UrlPersonImageSmall()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif
            //var builder = new ConfigurationBuilder()
            //       .SetBasePath(Directory.GetCurrentDirectory())
            //       .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            return Configuration["UrlPath:UrlPersonImageSmall"];

        }

        public static string UrlRessources()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif
            //var builder = new ConfigurationBuilder()
            //       .SetBasePath(Directory.GetCurrentDirectory())
            //       .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            return Configuration["UrlPath:UrlRessources"];

        }


        public static string EMAIL_CLIENT()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif

            Configuration = builder.Build();
            return Configuration["Network:SMTP_CLIENT"];
        }

        public static string NETWORK_CREDENTIAL_USER()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif

            Configuration = builder.Build();
            return Configuration["Network:NETWORK_CREDENTIAL_USER"];
        }

        public static string NETWORK_CREDENTIAL_PW()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif
            Configuration = builder.Build();
            return Configuration["Network:NETWORK_CREDENTIAL_PW"];
        }

        public static string MAILFORM()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif

            Configuration = builder.Build();
            return Configuration["Network:MAILFORM"];
        }

        public static string EMAIL_MailingList()
        {
            //var builder = new ConfigurationBuilder()
            //       .SetBasePath(Directory.GetCurrentDirectory())
            //       .AddJsonFile("appsettings.json");

#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif

            Configuration = builder.Build();
            return Configuration["Network:MAILINGLIST"];
        }

        public static string DO_SEMDMAIL_ADMIN()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif

            Configuration = builder.Build();
            return Configuration["Network:DO_SEMDMAIL_ADMIN"];
        }

        public static string DEBUG_MODE()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif

            Configuration = builder.Build();
            return Configuration["Network:DEBUG_MODE"];
        }

        public static string EMAIL_ADMIN()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif

            Configuration = builder.Build();
            return Configuration["Network:EMAIL_ADMIN"];
        }

        public static string FTP_URL_ROOT()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif

            Configuration = builder.Build();
            return Configuration["Network:FTP_URL_ROOT"];
        }

        public static string FTP_USER()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif

            Configuration = builder.Build();
            return Configuration["Network:FTP_USER"];
        }

        public static string FTP_USER_PW()
        {
#if DEBUG
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json", optional: true);
#else
  var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: true);
#endif

            Configuration = builder.Build();
            return Configuration["Network:FTP_USER_PW"];
        }

    }
}
