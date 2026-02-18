resource "aws_security_group" "app_lb_sg" {
  name        = "${var.project}-${var.environment}-app-lb-sg"
  description = "Security group for ALB"
  vpc_id      = var.vpc_id

  tags = merge(var.common_tags, {
    Name = "${var.project}-${var.environment}-app-lb-sg"
  })
}