FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["BooksApi.csproj", "./"]
RUN dotnet restore "BooksApi.csproj"

COPY . .
RUN dotnet publish "BooksApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BooksApi.dll"]