FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Timesheets/Timesheets.csproj", "Timesheets/"]
RUN dotnet restore "Timesheets/Timesheets.csproj"
COPY . .
WORKDIR "/src/Timesheets"
RUN dotnet build "Timesheets.csproj" -c Release -o /app

FROM build AS publish
RUN apt-get update -yq && apt-get upgrade -yq && apt-get install -yq curl
RUN curl -sL https://deb.nodesource.com/setup_8.x | bash - && apt-get install -yq nodejs build-essential
RUN npm install -g npm
RUN npm install -g @quasar/cli
RUN dotnet publish "Timesheets.csproj" -c Release -o /app
RUN cp -r /src/Timesheets/client/dist /app/client/dist

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Timesheets.dll"]