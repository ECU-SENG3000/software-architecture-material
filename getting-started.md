# Software Architecture Workstation Setup

## Goal

By the end of this setup, you will:

- download and install Docker Desktop
- download and install .Net 10 SDK
- download and install aspire CLI
- download and install / update to latest version of VS Code 
- install the required VS Code extensions in a dedicated **software-architecture** profile


---

## What these tools are

### Docker
Docker lets you run software in an isolated environment called a **container**.

- An **image** is the downloadable template.
- A **container** is the running instance of that image.
- A **volume** is persistent storage that survives even if the container is deleted.

For this course, we will use an orchestration tool called Aspire that will automatically download and run containers for us.  Docker Desktop must be installed and running for this to work.


### VS Code profile
A VS Code **profile** lets you keep class-specific extensions and settings separate from your normal development setup. For this course, we will use a profile named **DBMS**.

---

## Before you begin

Make sure you have:

- administrator access to your workstation
- a stable internet connection
- at least 10 GB of free disk space
- VS Code installed or permission to install it

---

## 1. Install Docker Desktop

Install Docker Desktop for your operating system:

- **Windows:** [Docker Desktop for Windows](https://docs.docker.com/desktop/setup/install/windows-install/)
  - Most windows users when use the x86_64 version. 
- **macOS:** [Docker Desktop for Mac](https://docs.docker.com/desktop/setup/install/mac-install/)
  - M1, M2, M3, M4, etc... mac users will use the Apple silicon version.
  - If you have an older mac that does not have M chip, then you will use the Intel chip version.
- **Linux:** [Docker Desktop for Linux](https://docs.docker.com/desktop/setup/install/linux/)
  - Select your distribution and following the provided website instructions.
After installation:

1. Start Docker Desktop.
2. Wait until Docker Desktop shows that it is running.
3. Leave Docker Desktop open.

> [!NOTE]
> On Windows, Docker Desktop may ask you to enable WSL 2. Accept the prompt and complete that setup if needed.

> [!NOTE]
> On Apple Silicon Macs, Docker may warn you about platform differences. If you later see a platform mismatch error when starting SQL Server, use the Apple Silicon command shown in Step 5.

---

## 2. Confirm Docker is installed and working

Open a terminal:

- **Windows:** PowerShell
- **macOS:** Terminal
- **Linux:** Terminal

Run:

```bash
docker --version
```

You should see a Docker version number.

Next, test Docker end to end:

```bash
docker run hello-world
```

You should see a success message from Docker explaining that your installation appears to be working correctly.

If this command fails, stop here and fix Docker Desktop before continuing.

---

## 3. Install the .NET 10 SDK

Download the latest .NET 10 SDK from Microsoft:

- https://dotnet.microsoft.com/en-us/download/dotnet/10.0

Use the same OS / CPU logic to select the correct SDK installer.
- Most windows users will download and run Windows x64 installer
- M1-M4 mac users will download and run macOS Arm64 installer 
- Most linux users will follow the Linux x64 instructions.

After installation, confirm the SDK is available:

```bash
dotnet --version
dotnet --list-sdks
```

As of 8/24/2026, the most recent version is 10.0.400.


---

## 4. Install or update Visual Studio Code

If you do not already have VS Code, install it here:

- [Visual Studio Code](https://code.visualstudio.com/)

If VS Code is already installed, update it to the latest version before continuing.

---

## 5. Create and use a VS Code profile named software-architecture

Using a dedicated profile keeps our course work from conflicting with other development tools.

### Create the profile from the VS Code interface

1. Open VS Code.
2. Select the **gear icon** in the lower-left corner.
3. Choose **Profiles**.
4. Create a new profile named **software-architecture**.
5. Switch to that profile.

### Command-line option

If the `code` command is available in your terminal, you can open VS Code with the profile by running:

```bash
code --profile software-architecture
```

> [!NOTE]
> If `code` is not recognized:
>
> - on **Windows**, reinstall or update VS Code and make sure the option to add `code` to PATH is enabled
> - on **macOS**, open VS Code and run **Shell Command: Install 'code' command in PATH** from the Command Palette
> - on **Linux**, the `code` command is usually installed automatically with VS Code

---

## 6. Install the required VS Code extensions

### Command-line option

If the `code` command works on your machine, run:

```bash
code --profile software-architecture --install-extension bierner.markdown-mermaid
code --profile software-architecture --install-extension alexcvzz.vscode-sqlite
code --profile software-architecture --install-extension humao.rest-client
code --profile software-architecture --install-extension jaufrdevosse.litedb-vscode
code --profile software-architecture --install-extension microsoft-aspire.aspire-vscode
code --profile software-architecture --install-extension ms-azuretools.vscode-containers
code --profile software-architecture --install-extension ms-azuretools.vscode-docker
code --profile software-architecture --install-extension ms-dotnettools.csdevkit
code --profile software-architecture --install-extension ms-dotnettools.csharp
code --profile software-architecture --install-extension ms-dotnettools.dotnet-maui
code --profile software-architecture --install-extension ms-dotnettools.vscode-dotnet-runtime
code --profile software-architecture --install-extension ms-mssql.data-workspace-vscode
code --profile software-architecture --install-extension ms-mssql.mssql
code --profile software-architecture --install-extension ms-mssql.sql-database-projects-vscode
code --profile software-architecture --install-extension ms-vscode-remote.remote-containers
code --profile software-architecture --install-extension ms-vscode.remote-explorer
code --profile software-architecture --install-extension ms-vscode.remote-server
```

### Course MCP servers

Enable the MCP marketplace in VS Code.

- Open Extensions
- Search for @mcp
- The MCP panel will display a button that reads [Enable MCP Servers Marketplace]
- Press the button to enable the MCP Marketplace

Search for and add the following MCP servers:

- `@mcp microsoftdocs`
- `@mcp com.microsoft/nuget`

Restart VS Code.

---

## 7. Install Aspire CLI

Documentation:  https://aspire.dev/get-started/install-cli/

macOS
```bash
brew install --cask microsoft/aspire/aspire
```

Windows
```bash
winget install Microsoft.Aspire
```

Universal NPM Option
```bash
npm install -g @microsoft/aspire-cli
```

Verify Aspire installed correctly
```bash
aspire --version
```

Lastest version as of 8/24/2026 is 13.5.0+