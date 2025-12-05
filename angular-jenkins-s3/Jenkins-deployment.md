#

- Create S3 bucket
- Enable Static Web Access
- Create a Bucket Policy

```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "Statement1",
            "Principal": "*",
            "Effect": "Allow",
            "Action": [
                "s3:GetObject"
            ],
            "Resource": "arn:aws:s3:::maran-angular-jenkins-app/*"
        }
    ]
}
```