﻿using System;
using System.Text;
using Aper_bot.Hosting.WebHost;
using Aper_bot.Modules.Discord.Config;
using Aper_bot.Util.Crypt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sodium;

namespace Aper_bot.Modules.DiscordSlash
{
    public class DiscordValidationFilter: ActionFilterAttribute //IAsyncActionFilter 
    {
        private readonly IOptions<DiscordConfig> _settings;
        private readonly ILogger _logger;

        public DiscordValidationFilter(IOptions<DiscordConfig> options, ILogger<DiscordValidationFilter> logger)
        {
            _settings = options;
            _logger = logger;
            //Order = 1;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_settings.Value.PublicKey == null)
            {
                return;
            }
            
            // Do something before the action executes.
            var request = context.HttpContext.Request;
            try
            {
                // Get the verification headers from the request ...
                var signature = request.Headers["X-Signature-Ed25519"].ToString();
                var timestamp = request.Headers["X-Signature-Timestamp"].ToString();
                // ... convert the signature and public key to byte[] to use in verification ...
                var byteSig = HexConverter.HexStringToByteArray(signature);
                // NOTE: This reads your Public Key that you need to store somewhere.
                var byteKey = HexConverter.HexStringToByteArray(_settings.Value.PublicKey);
                // ... read the body from the request ...

                if (byteSig.Length < 1 || byteKey.Length < 1)
                {
                    _logger.LogWarning("Failed to validate POST request for Discord API.");
                    context.Result = new UnauthorizedResult();
                    return;
                }
                

                var raw = request.BodyToString();
                /*
                using var reader = new StreamReader(request.Body);
                if (reader.BaseStream.CanSeek)
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                string raw = reader.ReadToEndAsync().Result;
                
                reader.Dispose();
                */
                
                // ... add the timestamp and convert it to a byte[] ...
                string body = timestamp + raw;
                var byteBody = Encoding.Default.GetBytes(body);
                // ... run through a verification with all the byte[]s ...
                bool validated = PublicKeyAuth.VerifyDetached(byteSig, byteBody, byteKey);
                // ... if it is not validated ...
                if(!validated)
                {   // ... log a warning and return a 401 Unauthorized.
                    _logger.LogWarning("Failed to validate POST request for Discord API.");
                    context.Result = new UnauthorizedResult();// Unauthorized("Invalid Request Signature");
                    return;
                }
                else
                { // ... otherwise continue onwards.
                    _logger.LogInformation("Received POST from Discord");
                }
            }
            catch (Exception ex)
            { // ... if an error occurred, log the error and return at 401 Unauthorized.
                _logger.LogInformation(ex, "Decryption failed.");
                _logger.LogWarning("Failed to validate POST request for Discord API.");
                context.Result = new UnauthorizedResult();// Unauthorized("Invalid Request Signature");
                return;
            }
            finally
            {
                //context.HttpContext.Request = request; 
            }

            //request.Body.Position = 0;
        }
        



    }
}