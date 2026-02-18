resource "aws_lb" "app_lb" {
  name               = "${var.project}-${var.environment}-alb"
  internal           = true
  load_balancer_type = "application"
  security_groups    = [aws_security_group.app_lb_sg.id]
  subnets           = var.private_subnet_ids

  enable_deletion_protection = false

  tags = merge(var.common_tags, {
    Name = "${var.project}-${var.environment}-alb"
  })  
}