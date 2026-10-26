
# Node + npm Survival Guide (for beginners)

This guide is for students who are new to Node.js and npm and want to confidently start and run JavaScript projects.

You’ll learn:

- What Node.js is and why we use it
- What npm is and how projects are structured
- How to initialize a project (`package.json`)
- How to install and use packages
- What scripts are and how to run them
- What `node_modules` and lockfiles are
- Common beginner problems (especially on Windows)

---

## 1) What is Node.js?

**Node.js** is a runtime that lets you run JavaScript **outside the browser**.

Typical uses:

- Web servers (Express)
- Command-line tools
- Build tooling (bundlers, linters, formatters)
- API servers

Important: Node and the browser both run JavaScript, but they have different APIs.

- Browser APIs: `document`, DOM, `window`
- Node APIs: filesystem (`fs`), paths (`path`), processes (`process`)

---

## 2) What is npm?

**npm** is a package manager for JavaScript.

It lets you:

- Download libraries (“packages”) from the npm registry
- Manage dependencies for a project
- Run project scripts (like `npm start`)

When you install Node.js, you usually get npm included.

Useful commands:

- Check versions: `node -v` and `npm -v`
- Install deps: `npm install`
- Run scripts: `npm run <script>`

---

## 3) What is a “JavaScript project”?

Most Node projects look like:

```
my-project/
	package.json
	package-lock.json
	node_modules/
	server.js (or src/index.js)
	README.md
```

- `package.json` describes the project and dependencies
- `node_modules/` contains installed packages (big, auto-generated)
- `package-lock.json` locks exact versions for repeatable installs

---

## 4) Initialize a project (create `package.json`)

### Option A: interactive

```bash
npm init
```

### Option B: fast default (common in class)

```bash
npm init -y
```

This creates a `package.json` with defaults.

---

## 5) Understanding `package.json`

`package.json` is the “manifest” for your project.

Common fields:

- `name`: project name
- `version`: project version
- `private`: prevents accidental publishing if set to `true`
- `main`: entry file (CommonJS projects), often `index.js` or `server.js`
- `type`: module system (`"commonjs"` default or `"module"` for ESM)
- `scripts`: shortcuts you can run with npm
- `dependencies`: packages needed to run the app
- `devDependencies`: packages used only for development (tests, linters, etc.)

Example:

```json
{
	"name": "my-app",
	"version": "1.0.0",
	"private": true,
	"main": "server.js",
	"scripts": {
		"start": "node server.js",
		"dev": "node --watch server.js"
	},
	"dependencies": {
		"express": "^4.19.2"
	}
}
```

---

## 6) Install packages (dependencies)

### Install a runtime dependency

```bash
npm install express
```

This adds it to `dependencies`.

### Install a development-only dependency

```bash
npm install --save-dev nodemon
```

This adds it to `devDependencies`.

### Install what’s already listed in `package.json`

If you clone/download a repo and it already has `package.json`, run:

```bash
npm install
```

That reads `package.json` + `package-lock.json` and populates `node_modules/`.

---

## 7) What is `node_modules/` and why is it huge?

`node_modules/` contains the actual code for installed packages.

- It can be very large because packages depend on other packages.
- You almost never edit files inside `node_modules/`.
- You generally do **not** commit `node_modules/` to Git.

Typical `.gitignore` entry:

```
node_modules/
```

---

## 8) Lockfiles (`package-lock.json`) and why they matter

`package-lock.json` records the **exact** versions installed.

Why it’s useful:

- Everyone gets the same versions when running `npm install`
- Builds are more repeatable
- Fewer “works on my machine” issues

In most class projects, you *do* commit `package-lock.json`.

---

## 9) Semantic Versioning (SemVer) in one minute

Version format: `MAJOR.MINOR.PATCH` (example: `4.19.2`)

- PATCH: bug fixes
- MINOR: new features (backwards compatible)
- MAJOR: breaking changes

In `package.json` you’ll see ranges:

- `"express": "^4.19.2"` allows updates that don’t change the major version (4.x.x)
- `"express": "~4.19.2"` allows patch updates only (4.19.x)
- `"express": "4.19.2"` pins exact version

---

## 10) Running code: `node` vs `npm`

### Run a file directly with Node

```bash
node server.js
```

### Run a script from `package.json`

```bash
npm start
```

`npm start` runs the `scripts.start` command.

For other scripts:

```bash
npm run dev
npm run test
```

---

## 11) The `scripts` section (super important)

Scripts are shortcuts that keep your commands consistent.

Example:

```json
"scripts": {
	"start": "node server.js",
	"dev": "nodemon server.js"
}
```

Then you run:

- `npm start`
- `npm run dev`

Why scripts are nice:

- Everyone runs the app the same way
- You don’t have to remember long commands

---

## 12) CommonJS vs ES Modules (why `require` vs `import`?)

There are two module systems you’ll see in Node projects.

### CommonJS (classic Node)

```js
const express = require('express');
module.exports = { /* ... */ };
```

### ES Modules (modern)

```js
import express from 'express';
export function helper() {}
```

How Node decides:

- If `package.json` has `"type": "module"`, `.js` files use `import/export`.
- If not, `.js` files use CommonJS (`require/module.exports`).

Beginner advice: don’t mix module styles in one project unless you know why.

---

## 13) `npx` (run tools without installing globally)

`npx` runs a package executable, even if it’s not installed globally.

Examples:

- `npx eslint --version`
- `npx nodemon server.js`

In general, prefer local installs + `npm run` scripts over global installs.

---

## 14) Common Windows gotchas

### Command separators differ between shells

- In **PowerShell**, `;` separates commands.
- In **cmd.exe**, use `&&`.

Example:

- PowerShell: `cd http-demo; npm install`
- cmd.exe: `cd /d http-demo && npm install`

### `node --watch` might not work

Some scripts use `node --watch` (auto-restart). If your Node is older, it can fail.

Fixes:

- Use `npm start` (no watch)
- Or install and use `nodemon` as a dev dependency
- Or upgrade Node

---

## 15) A tiny Express project from scratch (quick recipe)

```bash
mkdir my-express-app
cd my-express-app
npm init -y
npm install express
```

Create `server.js`:

```js
const express = require('express');
const app = express();

app.get('/', (req, res) => {
	res.type('text/plain').send('Hello from Express');
});

app.listen(3000, () => {
	console.log('Listening on http://localhost:3000');
});
```

Add scripts in `package.json`:

```json
"scripts": {
	"start": "node server.js"
}
```

Run it:

```bash
npm start
```

---

## 16) Troubleshooting checklist

- “Command not found: node” → Node isn’t installed or PATH isn’t set
- “Cannot find module …” → run `npm install`
- “EADDRINUSE 3000” → port already in use (close the other server or change the port)
- Script fails in `cmd` but works in PowerShell → likely a command separator difference

