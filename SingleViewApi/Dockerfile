FROM mcr.microsoft.com/dotnet/sdk:6.0

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./SingleViewApi/SingleViewApi.csproj ./
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./SingleViewApi.csproj

# Copy everything else and build
COPY ./SingleViewApi ./
RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

RUN dotnet tool install -g dotnet-ef

EXPOSE ${PORT:-3000}
CMD ASPNETCORE_URLS=http://+:${PORT:-3000} dotnet ./out/SingleViewApi.dll
