using Amazon.SimpleEmail;
using AmzSimpleEmail;
using AmzSimpleEmail.Endpoints;
using AmzSimpleEmail.Templates;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

builder.Services.AddAWSService<IAmazonSimpleEmailService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.ConfigurationSectionName)); ;

var app = builder.Build();

// Initialize email templates on application startup
// Commented out as we created the separate endpoint for registering email templates

// var emailService = app.Services.GetRequiredService<IAmazonSimpleEmailService>();
// await EmailTemplates.InitializeEmailTemplates(emailService);

app.MapGet("/", () => "Hello World!");

app.MapSESEndpoints();

app.UseHttpsRedirection();

app.Run();
