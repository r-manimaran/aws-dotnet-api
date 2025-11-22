
## Create IAM Role 
```bash
# Create the role
aws iam create-role \
  --role-name StepFunctionsExecutionRole \
  --assume-role-policy-document file://trust-policy.json

# Attach policy
aws iam attach-role-policy \
  --role-name StepFunctionsExecutionRole \
  --policy-arn arn:aws:iam::aws:policy/service-role/AWSStepFunctionsFullAccess
```