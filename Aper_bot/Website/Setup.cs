using System;
using System.IO;
using Certes;
using FluffySpoon.AspNet.LetsEncrypt;
using FluffySpoon.AspNet.LetsEncrypt.Certes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Aper_bot.Website
{
    public class Setup
    {
        //private IServiceCollection _serviceProvider;
        
        public Setup()
        {

        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            
            
            services.AddRouting();
            
            //the following line adds the automatic renewal service.
            services.AddFluffySpoonLetsEncrypt(new LetsEncryptOptions()
            {
                Email = "dpeter99@gmail.com", //LetsEncrypt will send you an e-mail here when the certificate is about to expire
                UseStaging = false, //switch to true for testing
                Domains = new[] { "staging.lab.aper-lab.com" },
                TimeUntilExpiryBeforeRenewal = TimeSpan.FromDays(30), //renew automatically 30 days before expiry
                TimeAfterIssueDateBeforeRenewal = TimeSpan.FromDays(7), //renew automatically 7 days after the last certificate was issued
                CertificateSigningRequest = new CsrInfo() //these are your certificate details
                {
                    CountryName = "Hungary",
                    Locality = "HU",
                    Organization = "Aper Lab",
                    OrganizationUnit = "Labs",
                    State = "CS"
                }
            });

            //the following line tells the library to persist the certificate to a file, so that if the server restarts, the certificate can be re-used without generating a new one.
            services.AddFluffySpoonLetsEncryptFileCertificatePersistence();

            //the following line tells the library to persist challenges in-memory. challenges are the "/.well-known" URL codes that LetsEncrypt will call.
            services.AddFluffySpoonLetsEncryptMemoryChallengePersistence();
            
            
            services.AddControllers();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();
            
            services.AddScoped<DiscordValidationFilter>();
            
        }
        
        public void Configure(IApplicationBuilder app)
        {
            app.UseFluffySpoonLetsEncrypt();
            
            app.UseHttpsRedirection();
            
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