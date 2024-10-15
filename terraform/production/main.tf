# INSTRUCTIONS:
# 1) ENSURE YOU POPULATE THE LOCALS
# 2) ENSURE YOU REPLACE ALL INPUT PARAMETERS, THAT CURRENTLY STATE 'ENTER VALUE', WITH VALID VALUES
# 3) YOUR CODE WOULD NOT COMPILE IF STEP NUMBER 2 IS NOT PERFORMED!
# 4) ENSURE YOU CREATE A BUCKET FOR YOUR STATE FILE AND YOU ADD THE NAME BELOW - MAINTAINING THE STATE OF THE INFRASTRUCTURE YOU CREATE IS ESSENTIAL - FOR APIS, THE BUCKETS ALREADY EXIST
# 5) THE VALUES OF THE COMMON COMPONENTS THAT YOU WILL NEED ARE PROVIDED IN THE COMMENTS
# 6) IF ADDITIONAL RESOURCES ARE REQUIRED BY YOUR API, ADD THEM TO THIS FILE
# 7) ENSURE THIS FILE IS PLACED WITHIN A 'terraform' FOLDER LOCATED AT THE ROOT PROJECT DIRECTORY

terraform {
  required_providers {
        aws = {
            source = "hashicorp/aws"
            version = "~> 2.0"
        }
    }
}


provider "aws" {
    region  = "eu-west-2"
}

data "aws_caller_identity" "current" {}
data "aws_region" "current" {}

locals {
    application_name = "single-view-api"
    parameter_store = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
    vpc_id = "vpc-006989d0b2bb070d9"
    cidr = "0.0.0.0/0"
}

data "aws_subnet_ids" "all" {
    vpc_id = local.vpc_id
}

# Create ElastiCache Redis security group

resource "aws_security_group" "redis_sg" {
    vpc_id = local.vpc_id

    ingress {
        cidr_blocks = [local.cidr]
        from_port   = 6379
        to_port     = 6379
        protocol    = "tcp"
    }

    egress {
        from_port       = 0
        to_port         = 0
        protocol        = "-1"
        cidr_blocks = [local.cidr]
    }

}

# Create ElastiCache Redis subnet group

resource "aws_elasticache_subnet_group" "default" {
    name        = "subnet-group-single-view"
    description = "Private subnets for the ElastiCache instances: single view"
    subnet_ids  = data.aws_subnet_ids.all.ids
}


# Create ElastiCache Redis cluster

resource "aws_elasticache_cluster" "redis" {
    cluster_id           = "single-view-production"
    engine               = "redis"
    engine_version       = "7.0"
    node_type            = "cache.t4g.micro"
    num_cache_nodes      = 1
    parameter_group_name = "default.redis7"
    port                 = 6379
    subnet_group_name    = aws_elasticache_subnet_group.default.name
    security_group_ids   = [aws_security_group.redis_sg.id]
}

terraform {
  backend "s3" {
    bucket  = "terraform-state-disaster-recovery"
    encrypt = true
    region  = "eu-west-2"
    key     = "services/single-view-api/state"
  }
}

