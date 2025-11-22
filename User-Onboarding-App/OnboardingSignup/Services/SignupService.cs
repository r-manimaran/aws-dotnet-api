using Amazon.StepFunctions;
using Amazon.StepFunctions.Model;
using System.Text.Json;

namespace OnboardingSignup.Services;

public class SignupService
{
    private readonly IAmazonStepFunctions _sf;
    public SignupService(IAmazonStepFunctions sf)
    {
        _sf = sf;
    }

    public async Task<string> StartSignupWorkflowAsync(string name, string email, string role)
    {
        var input = new
        {
            Name = name,
            Email = email,
            Role = role            
        };

        var startRequest = new StartExecutionRequest
        {
            StateMachineArn = Environment.GetEnvironmentVariable("SIGNUP_STATE_MACHINE_ARN") ?? throw new InvalidOperationException("State Machine ARN not configured."),
            Input = JsonSerializer.Serialize(input)
        };
        
        var response = await _sf.StartExecutionAsync(startRequest);

        return response.ExecutionArn;
    }
}
