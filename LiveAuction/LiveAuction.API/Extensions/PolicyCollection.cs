using Microsoft.AspNetCore.Builder;

namespace LiveAuction.API.Extensions
{
    public static class PolicyCollection
    {
        public static HeaderPolicyCollection policyCollection(WebApplication app)
        {
            var policy = new HeaderPolicyCollection()
                .AddDefaultSecurityHeaders()
                 .RemoveServerHeader()
                .AddPermissionsPolicy(builder =>
                {
                    builder.AddCamera().None();
                    builder.AddMicrophone().None();
                    builder.AddGeolocation().None();
                });

            if (app.Environment.IsProduction())
            {
                policy.AddStrictTransportSecurity(31536000, true, true)
                      .AddContentSecurityPolicy(csp =>
                      {
        
                          csp.AddDefaultSrc().Self();

                        
                          csp.AddImgSrc().Self().From("data:");
                          csp.AddFontSrc().Self().From("data:");

                       
                          csp.AddConnectSrc().Self();

     
                          csp.AddScriptSrc()
                             .Self()            
                             .UnsafeInline()   
                             .UnsafeEval();   

                          // 5. الستايلات
                          csp.AddStyleSrc()
                             .Self()
                             .UnsafeInline();    
                      });
            }


            return policy;

        }
    }
}
