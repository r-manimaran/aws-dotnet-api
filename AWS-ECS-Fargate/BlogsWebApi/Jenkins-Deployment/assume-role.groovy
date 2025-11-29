pipeline {
 agent { label 'dotnet'}

 environment {
    AWS_REGION = 'us-east-1'
    ROLE_ARN = ''
    ECR_URI = ''
 }
}