#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5080

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ASME.WebEnd/ASME.WebEnd.csproj", "ASME.WebEnd/"]
RUN dotnet restore "ASME.WebEnd/ASME.WebEnd.csproj"
COPY . .
WORKDIR "/src/ASME.WebEnd"
RUN dotnet build "ASME.WebEnd.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ASME.WebEnd.csproj" -c Release -o /app/publish

ENV ASPNETCORE_URLS=http://0.0.0.0:5080

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ASME.WebEnd.dll"]