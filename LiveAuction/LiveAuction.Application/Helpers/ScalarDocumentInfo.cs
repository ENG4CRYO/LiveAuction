using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Helpers
{
    public static class ScalarDocumentInfo
    {
        public static string GetScalarDocumentInfo()
        {
            string template = """
                # LiveAuction API Documentation
                Welcome to the official API documentation for the LiveAuction platform.

                ## Test Credentials
                Use the following credentials to bypass the email verification step during testing:
                * **Test Email:** `example@test.com`
                * **Static OTP:** `123456`

                ---

                ## Required Headers
                Please include the following headers in your requests where applicable:

                | Header Name | Value | Description | Required? |
                | :--- | :--- | :----- | :--- |
                | **Authorization** | `Bearer {token}` | JWT Token for secured endpoints. | Yes |
                | **X-Api-Version** | `1.0` | Target API version. | Yes |
                | **X-App-Version** | `1.0.0` | Current version of the mobile app. | Yes |
                | **Accept-Encoding**| `gzip, br` | Requests compressed payload (br: Smallest, gzip: Fastest). | Optional (Recommended)|

                ---

                ## App Version Control
                The API strictly monitors the mobile app version via the `X-App-Version` header:
                * **Supported Versions:** `>= 1.0.0`
                * **Banned/Deprecated Versions:** `0.3.1` and `0.3.0` forced to update

                ---
                ## Rate Limiting
                Rate Limiting per IP address/User:
                * **General Limit:**  100 requests per 10 seconds.
                * **Auth Limit:**  5 requests per 30 seconds.
                ---

                ## General API Behaviors
                
                ### 1. Standard Response Format
                All endpoints return a unified JSON wrapper (`ApiResponse<T>`):
                ```json
                {
                  "succeeded": true,
                  "message": "Operation completed successfully.",
                  "errors": [],
                  "data": { ... } 
                }
                ```

                """;

            return template;
        }
    }
}
