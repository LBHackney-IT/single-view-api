# INSTRUCTIONS:
# 1) ENSURE YOU POPULATE THE LOCALS
# 2) ENSURE YOU REPLACE ALL INPUT PARAMETERS, THAT CURRENTLY STATE 'ENTER VALUE', WITH VALID VALUES
# 3) YOUR CODE WOULD NOT COMPILE IF STEP NUMBER 2 IS NOT PERFORMED!
# 4) ENSURE YOU CREATE A BUCKET FOR YOUR STATE FILE AND YOU ADD THE NAME BELOW - MAINTAINING THE STATE OF THE INFRASTRUCTURE YOU CREATE IS ESSENTIAL - FOR APIS, THE BUCKETS ALREADY EXIST
# 5) THE VALUES OF THE COMMON COMPONENTS THAT YOU WILL NEED ARE PROVIDED IN THE COMMENTS
# 6) IF ADDITIONAL RESOURCES ARE REQUIRED BY YOUR API, ADD THEM TO THIS FILE
# 7) ENSURE THIS FILE IS PLACED WITHIN A 'terraform' FOLDER LOCATED AT THE ROOT PROJECT DIRECTORY

provider "aws" {
  region  = "eu-west-2"
  version = "~> 2.0"
}
data "aws_caller_identity" "current" {}
data "aws_region" "current" {}
locals {
  application_name = "single-view-api"
   parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
}

# Create ElastiCache Redis security group

resource "aws_security_group" "redis_sg" {
    vpc_id = "vpc-06bc937006240a256"

    ingress {
        cidr_blocks = ["0.0.0.0/0"]
        from_port   = 6379
        to_port     = 6379
        protocol    = "tcp"
    }

    egress {
        from_port       = 0
        to_port         = 0
        protocol        = "-1"
        cidr_blocks     = ["0.0.0.0/0"]
    }

}

# Create ElastiCache Redis subnet group

resource "aws_elasticache_subnet_group" "default" {
    name        = "subnet-group-single-view"
    description = "Private subnets for the ElastiCache instances: single view"
    subnet_ids  = ["subnet-05e41a7c805051449", "subnet-02e2bfa025835cc20", "subnet-04fe111167269c8c2", "subnet-0dc9e293451de5466" ]
}


# Create ElastiCache Redis cluster

resource "aws_elasticache_cluster" "redis" {
    cluster_id           = "single-view-staging"
    engine               = "redis"
    engine_version       = "3.2.10"
    node_type            = "cache.m4.large"
    num_cache_nodes      = 1
    parameter_group_name = "default.redis3.2"
    port                 = 6379
    subnet_group_name    = aws_elasticache_subnet_group.default.name
    security_group_ids   = [aws_security_group.redis_sg.id]
}

#data "aws_iam_role" "ec2_container_service_role" {
#  name = "ecsServiceRole"
#}
#
#data "aws_iam_role" "ecs_task_execution_role" {
#  name = "ecsTaskExecutionRole"
#}

terraform {
  backend "s3" {
    bucket  = "terraform-state-corporate-staging"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/single-view-api/state"
  }
}
#
#module "development" {
#  # Delete as appropriate:
#  source                      = "github.com/LBHackney-IT/aws-hackney-components-per-service-terraform.git//modules/environment/backend/fargate"
#  # source = "github.com/LBHackney-IT/aws-hackney-components-per-service-terraform.git//modules/environment/backend/ec2"
#  cluster_name                = "staging-apis"
#  ecr_name                    = ecr repository name # Replace with your repository name - pattern: "hackney/YOUR APP NAME"
#  environment_name            = "staging"
#  application_name            = local.application_name
#  security_group_name         = back end security group name # Replace with your security group name, WITHOUT SPECIFYING environment. Usually the SG has the name of your API
#  vpc_name                    = "vpc-staging-apis"
#  host_port                   = port # Replace with the port to use for your api / app
#  port                        = port # Replace with the port to use for your api / app
#  desired_number_of_ec2_nodes = number of nodes # Variable will only be used if EC2 is required. Do not remove it.
#  lb_prefix                   = "nlb-staging-apis"
#  ecs_execution_role          = data.aws_iam_role.ecs_task_execution_role.arn
#  lb_iam_role_arn             = data.aws_iam_role.ec2_container_service_role.arn
#  task_definition_environment_variables = {
#    ASPNETCORE_ENVIRONMENT = "staging"
#  }
#  task_definition_environment_variable_count = number # This number needs to reflect the number of environment variables provided
#  cost_code = your project's cost code
#  task_definition_secrets      = {}
#  task_definition_secret_count = number # This number needs to reflect the number of environment variables provided
#}
