# Use this Makefile to connect to the remote database on AWS via port forwarding

.ONESHELL:

# Requires AWS CLI Profile matching corporate-${ENVIRONMENT} to be set up
# Requires AWS Session Manager Plugin to be installed:
# 	https://docs.aws.amazon.com/systems-manager/latest/userguide/session-manager-working-with-install-plugin.html
# On Windows you will need to run these commands using Git Bash, NOT Powershell / CMD

# -- Configuration --
# Local port to connect to on your machine
LOCAL_PORT = 5432
# For Parameter store URL Paths
ENVIRONMENT = development
# Set to AWSCLI Profile name
PROFILE = corporate-${ENVIRONMENT}


INSTANCE_ID_PATH := "/single-view/${ENVIRONMENT}/jump-box-instance-id"
POSTGRES_HOST_PATH := "/single-view/${ENVIRONMENT}/postgres-hostname"
DB_USERNAME_PATH := "/single-view/${ENVIRONMENT}/postgres-username"
DB_PASSWORD_PATH := "/single-view/${ENVIRONMENT}/postgres-password"

# -- Parameters --
INSTANCE_ID := $(shell aws ssm get-parameter --name ${INSTANCE_ID_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
REMOTE_HOST := $(shell aws ssm get-parameter --name ${POSTGRES_HOST_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
REMOTE_PORT := 5302
DB_USERNAME := $(shell aws ssm get-parameter --name ${DB_USERNAME_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)
DB_PASSWORD := $(shell aws ssm get-parameter --name ${DB_PASSWORD_PATH} --region "eu-west-2" --profile ${PROFILE} --query Parameter.Value --output text)

DATABASE_PARAMS = '{"host":["${REMOTE_HOST}"], "portNumber":["${REMOTE_PORT}"], "localPortNumber":["${LOCAL_PORT}"]}'

# -- Commands --

# Use this command to open a browser window and login to the AWS SSO service
sso_login:
	if (aws sts get-caller-identity --profile ${PROFILE})
	then
		echo "Session still valid"
	else
		echo "Session expired, logging in"
		aws sso login --profile ${PROFILE}
	fi

# Use this command to port forward to the remote database and connect to it at localhost:${LOCAL_PORT}
port_forwarding_to_sv_db:
	echo "Connecting to ${REMOTE_HOST} on local port ${LOCAL_PORT}
	echo Database: singleview
	echo Username: ${DB_USERNAME}
	echo Password: ${DB_PASSWORD}"
	aws ssm start-session \
		--target ${INSTANCE_ID} \
		--region=eu-west-2  \
		--profile=${PROFILE} \
		--document AWS-StartPortForwardingSessionToRemoteHost \
		--parameters ${DATABASE_PARAMS}

clear_port:
	fuser -n tcp -k ${LOCAL_PORT} && echo "Port ${LOCAL_PORT} cleared" || echo "Port ${LOCAL_PORT} not in use"
