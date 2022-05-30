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

#
#locals {
#    name   = "complete-postgresql"
#    region = "eu-west-1"
#    tags = {
#        Owner       = "user"
#        Environment = "dev"
#    }
#}

################################################################################
# Supporting Resources
################################################################################

module "security_group" {
    source  = "terraform-aws-modules/security-group/aws"
    version = "~> 4.0"

    name        = local.application_name
    description = "Complete PostgreSQL example security group"
    vpc_id      = local.vpc_id

    # ingress
    ingress_with_cidr_blocks = [
        {
            from_port   = 5432
            to_port     = 5432
            protocol    = "tcp"
            description = "PostgreSQL access from within VPC"
            cidr_blocks = [local.cidr]
        },
    ]
}

################################################################################
# RDS Module
################################################################################
module "rds" {
    source  = "terraform-aws-modules/rds/aws"
    version = "4.4.0"
    # insert the 38 required variables here

    identifier = local.application_name

    # All available versions: https://docs.aws.amazon.com/AmazonRDS/latest/UserGuide/CHAP_PostgreSQL.html#PostgreSQL.Concepts
    engine               = "postgres"
    engine_version       = "14.1"
    family               = "postgres14" # DB parameter group
    major_engine_version = "14"         # DB option group
    instance_class       = "db.t2.micro"

    allocated_storage     = 20
    max_allocated_storage = 100

    # NOTE: Do NOT use 'user' as the value for 'username' as it throws:
    # "Error creating DB Instance: InvalidParameterValue: MasterUsername
    # user cannot be used as it is a reserved word used by the engine"
    db_name  = "completePostgresql"
    username = "complete_postgresql"
    port     = 5432

    multi_az               = true
    db_subnet_group_name   = aws_elasticache_subnet_group.default.name
    vpc_security_group_ids = [module.security_group.security_group_id]

    maintenance_window              = "Mon:00:00-Mon:03:00"
    backup_window                   = "03:00-06:00"
    enabled_cloudwatch_logs_exports = ["postgresql", "upgrade"]
    create_cloudwatch_log_group     = true

    backup_retention_period = 0
    skip_final_snapshot     = true
    deletion_protection     = false

    performance_insights_enabled          = true
    performance_insights_retention_period = 7
    create_monitoring_role                = true
    monitoring_interval                   = 60
    monitoring_role_name                  = "single-view-monitoring-role-name"
    monitoring_role_description           = "Description for monitoring role"

    parameters = [
        {
            name  = "autovacuum"
            value = 1
        },
        {
            name  = "client_encoding"
            value = "utf8"
        }
    ]

    db_option_group_tags = {
        "Sensitive" = "low"
    }
    db_parameter_group_tags = {
        "Sensitive" = "low"
    }
}
