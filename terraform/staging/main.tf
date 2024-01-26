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
    parameter_store  = "arn:aws:ssm:${data.aws_region.current.name}:${data.aws_caller_identity.current.account_id}:parameter"
    vpc_id           = "vpc-06bc937006240a256"
    cidr             = "0.0.0.0/0"
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
        from_port   = 0
        to_port     = 0
        protocol    = "-1"
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
    cluster_id           = "single-view-staging"
    engine               = "redis"
    engine_version       = "7.x"
    node_type            = "cache.t4g.micro"
    num_cache_nodes      = 1
    parameter_group_name = "default.redis7"
    port                 = 6379
    subnet_group_name    = aws_elasticache_subnet_group.default.name
    security_group_ids   = [aws_security_group.redis_sg.id]
}

terraform {
    backend "s3" {
        bucket  = "terraform-state-corporate-staging"
        encrypt = true
        region  = "eu-west-2"
        key     = "services/single-view-api/state"
    }
}

################################################################################
# Supporting Resources
################################################################################

data "aws_ssm_parameter" "uh_postgres_db_password" {
    name = "/single-view/staging/postgres-password"
}

data "aws_ssm_parameter" "uh_postgres_username" {
    name = "/single-view/staging/postgres-username"
}

#####
# DB
#####
module "postgres_db" {
    source                     = "github.com/LBHackney-IT/aws-hackney-common-terraform.git//modules/database/postgres"
    environment_name           = "staging"
    vpc_id                     = local.vpc_id
    db_identifier              = "singleview"
    db_name                    = "singleview"
    db_port                    = 5302
    subnet_ids                 = data.aws_subnet_ids.all.ids
    db_engine                  = "postgres"
    db_engine_version          = "12.14" //DMS does not work well with v12
    db_instance_class          = "db.t3.micro"
    db_allocated_storage       = 20
    maintenance_window         = "sun:10:00-sun:10:30"
    db_username                = data.aws_ssm_parameter.uh_postgres_username.value
    db_password                = data.aws_ssm_parameter.uh_postgres_db_password.value
    storage_encrypted          = false
    multi_az                   = false //only true if production deployment
    publicly_accessible        = false
    project_name               = "single view"
}
