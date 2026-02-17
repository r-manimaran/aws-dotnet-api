# configure the AWS provider
provider "aws" {
  region = "us-east-1"
  profile = "default" # If not default, specify your AWS CLI profile name here
}
# create an S3 bucket
resource "aws_s3_bucket" "my_bucket" {
  bucket = "maran-unique-bucket-name-12345"

    tags = {
        Name        = "My S3 Bucket"
        Environment = "Dev"
    }
}
