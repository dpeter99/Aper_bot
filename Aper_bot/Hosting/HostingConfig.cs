using System.Collections.Generic;

namespace Aper_bot.Hosting
{
    public class HostingConfig
    {
        public CertConfig CertConfig;
        public int HttpPort;
        public int HttpsPort;
    }

    public class CertConfig
    {
        public bool AcceptTermsOfService { get; set; }
        public List<string> DomainNames { get; set; }
        
        public string EmailAddress { get; set; }
        
        public string Country { get; set; }
        public string Organization { get; set; }
        
        public string? OrganizationUnit { get; set; }
        public string? State { get; set; }
    }
}