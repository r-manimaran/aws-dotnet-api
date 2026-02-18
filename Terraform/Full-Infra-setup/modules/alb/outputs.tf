  # output the ALB ARN
output "alb_arn" {
  value = aws_lb.app_lb.arn
}