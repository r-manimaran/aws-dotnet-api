using OnboardingSignup.Services;

namespace OnboardingSignup.Endpoints;

public static class UsersEndpoints
{

    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/users").WithTags("Users");
       // Signup endpoint
               users.MapPost("/signup", async (UserSignupRequest request, SignupService signupService) =>
        {
            var executionArn = await signupService.StartSignupWorkflowAsync(request.Name, request.Email, request.Role);
        })
        .WithName("UserSignup")
        .WithSummary("Registers a new user.")
        .WithDescription("Creates a new user account with the provided username and email.")
        .Produces(201)
        .ProducesValidationProblem();
    }
}

public record UserSignupRequest(string Name, string Email, string Role);