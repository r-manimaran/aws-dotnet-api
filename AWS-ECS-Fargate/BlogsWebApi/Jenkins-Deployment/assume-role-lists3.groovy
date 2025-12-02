pipeline {
     agent { label 'dotnet'}
    
    stages {
        stage('Assume Role and List Buckets') {
            steps {
                withCredentials([
                [$class:'AmazonWebServicesCredentialsBinding',
            credentialsId:'aws-creds']
            ])
            {
            sh '''
                # 1. Generate short-lived STS credentials
                CREDS=$(aws sts assume-role \
                --role-arn "arn:aws:iam::395109667422:role/assumeRole-demo-role" \
                --role-session-name "S3ListerSession" \
                --external-id "Maran" \
                --output text \
                --query "Credentials.[AccessKeyId,SecretAccessKey,SessionToken]"
                )

                CREDS_AKI=$(echo $CREDS | awk '{print $1}')
                CREDS_SAK=$(echo $CREDS | awk '{print $2}')
                CREDS_ST=$(echo $CREDS | awk '{print $3}')

                export AWS_ACCESS_KEY_ID=$CREDS_AKI
                export AWS_SECRET_ACCESS_KEY=$CREDS_SAK
                export AWS_SESSION_TOKEN=$CREDS_ST

                # 2. Use Temp credentials to list buckets
                aws s3 ls
                
            '''
            }
            }
        }
    }
}