pipeline {
 agent { label 'dotnet'}

 environment {
    AWS_REGION = 'us-east-1'
    ROLE_ARN = ''
    ECR_URI = ''
 }

 stages {
    stage('Checkout'){
        steps {
            git url:'https://github.com/r-manimaran/aws-dotnet-api.git', branch: 'master'
        }
    }

    stage ('Build .NET') {
        steps {
            sh '
              cd AWS-ECS-Fargate/BlogsWebApi && dotnet restore && dotnet build -c Release
              '
        }
    }

    stage('Docker Build') {
        steps {
            sh '''
            cd AWS-ECS-Fargate
            docker build -t blogsapi:latest .
            '''
            }
    }

    stage('Assume Role & Push'){
        steps {
            // ASSUME_ROLE_CREDENTIALS_ID is the Jenkins credentials ID (username=accessKey, password=secret) for the lightweight assume-only user
            withCredentials([usernamePassword(credentialsId: 'ASSUME_ROLE_CREDENTIALS_ID', usernameVariable:'ASSUME_AK', passwordVariable:'ASSUME_SK')]) {
                
                sh '''
                 # 1)Use lightweight credentials to assume the deployment role and capture temporary credentials
                 CREDS=$(aws sts assume-role \
                 --role-arn ${ROLE_ARN} \
                 --role-session-name jenkins-session-$(date +%s) \
                 --duration-seconds 3600 \
                 --query 'Credentials.[AccessKeyId, SecretAccessKey, SessionToken]' \
                 --output text \
                 --region ${AWS_REGION} \
                 --profile default || true)

                if [-z "$CREDS"]; then

                #export AWS Keys from the Jenkins-bound credentials if assume-role call needs the inline crdentials
                export AWS_ACCESS_KEY_ID=${ASSUME_AK}
                export AWS_SECRET_ACCESS_KEY=${ASSUME_SK}

                # retry assume-role now that env vars are available
                CREDS=$(aws sts assume-role \
                --role-arn ${ROLE_ARN} \
                --role-session-name jenkins-session-$(date +%s) \
                --duration-seconds 3600 \
                --query 'Credentials.[AccessKeyId,SecretAccessKey,SessionToken]' \
                --output text \
                --region ${AWS_REGION})

                fi

                read AWS_ACCESS_KEY_ID AWS_SECRET_ACCESS_KEY AWS_SESSION_TOKEN <<< "$CREDS"

                export AWS_ACCESS_KEY_ID
                export AWS_SECRET_ACCESS_KEY
                export AWS_SESSION_TOKEN
                export AWS_DEFAULT_REGION=${AWS_REGION}

                # 2) Login to ECR using temporary creds
                aws ecr get-login-password --region ${AWS_REGION} \
                  | docker login --username AWS --password-stdin ${ECR_URI}

                # 3) Tag & push
                docker tag  blogsapi:latest ${ECR_URI}:latest
                docker push ${ECR_URI}:latest

                '''
            }
        }       
    }
 }
}