resource "aws_s3_bucket" "s3" {

  bucket = "${var.project}-${var.environment}-bucket"

  tags = merge(var.common_tags, {
    Name = "${var.project}-${var.environment}-bucket"
  })
  
}