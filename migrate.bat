powershell -Command "dotnet ef migrations remove --force --project '.\WebApp.Dal\WebApp.Dal.csproj' --startup-project '.\WebApp.Server\WebApp.Server.csproj'"
powershell -Command "dotnet ef database drop --force --project '.\WebApp.Server\'"
powershell -Command "dotnet ef migrations add init --project .\WebApp.Dal\WebApp.Dal.csproj --startup-project .\WebApp.Server\WebApp.Server.csproj"
powershell -Command "dotnet ef database update --project .\WebApp.Dal\WebApp.Dal.csproj --startup-project .\WebApp.Server\WebApp.Server.csproj"
pause