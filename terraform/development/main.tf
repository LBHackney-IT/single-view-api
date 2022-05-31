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


resource "aws_security_group" "db_sg" {
    vpc_id = local.vpc_id

    ingress {
        cidr_blocks = [local.cidr]
        from_port   = 5432
        to_port     = 5432
        protocol    = "tcp"
    }
}

##############################################################
# Data sources to get VPC, subnets and security group details
##############################################################
data "aws_subnet_ids" "all" {
    vpc_id = local.vpc_id
}

data "aws_security_group" "default" {
    vpc_id = local.vpc_id
    name   = "default"
}

#####
# DB
#####
module "db" {
    source  = "../modules/rds"

    identifier = "singleviewapi"

    engine         = "aurora-postgresql"
    engine_version = "11.9"

    instance_class    = "db.t3.medium"
    allocated_storage = 5
    storage_encrypted = false

    # kms_key_id        = "arm:aws:kms:<region>:<account id>:key/<kms key id>"
    name = local.application_name

    # NOTE: Do NOT use 'user' as the value for 'username' as it throws:
    # "Error creating DB Instance: InvalidParameterValue: MasterUsername
    # user cannot be used as it is a reserved word used by the engine"
    username = "dbuser"

    password = "YourPwdShouldBeLongAndSecure!"
    port     = "5432"

    vpc_security_group_ids = [data.aws_security_group.default.id]

    maintenance_window = "Mon:00:00-Mon:03:00"
    backup_window      = "03:00-06:00"

    # disable backups to create DB faster
    backup_retention_period = 0

    enabled_cloudwatch_logs_exports = ["postgresql", "upgrade"]

    # DB subnet group
    subnet_ids = data.aws_subnet_ids.all.ids

    family = "postgres9.6"

    # DB option group
    major_engine_version = "9.6"

    # Snapshot name upon DB deletion
    final_snapshot_identifier = "demodb"

    # Database Deletion Protection
    deletion_protection = false
}
