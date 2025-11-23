

## Deploy .Net Lambda using CLI

1️⃣ Build & Package Lambda
```bash
cd lambdas/MyLambdaFunction
dotnet publish -c Release -o publish
cd publish
zip -r lambda.zip .
```

2️⃣ Update existing Lambda code
```bash
aws lambda update-function-code \
    --function-name MyLambdaFunction \
    --zip-file fileb://lambda.zip
```

3️⃣ Create new Lambda (first-time deploy)
```bash
aws lambda create-function \
  --function-name MyLambdaFunction \
  --runtime dotnet8 \
  --role arn:aws:iam::<account-id>:role/LambdaExecutionRole \
  --handler MyLambda::MyLambda.Function::FunctionHandler \
  --zip-file fileb://lambda.zip
```

## Deploying Step Functions with AWS CLI
1️⃣ Update an existing State Machine
```bash
aws stepfunctions update-state-machine \
  --state-machine-arn arn:aws:states:<region>:<account>:stateMachine:UserOnboardingMachine \
  --definition file://user-onboarding.asl.json

```

2️⃣ Create a new Step Function
```bash
aws stepfunctions create-state-machine \
  --name UserOnboardingMachine \
  --definition file://user-onboarding.asl.json \
  --role-arn arn:aws:iam::<account>:role/StepFunctionsRole
```