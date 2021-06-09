using System;
using System.IO;
using System.Reflection;
using Aper_bot.Hosting.WebHost.Infrastructure;
using Aper_bot.Modules.DiscordSlash;
using Certes;
using FluffySpoon.AspNet.LetsEncrypt;
using FluffySpoon.AspNet.LetsEncrypt.Certes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Aper_bot.Hosting.WebHost
{
    public class Setup
    {
        private readonly IConfiguration _conf;
        //private IServiceCollection _serviceProvider;
        
        public Setup(IConfiguration conf)
        {
            _conf = conf;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            CertConfig conf = new CertConfig();
            _conf.Bind("Hosting:CertConfig", conf);

            services.AddRouting();


            if (_conf.GetValue<bool>("Hosting:UseLetsEncrypt"))
            {

                //the following line adds the automatic renewal service.
                services.AddFluffySpoonLetsEncrypt(new LetsEncryptOptions()
                {
                    Email = conf.EmailAddress, // "dpeter99@gmail.com", //LetsEncrypt will send you an e-mail here when the certificate is about to expire
                    UseStaging = false, //switch to true for testing
                    Domains = conf.DomainNames, // new[] { "staging.lab.aper-lab.com" },
                    TimeUntilExpiryBeforeRenewal = TimeSpan.FromDays(30), //renew automatically 30 days before expiry
                    TimeAfterIssueDateBeforeRenewal = TimeSpan.FromDays(7), //renew automatically 7 days after the last certificate was issued
                    CertificateSigningRequest = new CsrInfo() //these are your certificate details
                    {
                        CountryName = conf.Country,
                        Locality = conf.Country,
                        Organization = conf.Organization,
                        OrganizationUnit = conf.OrganizationUnit ?? "",
                        State = conf.State ?? ""
                    }
                });

                //the following line tells the library to persist the certificate to a file, so that if the server restarts, the certificate can be re-used without generating a new one.
                services.AddFluffySpoonLetsEncryptFileCertificatePersistence();

                //the following line tells the library to persist challenges in-memory. challenges are the "/.well-known" URL codes that LetsEncrypt will call.
                services.AddFluffySpoonLetsEncryptMemoryChallengePersistence();
            }

            services.AddControllers().AddNewtonsoftJson();

            services.AddApiVersioning(conf =>
            {
                conf.UseApiBehavior = false;
                conf.AssumeDefaultVersionWhenUnspecified = false;
                conf.DefaultApiVersion = new ApiVersion(1,0);
                
            });
            services.AddVersionedApiExplorer(options =>
            {
                options.SubstituteApiVersionInUrl = true;
                options.GroupNameFormat = "'v'VVV";
            });
            
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();

                // integrate xml comments
                options.IncludeXmlComments( XmlCommentsFilePath );
            } );
            //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            
            services.AddScoped<DiscordValidationFilter>();
            
        }
        
        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            if (_conf.GetValue<bool>("Hosting:UseLetsEncrypt"))
            {
                app.UseFluffySpoonLetsEncrypt();
            }
            
            if (_conf.GetValue<bool>("Hosting:UseHTTPS"))
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseApiVersioning();
            
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapHealthChecks("/health");
            });
            
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach ( var description in provider.ApiVersionDescriptions )
                    {
                        options.SwaggerEndpoint( $"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant() );
                    }
                } );


        }
        
        static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof( Setup ).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine( basePath, fileName );
            }
        }
    }
}