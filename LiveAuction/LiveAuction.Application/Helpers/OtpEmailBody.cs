using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Helpers
{
    public static class OtpEmailBody
    {
        public static string GenerateOtpEmailBody(string otp)
        {
            string template = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <style>
            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
            .container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); overflow: hidden; }}
            .header {{ background-color: #2c3e50; color: #ffffff; padding: 20px; text-align: center; }}
            .header h1 {{ margin: 0; font-size: 24px; }}
            .content {{ padding: 30px; color: #333333; line-height: 1.6; }}
            .otp-box {{ background-color: #e8f0fe; border: 1px dashed #2c3e50; color: #2c3e50; font-size: 32px; font-weight: bold; text-align: center; padding: 15px; margin: 20px 0; border-radius: 5px; letter-spacing: 5px; }}
            .footer {{ background-color: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #888888; border-top: 1px solid #eeeeee; }}
            .warning {{ color: #d9534f; font-size: 13px; margin-top: 20px; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h1>LiveAuction</h1>
            </div>
            <div class='content'>
                <p>Hello,</p>
                <p>Thank you for registering with <strong>LiveAuction</strong>. To complete your account setup, please use the following One-Time Password (OTP):</p>
                
                <div class='otp-box'>{otp}</div>
                
                <p>This code is valid for <strong>5 minutes</strong>. Do not share this code with anyone.</p>
                
                <p class='warning'>If you did not request this code, please ignore this email.</p>
            </div>
            <div class='footer'>
                <p>&copy; {DateTime.UtcNow.Year} LiveAuction Inc. All rights reserved.</p>
                <p>This is an automated message, please do not reply.</p>
            </div>
        </div>
    </body>
    </html>";

            return template;
        }
    }
}
