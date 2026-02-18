variable "project" {
    description = "The name of the project"
    type        = string
    default     = "aws-dotnet-api"
}

variable "environment" {
    description = "The environment (e.g., dev, staging, prod)"
    type        = string
    default     = "dev"
}

variable "vpc_id" {
    description = "The ID of the VPC"
    type        = string
    default     = ""
}

variable "private_subnet_ids" {
    description = "List of private subnet IDs for the ALB"
    type        = list(string)
    default     = []
}

# variables.tf
variable "common_tags" {
  type = map(string)
  default = {
    Environment = "dev"
    ManagedBy   = "Terraform"
    Project     = "aws-dotnet-api"
  }
}
