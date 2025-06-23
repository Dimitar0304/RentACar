# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and all project files
COPY *.sln ./
COPY RentACar/*.csproj ./RentACar/
COPY RentACar.Core/*.csproj ./RentACar.Core/
COPY RentACar.Infrastructure/*.csproj ./RentACar.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Publish the app
WORKDIR /app/RentACar
RUN dotnet publish -c Release -o /out

# Use the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 80
ENTRYPOINT ["dotnet", "RentACar.dll"]
CMD /bin/bash -c "sleep 15 && dotnet RentACar.dll"
