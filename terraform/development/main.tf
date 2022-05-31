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
    vpc_id = "vpc-05c7e3d5ffd5f00a4"
    cidr = "0.0.0.0/0"
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
        cidr_blocks     = [local.cidr]
    }

}

# Create ElastiCache Redis subnet group

resource "aws_elasticache_subnet_group" "default" {
    name        = "subnet-group-single-view"
    description = "Private subnets for the ElastiCache instances: single view"
    subnet_ids  = ["subnet-068ec0a87972e4714", "subnet-05fe49c939c6c7b1e", "subnet-07ba583cbf5207869", "subnet-0b0b79fab8c3fd705"]
}


# Create ElastiCache Redis cluster

resource "aws_elasticache_cluster" "redis" {

    cluster_id           = "single-view-development"
    engine               = "redis"
    engine_version       = "3.2.10"
    node_type            = "cache.m4.large"
    num_cache_nodes      = 1
    parameter_group_name = "default.redis3.2"
    port                 = 6379
    subnet_group_name    = aws_elasticache_subnet_group.default.name
    security_group_ids   = [aws_security_group.redis_sg.id]
}

terraform {
    backend "s3" {
        bucket  = "terraform-state-corporate-development"
        encrypt = true
        region  = "eu-west-2"
        key     = "services/single-view-api/state"
    }
}


################################################################################
# Supporting Resources
################################################################################


data "aws_subnet_ids" "all" {
    vpc_id = local.vpc_id
}

data "aws_ssm_parameter" "uh_postgres_db_password" {
    name = "/single-view/development/postgres-password"
}

data "aws_ssm_parameter" "uh_postgres_username" {
    name = "/single-view/development/postgres-username"
}

#####
# DB
#####
module "postgres_db_staging" {
    source = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/postgres"
    environment_name = "development"
    vpc_id = local.vpc_id
    db_identifier = "singleview"
    db_name = "singleview"
    db_port  = 5302
    subnet_ids = data.aws_subnet_ids.all.ids
    db_engine = "postgres"
    db_engine_version = "11.1" //DMS does not work well with v12
    db_instance_class = "db.t2.micro"
    db_allocated_storage = 20
    maintenance_window = "sun:10:00-sun:10:30"
    db_username = data.aws_ssm_parameter.uh_postgres_username.value
    db_password = data.aws_ssm_parameter.uh_postgres_db_password.value
    storage_encrypted = false
    multi_az = false //only true if production deployment
    publicly_accessible = false
    project_name = "single view"
}
