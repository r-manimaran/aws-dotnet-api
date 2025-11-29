pipeline {
    agent { label 'dotnet'}

    stages {
        stage('Checkout') {
            steps {
                git branch: 'master',
                    url: 'https://github.com/r-manimaran/aws-dotnet-api.git'
            }
        }
        
        stage('Restore') {
            steps {
                sh 'cd AWS-ECS-Fargate/BlogsWebApi && dotnet restore'
            }
        }
        
        stage('Build') {
            steps {
                sh 'cd AWS-ECS-Fargate/BlogsWebApi && dotnet build --configuration Release'
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

        stage('Docker Push') {
            steps {
                withAWS(credentials:'aws-creds',region:'us-east-1'){
                sh '''
                # Login
                aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 395109667422.dkr.ecr.us-east-1.amazonaws.com
                
                 # Tag the local image to match ECR repo
                docker tag blogsapi:latest 395109667422.dkr.ecr.us-east-1.amazonaws.com/jenkins/dotnetdeploy:latest
                
                # Push to ECR
                docker push 395109667422.dkr.ecr.us-east-1.amazonaws.com/jenkins/dotnetdeploy:latest
                '''
                }
            }
        }
    }
}
