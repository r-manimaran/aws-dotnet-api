# Amazon Simple Email Service (SES) .NET API

A .NET 9 Web API application that demonstrates email functionality using Amazon Simple Email Service (SES). This application provides endpoints for sending emails, managing email templates, and handling different types of email communications.

## Features

- Send simple HTML emails
- Template-based email system
- Welcome email templates
- Newsletter email templates
- AWS SES integration with .NET Core

## Prerequisites

- .NET 9.0 SDK
- AWS Account with SES configured
- Verified email address in Amazon SES
- AWS credentials configured (AWS CLI, IAM roles, or environment variables)

## Project Structure

```
AmzSimpleEmail/
├── Endpoints/
│   └── AmazonSESEndpoints.cs    # API endpoints for email operations
├── Templates/
│   └── EmailTemplates.cs       # Email template management
├── EmailSettings.cs            # Configuration settings
├── Program.cs                  # Application entry point
└── appsettings.json           # Configuration file
```

## Setup

### 1. Clone and Build

```bash
git clone <repository-url>
cd AmazonSimpleEmailServiceApp/AmzSimpleEmail
dotnet restore
dotnet build
```

### 2. Configure AWS Settings

Update `appsettings.json`:

```json
{
  "AWS": {
    "Region": "us-east-1"
  },
  "EmailSettings": {
    "FromAddress": "your-verified-email@domain.com"
  }
}
```

### 3. AWS Credentials

Ensure AWS credentials are configured via:
- AWS CLI: `aws configure`
- Environment variables: `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`
- IAM roles (for EC2/Lambda deployment)

### 4. SES Setup

- Verify your sender email address in AWS SES Console
- If in sandbox mode, verify recipient email addresses
- Request production access for unrestricted sending

## Running the Application

```bash
dotnet run
```

The application will start on `https://localhost:5001` (or configured port).

## API Endpoints

### 1. Send Simple Email
```http
POST /email?email=recipient@example.com
```

Sends a basic HTML welcome email to the specified recipient.

### 2. Initialize Email Templates
```http
POST /email/initialize-templates
```

Creates and registers email templates in AWS SES. Run this once before using templated emails.

### 3. Send Welcome Email
```http
POST /email/welcome?email=recipient@example.com
```

Sends a personalized welcome email using a predefined template.

### 4. Send Newsletter Email
```http
POST /email/newsletter?email=recipient@example.com
```

Sends a newsletter email with dynamic content including articles and personalized information.

## Email Templates

The application includes two predefined templates:

### Welcome Template
- **Name**: `WelcomeTemplate`
- **Purpose**: User onboarding and welcome messages
- **Variables**: `{{user_name}}`

### Newsletter Template
- **Name**: `NewsletterTemplate`
- **Purpose**: Regular newsletter communications
- **Variables**: `{{user_name}}`, `{{newsletter_name}}`, `{{newsletter_content}}`, `{{articles}}`

## Dependencies

- **AWSSDK.SimpleEmail** (4.0.0.10) - AWS SES SDK
- **AWSSDK.Extensions.NETCore.Setup** (4.0.2) - AWS integration for .NET Core

## Configuration

### EmailSettings Class
```csharp
public class EmailSettings
{
    public const string ConfigurationSectionName = nameof(EmailSettings);
    public string FromAddress { get; set; }
}
```

### AWS Configuration
The application uses AWS SDK's default credential chain and configuration from `appsettings.json`.

## Error Handling

All endpoints include comprehensive error handling:
- HTTP 200: Successful email sending
- HTTP 500: Email sending failures or AWS service errors
- Detailed error messages in response body

## Development Notes

- Templates are recreated on each initialization (existing templates are deleted first)
- HTML and text versions are supported for all emails
- JSON serialization is used for template data
- HTTPS redirection is enabled by default

## Deployment Considerations

1. **Production SES Access**: Request production access to remove sending limits
2. **IAM Permissions**: Ensure proper SES permissions for the application
3. **Environment Variables**: Use secure credential management in production
4. **Monitoring**: Implement logging and monitoring for email delivery

## Troubleshooting

### Common Issues

1. **Email not received**: Check SES sandbox restrictions and verify recipient addresses
2. **Authentication errors**: Verify AWS credentials and IAM permissions
3. **Template errors**: Ensure templates are initialized before sending templated emails

### Required SES Permissions

```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": [
                "ses:SendEmail",
                "ses:SendTemplatedEmail",
                "ses:CreateTemplate",
                "ses:DeleteTemplate"
            ],
            "Resource": "*"
        }
    ]
}
```

## License

This project is for educational and demonstration purposes.