namespace api_lambda_deployment.Abstractions;

public interface IEndpoint{
    void MapEndpoint(IEndpointRouteBuilder app);
}