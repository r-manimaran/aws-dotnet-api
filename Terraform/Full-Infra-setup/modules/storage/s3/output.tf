# output the bucket name
output "bucket_name" {
  value = aws_s3_bucket.bucket.id
}

# output the bucket ARN
output "bucket_arn" {
  value = aws_s3_bucket.bucket.arn
}