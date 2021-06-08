using System;
using Aper_bot.Modules.DiscordSlash;
using Certes;
using FluffySpoon.AspNet.LetsEncrypt;
using FluffySpoon.AspNet.LetsEncrypt.Certes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();
            
            services.AddScoped<DiscordValidationFilter>();
            
        }
        
        public void Configure(IApplicationBuilder app)
        {
            if (_conf.GetValue<bool>("Hosting:UseLetsEncrypt"))
            {
                app.UseFluffySpoonLetsEncrypt();
            }
            
            if (_conf.GetValue<bool>("Hosting:UseHTTPS"))
            {
                app.UseHttpsRedirection();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}