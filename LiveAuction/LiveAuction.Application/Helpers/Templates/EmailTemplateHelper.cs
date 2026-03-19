using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Helpers.Templates
{
    public static class EmailTemplateHelper
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
        public static string GenerateResetPasswordEmail(string otp)
        {
            string template = $$"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Reset Password</title>
                </head>
                <body style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; margin: 0; padding: 0;">
                    <div style="max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05);">
                        
                        <div style="background-color: #2c3e50; padding: 30px 20px; text-align: center;">
                            <h1 style="color: #ffffff; margin: 0; font-size: 28px; letter-spacing: 1px;">LiveAuction</h1>
                        </div>
                        
                        <div style="padding: 40px 30px; text-align: center;">
                            <h2 style="color: #333333; font-size: 24px; margin-bottom: 20px;">Password Reset Request</h2>
                            <p style="color: #666666; font-size: 16px; line-height: 1.6; margin-bottom: 30px;">
                                We received a request to reset the password for your LiveAuction account. 
                                Please use the following One-Time Password (OTP) to proceed. This code is valid for <strong>5 minutes</strong>.
                            </p>
                            
                            <div style="background-color: #f8f9fa; border: 2px dashed #3498db; border-radius: 8px; padding: 20px; margin: 0 auto; max-width: 250px;">
                                <span style="font-size: 32px; font-weight: bold; color: #2c3e50; letter-spacing: 5px;">{{otp}}</span>
                            </div>
                            
                            <p style="color: #999999; font-size: 14px; margin-top: 30px;">
                                If you did not request a password reset, please ignore this email or contact support if you have concerns.
                            </p>
                        </div>
                        
                        <div style="background-color: #f8f9fa; padding: 20px; text-align: center; border-top: 1px solid #eeeeee;">
                            <p style="color: #aaaaaa; font-size: 12px; margin: 0;">
                                &copy; {{DateTime.UtcNow.Year}} LiveAuction. All rights reserved.
                            </p>
                        </div>
                    </div>
                </body>
                </html>
                """;

            return template;
        }

        public static string GeneratePasswordChangedSuccessEmail(string userName = "Valued User")
        {
            string template = $$"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Password Changed Successfully</title>
                </head>
                <body style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; margin: 0; padding: 0;">
                    <div style="max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 10px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05);">
                        
                        <div style="background-color: #2c3e50; padding: 30px 20px; text-align: center;">
                            <h1 style="color: #ffffff; margin: 0; font-size: 28px; letter-spacing: 1px;">LiveAuction</h1>
                        </div>
                        
                        <div style="padding: 40px 30px; text-align: center;">
                            <div style="margin-bottom: 20px;">
                                <div style="display: inline-block; background-color: #e8f8f5; border-radius: 50%; padding: 15px; width: 50px; height: 50px; line-height: 50px;">
                                    <span style="color: #27ae60; font-size: 35px;">✓</span>
                                </div>
                            </div>

                            <h2 style="color: #333333; font-size: 24px; margin-bottom: 20px;">Password Changed</h2>
                            
                            <p style="color: #666666; font-size: 16px; line-height: 1.6; margin-bottom: 20px;">
                                Hello {{userName}},
                            </p>
                            <p style="color: #666666; font-size: 16px; line-height: 1.6; margin-bottom: 30px;">
                                This is a quick confirmation that the password for your LiveAuction account has been changed successfully.
                            </p>
                            
                            <div style="background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; text-align: left; margin: 0 auto; max-width: 450px;">
                                <p style="color: #856404; font-size: 14px; margin: 0;">
                                    <strong>Security Alert:</strong> If you did not make this change, please contact our support team immediately to secure your account.
                                </p>
                            </div>
                        </div>
                        
                        <div style="background-color: #f8f9fa; padding: 20px; text-align: center; border-top: 1px solid #eeeeee;">
                            <p style="color: #aaaaaa; font-size: 12px; margin: 0;">
                                &copy; {{DateTime.UtcNow.Year}} LiveAuction. All rights reserved.
                            </p>
                        </div>
                    </div>
                </body>
                </html>
                """;

            return template;
        }
    }
}
