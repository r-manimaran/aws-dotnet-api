pipeline {
    agent { label 'dotnet'}
     environment {
        SONAR_HOST_URL = "http://host.docker.internal:9000"
    }
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
        
        stage('Sonar Begin'){
            steps {
                withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]){
                    sh '''
                    # install dotnet sonarscanner (idempotent)
                    cd AWS-ECS-Fargate
                    export PATH="$Home/.dotnet/tools:$PATH"
                    dotnet tool install --global dotnet-sonarscanner || true
                    export PATH="$PATH:/root/.dotnet/tools"

                    #begin analysis
                    dotnet-sonarscanner begin \
                        /k:"dotnetapp" \
                        /d:sonar.host.url=http://host.docker.internal:9000 \
                        /d:sonar.login=${SONAR_TOKEN} \
                        /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"
                    '''
                }
            }
        }
        
        stage('Build (with Sonar)'){
            steps {
                sh '''
                cd AWS-ECS-Fargate
                dotnet build BlogsWebApi/BlogsWebApi.csproj --configuration Release
                '''
            }
        }
        
        stage('Sonar End') {
            steps {
                withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]) {
      sh '''
        export PATH="$PATH:/root/.dotnet/tools"
        cd AWS-ECS-Fargate
        dotnet-sonarscanner end /d:sonar.login=$SONAR_TOKEN
      '''
    }
            }
        }
    }
}