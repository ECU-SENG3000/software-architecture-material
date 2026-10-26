# 🟦 .NET CLI — Essential Command Reference

## 📦 Installation & Info
- `dotnet --version` — Show installed .NET SDK version  
- `dotnet --info` — Display environment and runtime information  
- `dotnet --list-sdks` — List installed SDKs  
- `dotnet --list-runtimes` — List installed runtimes  

---

## 🧱 Creating Projects & Solutions
- `dotnet new <template>` — Create a new project  
- `dotnet new console` — Create a console app  
- `dotnet new web` — Create an ASP.NET Core web app  
- `dotnet new webapi` — Create an ASP.NET Core Web API  
- `dotnet new mvc` — Create an MVC project  
- `dotnet new sln` — Create a solution file  
- `dotnet sln add <project>` — Add a project to a solution  
- `dotnet sln list` — List projects in a solution  

---

## ▶️ Running & Building
- `dotnet run` — Run the current project  
- `dotnet run --project <path>` — Run a specific project  
- `dotnet build` — Build the project  
- `dotnet build --configuration Release` — Build in Release mode  
- `dotnet watch run` — Run with file‑watching and auto‑reload  

---

## 🧪 Testing
- `dotnet test` — Run tests in the solution  
- `dotnet test --filter <expression>` — Run specific tests  
- `dotnet test --logger "console;verbosity=detailed"` — Detailed test output  

---

## 📦 Package Management (NuGet)
- `dotnet add package <name>` — Add a NuGet package  
- `dotnet remove package <name>` — Remove a NuGet package  
- `dotnet list package` — List installed packages  
- `dotnet restore` — Restore dependencies  

---

## 🏗️ Publishing
- `dotnet publish` — Publish the project  
- `dotnet publish -c Release` — Publish in Release mode  
- `dotnet publish -r win-x64` — Publish for a specific runtime  
- `dotnet publish --self-contained true` — Produce a self‑contained executable  

---

## 🧩 Project & Solution Utilities
- `dotnet clean` — Clean build outputs  
- `dotnet format` — Format code according to .editorconfig  
- `dotnet tool install -g <tool>` — Install a global .NET tool  
- `dotnet tool list -g` — List global tools  
- `dotnet tool update -g <tool>` — Update a global tool  

---

## 🎨 Common Templates
- `dotnet new console`  
- `dotnet new classlib`  
- `dotnet new web`  
- `dotnet new webapi`  
- `dotnet new mvc`  
- `dotnet new blazorserver`  
- `dotnet new blazorwasm`  
- `dotnet new maui` (if MAUI workload installed)  

---

## 🧱 Workloads (MAUI, WebAssembly, etc.)
- `dotnet workload list` — List installed workloads  
- `dotnet workload search` — Search available workloads  
- `dotnet workload install maui` — Install .NET MAUI  
- `dotnet workload update` — Update workloads  

---

## 🛠️ Help & Documentation
- `dotnet help` — Show help  
- `dotnet <command> --help` — Help for a specific command  
