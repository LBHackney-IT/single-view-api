FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./SingleViewApi.sln ./
COPY ./SingleViewApi/SingleViewApi.csproj ./SingleViewApi/
COPY ./SingleViewApi.Tests/SingleViewApi.Tests.csproj ./SingleViewApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./SingleViewApi/SingleViewApi.csproj
RUN dotnet restore ./SingleViewApi.Tests/SingleViewApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out SingleViewApi.Tests/SingleViewApi.Tests.csproj

CMD dotnet test
