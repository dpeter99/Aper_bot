using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Aper_bot.Website
{
    public static class Helpers
    {
        public static string BodyToString(this HttpRequest request)
        {
            var returnValue = string.Empty;
            //request.EnableRewind();
            request.EnableBuffering();
            //ensure we read from the begining of the stream - in case a reader failed to read to end before us.
            request.Body.Position = 0;
            //use the leaveOpen parameter as true so further reading and processing of the request body can be done down the pipeline
            using (var stream = new StreamReader(request.Body, Encoding.UTF8, true, 1024, leaveOpen:true))
            {
                returnValue = stream.ReadToEndAsync().Result;
            }
            //reset position to ensure other readers have a clear view of the stream 
            request.Body.Position = 0;
            return returnValue;
        }
    }
}