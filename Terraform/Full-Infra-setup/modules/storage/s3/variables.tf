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

variable "common_tags" {
  type = map(string)
}