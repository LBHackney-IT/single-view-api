.PHONY: setup
setup:
	docker-compose build

.PHONY: build
buid:
	docker-compose build SingleViewApi

.PHONY: serve
serve:
	docker-compose build SingleViewApi && docker-compose up SingleViewApi; docker-compose down

.PHONY: shell
shell:
	docker-compose run SingleViewApi bash

.PHONY: test
test:
	docker-compose up test-database & docker-compose build SingleViewApi-test && docker-compose up SingleViewApi-test; docker-compose down

.PHONY: test-db
test-db:
	docker-compose up test-database

.PHONY: lint
lint:
	-dotnet tool install -g dotnet-format
	dotnet tool update -g dotnet-format
	dotnet format

.PHONY: restart-db
restart-db:
	docker stop $$(docker ps -q --filter ancestor=test-database -a)
	-docker rm $$(docker ps -q --filter ancestor=test-database -a)
	docker rmi test-database
	docker-compose up -d test-database
