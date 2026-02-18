# 1. Create the Security Groups


# create the ALB
module "alb" {
  source = "../../modules/alb"

  project            = var.project
  environment        = var.environment
  private_subnet_ids = var.private_subnet_ids
  common_tags        = var.common_tags
}

# Create IAM Role for ECS Task Execution

# Create the ECR Repository

# Create the ECS Cluster

# Create the ECS Task Definition

# Create the ECS Service

# Create CloudWatch Log Group for ECS Services

#create the S3 Bucket for storing application data
module "s3" {
  source = "../../modules/storage/s3"

  project     = var.project
  environment = var.environment
  common_tags = var.common_tags
}


