# URL Shortner with Postgresql - Deploy in AWS Lambda

This project will helps to create a URL shortner and redirect to the long url when called with Get method.


### Install the NuGet packages
```bash
dotnet add package "Microsoft.EntityFrameworkCore.Tools" --version 8.0.6
dotnet add package "Npgsql.EntityFrameworkCore.PostgreSQL" --version 8.0.4
dotnet add package Amazon.Lambda.AspNetCoreServer.Hosting --version 1.7.0
dotnet add package Microsoft.Extensions.Configuration.UserSecrets --prerelease
dotnet user-secrets init
dotnet user-secrets set myappSecret "secret"


dotnet ef migrations add initialCreate
dotnet ef database update

dotnet tool install -g Amazon.Lambda.Tools
dotnet lambda deploy-function
For Enter Runtime type dotnet8
For Enter Function Name: UrlShortner
Select or Create IAM Role with AWSLambdaBasicExecutionRole
Enter Memory Size:  256
Enter Timeout: 5
Enter Handler: api_lambda_deployment
```



## References
### Adding gitigonore file
```bash
> dotnet new gitignore
```