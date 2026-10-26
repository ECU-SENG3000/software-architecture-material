# 🧰 GitHub CLI (gh) — Essential Command Reference

## 🔐 Authentication & Configuration
- `gh auth login` — Authenticate with GitHub  
- `gh auth logout` — Log out of GitHub  
- `gh auth status` — Show authentication status  
- `gh config set <key> <value>` — Set configuration values  
- `gh config get <key>` — Read a configuration value  
- `gh config list` — List all config settings  

---

## 📦 Repository Management
- `gh repo create` — Create a new repository  
- `gh repo clone <repo>` — Clone a repository  
- `gh repo fork <repo>` — Fork a repository  
- `gh repo view` — View repository details  
- `gh repo list` — List your repositories  

---

## 🔀 Pull Requests
- `gh pr create` — Create a pull request  
- `gh pr view` — View PR details  
- `gh pr checkout <number>` — Check out a PR branch  
- `gh pr merge <number>` — Merge a PR  
- `gh pr list` — List pull requests  
- `gh pr status` — Show PRs related to your current branch  

---

## 🐞 Issues
- `gh issue create` — Create an issue  
- `gh issue view <number>` — View an issue  
- `gh issue list` — List issues  
- `gh issue close <number>` — Close an issue  

---

## 🚀 GitHub Actions
- `gh workflow list` — List workflows  
- `gh workflow run <name>` — Trigger a workflow  
- `gh run list` — List workflow runs  
- `gh run view <id>` — View a specific run  

---

## 🏷️ Releases
- `gh release create <tag>` — Create a release  
- `gh release upload <tag> <file>` — Upload release assets  
- `gh release list` — List releases  
- `gh release delete <tag>` — Delete a release  

---

## 📄 Gists
- `gh gist create <file>` — Create a gist  
- `gh gist list` — List your gists  
- `gh gist view <id>` — View a gist  

---

## 🌐 Browsing & Navigation
- `gh browse` — Open the current repo in the browser  
- `gh pr view --web` — Open a PR in the browser  
- `gh issue view --web` — Open an issue in the browser  

---

## 🧩 Extensions
- `gh extension install <repo>` — Install an extension  
- `gh extension list` — List installed extensions  
- `gh extension upgrade` — Upgrade extensions  

---

## 🛠️ Utility Commands
- `gh help` — Show help for any command  
- `gh <command> --help` — Show help for a subcommand  
- `gh alias set <name> <command>` — Create a command alias  
- `gh environment` — View environment variables used by GitHub CLI  
