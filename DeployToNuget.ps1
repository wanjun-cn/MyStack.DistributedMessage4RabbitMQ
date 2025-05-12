# Release
dotnet restore
dotnet build -c Release
 
cd src/MyStack.DistributedMessage4RabbitMQ/bin/Release
dotnet nuget push DistributedMessage4RabbitMQ.*.nupkg  --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate
cd ../../../../

