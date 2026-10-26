# 📦 npm, npx, and node — Essential Command Reference

## 🟦 Node.js (node)
- `node <file>` — Run a JavaScript file  
- `node` — Start the Node REPL  
- `node --version` — Show Node version  
- `node --watch <file>` — Run a file and restart on changes  
- `node --inspect <file>` — Run with debugger enabled  
- `node --trace-warnings` — Show detailed warnings  
- `node --experimental-modules` — Enable ES module support (older Node versions)  

---

## 🟩 npm — Node Package Manager
### 🔧 Package Installation
- `npm install` — Install all dependencies from package.json  
- `npm install <pkg>` — Install a package locally  
- `npm install -g <pkg>` — Install a package globally  
- `npm uninstall <pkg>` — Remove a package  
- `npm update` — Update all dependencies  
- `npm outdated` — List outdated packages  

### 📦 Package Management
- `npm init` — Create a new package.json interactively  
- `npm init -y` — Create a default package.json  
- `npm config get <key>` — Read npm config  
- `npm config set <key> <value>` — Set npm config  

### 🏃 Scripts
- `npm run <script>` — Run a script from package.json  
- `npm run` — List available scripts  
- `npm test` — Run the "test" script  
- `npm start` — Run the "start" script  

### 🔍 Info & Utilities
- `npm list` — Show installed packages  
- `npm list -g` — Show globally installed packages  
- `npm view <pkg>` — Show package metadata  
- `npm audit` — Scan for vulnerabilities  
- `npm audit fix` — Attempt to fix vulnerabilities  

---

## 🟧 npx — Execute Packages Without Installing
- `npx <command>` — Run a package from npm without installing it  
- `npx create-react-app my-app` — Run a scaffolding tool  
- `npx vite` — Run Vite from the network cache  
- `npx eslint .` — Run ESLint without global install  
- `npx --yes <command>` — Skip confirmation prompts  
- `npx --package <pkg> <cmd>` — Run a command from a specific package  

---

## 🧩 Common Use Cases
- **Scaffolding projects:**  
  - `npx create-react-app my-app`  
  - `npm create vite@latest my-app`  
- **Local dev servers:**  
  - `npm run dev`  
- **Production builds:**  
  - `npm run build`  
- **Running scripts:**  
  - `node server.js`  

---

## 📝 Version Checks
- `node --version`  
- `npm --version`  
- `npx --version`  