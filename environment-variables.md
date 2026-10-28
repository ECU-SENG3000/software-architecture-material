# Environment Variables

1) What is an environment?

An environment is the set of values and settings that a running process inherits from its parent shell or OS. These values tell the program:

- where to find tools
- where the user’s home folder is
- which locale/language to use
- which config files to load
- which APIs, databases, or services to connect to

The environment affects how a program behaves without requiring hardcoded values in the code.

Examples:
- PATH tells the shell where to look for commands like git, node, python
- HOME tells programs where the current user’s home directory is
- TEMP/TMP tells programs where temporary files should go
- JAVA_HOME points to the Java installation
- DATABASE_URL tells an app which database to use

2) What is an environment variable?

An environment variable is a named value stored in the process environment. It is usually a string key/value pair, like:

- PATH = C:\Windows\System32;C:\Tools
- HOME = /home/student
- USER = student
- NODE_ENV = development

These variables are accessible to programs started from that environment.

3) How environment variables are inherited

This is the most important concept for troubleshooting:

- A shell starts as a process
- It receives environment variables from its parent process
- Any new programs launched from that shell inherit the shell’s environment
- If you change a variable after the shell has already started, that shell does not automatically know about the new value unless you update it in that shell session

Example:
- You open PowerShell
- It starts with PATH from Windows
- You run:
  - $env:PATH += ";C:\mytools"
- That changes PATH only for the current PowerShell session
- Any new terminal windows you open later may not inherit that unless you saved it permanently

In Bash/Zsh:
- export PATH="$PATH:/opt/mytool/bin"
- This affects the current shell and any child processes launched afterward
- It does not change other already-open shells

This is why students often get confused:
- “I added the environment variable, but the terminal still says it is missing.”
- The answer is usually: it was added after the shell started, or it was only set in a different shell/session.

4) Session variables vs permanent variables

There are three important scopes:

- Current session only
  - lasts only for the current terminal window or current process
  - good for quick testing

- User-level persistent
  - available for that user on future sessions
  - good for personal tools and development setup

- System-level persistent
  - applies machine-wide
  - usually requires administrator privileges
  - used for shared tools or system-level configuration

5) PowerShell behavior

Current session:
```powershell
$env:MY_VAR = "hello"
```

This works only for the current shell and child processes started from it.

Permanent user-level variable:
```powershell
[Environment]::SetEnvironmentVariable("MY_VAR", "hello", "User")
```

Permanent machine-level variable:
```powershell
[Environment]::SetEnvironmentVariable("MY_VAR", "hello", "Machine")
```

Read the value:
```powershell
$env:MY_VAR
```

Read the user/system variable directly:
```powershell
[System.Environment]::GetEnvironmentVariable("Path", "User")
[System.Environment]::GetEnvironmentVariable("Path", "Machine")
```

Delete:
```powershell
Remove-Item Env:MY_VAR
[Environment]::SetEnvironmentVariable("MY_VAR", $null, "User")
```

Important PowerShell troubleshooting note:
- If you set an environment variable in one PowerShell window, it is not automatically available in another already-open PowerShell window
- You must reopen the shell or refresh the environment in that shell

6) Bash/Zsh behavior

Current shell session:
```bash
export MY_VAR="hello"
```

This is available to the current shell and any commands launched from it.

Permanent user-level variable:
For Bash:
```bash
echo 'export MY_VAR="hello"' >> ~/.bashrc
source ~/.bashrc
```

For Zsh:
```bash
echo 'export MY_VAR="hello"' >> ~/.zshrc
source ~/.zshrc
```

Read the value:
```bash
echo $MY_VAR
```

Delete:
```bash
unset MY_VAR
```

Important note:
- Changing ~/.bashrc or ~/.zshrc does not update the current shell automatically
- You must either:
  - open a new terminal
  - or run:
    ```bash
    source ~/.bashrc
    ```
    or
    ```bash
    source ~/.zshrc
    ```

7) Why PATH is special

PATH is one of the most important environment variables. It tells the OS or shell where to find executable files.

Example:
```bash
echo $PATH
```

PowerShell:
```powershell
$env:PATH
```

When you type:
```bash
node -v
```

the system checks the directories in PATH, in order, until it finds node.

Adding PATH incorrectly causes many common issues:
- command not found
- old version of a tool gets used
- app cannot start because it cannot locate dependencies

Examples:
```bash
export PATH="/opt/tools/bin:$PATH"
```

PowerShell:
```powershell
$env:PATH += ";C:\tools\bin"
```

Important:
- If you add a directory after the shell already started, it is available only in that current session unless persisted
- If you add it to PATH permanently, you often still need to reopen the shell or reload the profile

8) When variables do not show up

Common causes:

- Variable set in a different shell
- Variable saved in a startup file but not reloaded
- Variable was set only for the current session
- Wrong scope: user-level vs system-level vs session-level
- Typo in variable name
- App launched before variable was set
- Using absolute path with quotes incorrectly
- PATH order wrong (newer tool hidden by older one)

Examples of common mistakes:

Wrong:
```bash
MY_VAR="hello"
```
This is not exported and may not be visible to child processes.

Correct:
```bash
export MY_VAR="hello"
```

Wrong:
```powershell
[Environment]::SetEnvironmentVariable("MY_VAR", "hello")
```
This sets it to null scope in some cases or may not do what you expect unless you specify the scope.

Correct:
```powershell
[Environment]::SetEnvironmentVariable("MY_VAR", "hello", "User")
```

9) Shell startup files

On Unix-like systems, many environment variables are stored in shell startup files. These are executed when a terminal starts.

Common ones:
- Bash: ~/.bashrc, ~/.bash_profile, ~/.profile
- Zsh: ~/.zshrc, ~/.zprofile
- Linux login shells may source profile files
- macOS Terminal often uses zsh by default

Example:
```bash
echo 'export JAVA_HOME=/usr/lib/jvm/java-17-openjdk' >> ~/.bashrc
echo 'export PATH="$JAVA_HOME/bin:$PATH"' >> ~/.bashrc
source ~/.bashrc
```

On Windows:
- environment variables are managed via system properties or PowerShell
- startup scripts are less central than on Unix
- tools often rely on PATH and user environment variables rather than shell config files

10) Environment variables for developers

Developers often set:

- JAVA_HOME
- MAVEN_HOME
- ANDROID_HOME
- NODE_ENV
- PYTHONPATH
- PATH
- DOTNET_ROOT
- GH_TOKEN or GITHUB_TOKEN
- DATABASE_URL
- ASPNETCORE_ENVIRONMENT

These values are often required by build tools, runtimes, CLIs, and applications.

Example:
```bash
export JAVA_HOME=/usr/lib/jvm/java-17-openjdk
export PATH="$JAVA_HOME/bin:$PATH"
```

PowerShell:
```powershell
$env:JAVA_HOME = "C:\Program Files\Java\jdk-17"
$env:PATH = "$env:JAVA_HOME\bin;$env:PATH"
```

11) Troubleshooting guide

If a program says it cannot find a command:
- Check PATH
- Confirm the executable exists
- Confirm the shell is using the updated PATH
- Restart the shell or source the profile file

Example:
```bash
echo $PATH
which node
node -v
```

PowerShell:
```powershell
$env:PATH
Get-Command node
node -v
```

If a variable is missing:
- Check whether it was set only in the current session
- Check whether it was set in a startup file
- Open a new shell
- Reload config file

If a program sees a different value than expected:
- Run in a fresh shell and print the value
- Confirm you are not accidentally setting the same variable in another script
- Confirm you are reading the correct scope (user vs machine vs session)

12) Process-specific environment

A process can also receive environment variables directly from the command that starts it.

Examples:
```bash
MY_VAR=abc node app.js
```

PowerShell:
```powershell
$env:MY_VAR = "abc"; node app.js
```

This is useful when:
- you only want a variable for one run
- you do not want to permanently store it
- you want to test a build or configuration without changing the shell environment

13) Best practices for students

- Use environment variables for configuration, not for logic you can hardcode
- Keep shells and scripts consistent
- Prefer user-level variables for personal dev tools
- Use system-wide variables only when you truly need them
- Save values in startup files when they should persist
- Reload or reopen terminal after edits
- Avoid putting secrets directly into shell config files unless necessary
- Prefer process-specific environment for one-off testing

14) Very practical rule to remember

If you add a variable after the shell starts:
- it is not automatically available to the shell you are currently using
- it is available only to future processes launched after the change
- if you want it in the current shell, set it in that shell or reload the config

This is the rule behind most “why isn’t it working?” situations.

15) Quick examples

Bash:
```bash
export PATH="$PATH:/my/tools"
echo $PATH
source ~/.bashrc
```

Zsh:
```zsh
export PATH="$PATH:/my/tools"
source ~/.zshrc
```

PowerShell:
```powershell
$env:PATH += ";C:\my\tools"
$env:MY_VAR = "abc"
```

Permanent user setting:
```powershell
[Environment]::SetEnvironmentVariable("MY_VAR", "abc", "User")
```

Persistent Bash setting:
```bash
echo 'export MY_VAR="abc"' >> ~/.bashrc
source ~/.bashrc
```

16) Summary

Environment variables are how operating systems and shells provide runtime configuration to programs.

The key ideas:
- variables live in the environment
- shells inherit them from parent processes
- child processes inherit from the current shell
- changes made after a shell starts do not automatically update already-running shells
- persistent configuration is usually written to profile files or system/user environment settings
- PATH is a critical variable that affects command discovery

