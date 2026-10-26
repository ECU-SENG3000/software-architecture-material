
# Modern JavaScript Survival Guide 

This guide is for students learning **modern JavaScript (ES6+)** for the web.

Goals:

- Write clean JS using modern syntax (`let/const`, arrow functions, modules)
- Understand the DOM and event-driven programming
- Make HTTP requests with `fetch` + `async/await`
- Debug problems quickly

---

## 1) Where JavaScript runs

JavaScript commonly runs in two places:

- **Browser** (front-end): interacts with HTML/CSS via the DOM
- **Node.js** (back-end/tools): runs on your computer/server

Many language features are shared, but some APIs are environment-specific.

---

## 2) The “downloaded page is an initializer” (JS version)

When you load a webpage:

1. Browser downloads HTML (initializer)
2. Browser builds the DOM
3. Browser downloads CSS/JS files referenced by HTML
4. JavaScript runs and can:
	 - attach event listeners (click handlers)
	 - modify the DOM (change text, add/remove elements)
	 - fetch data from servers

Huge beginner insight:

- **View Source** = the original downloaded HTML
- **Elements tab (DevTools)** = the current DOM (after JS changes)

---

## 3) Variables: `const` and `let`

Prefer:

- `const` for variables you won’t reassign
- `let` for variables you will reassign

Avoid `var` in modern code.

```js
const name = 'Asha';
let score = 0;

score = score + 1;
```

Important: `const` does **not** mean “immutable”. It means “cannot be reassigned”.

```js
const user = { name: 'Asha' };
user.name = 'Asha Singh'; // allowed
// user = {}              // NOT allowed
```

---

## 4) Types (the ones you’ll actually see)

Primitives:

- `string`, `number`, `boolean`, `null`, `undefined`, `bigint`, `symbol`

Objects:

- `object` (includes arrays, functions, plain objects, dates, etc.)

Useful checks:

```js
typeof 'hi'      // 'string'
typeof 123       // 'number'
typeof true      // 'boolean'
typeof undefined // 'undefined'
typeof null      // 'object'  <-- historical weirdness

Array.isArray([1, 2, 3]) // true
```

---

## 5) Equality: `===` vs `==`

Use **strict equality** almost always:

```js
5 === '5' // false
5 == '5'  // true  (type coercion)
```

Rule: prefer `===` and `!==`.

---

## 6) Functions (3 modern patterns)

### Function declaration

```js
function add(a, b) {
	return a + b;
}
```

### Function expression

```js
const add = function (a, b) {
	return a + b;
};
```

### Arrow function (very common)

```js
const add = (a, b) => a + b;
```

Arrow function note: arrow functions do **not** have their own `this`. That’s often good, but sometimes surprising.

---

## 7) Objects and arrays (daily bread)

### Objects

```js
const item = {
	id: 1,
	name: 'Notebook',
	price: 4.5
};

console.log(item.name);
console.log(item['price']);
```

### Arrays

```js
const items = ['pen', 'pencil', 'marker'];
items.push('eraser');

for (const x of items) {
	console.log(x);
}
```

---

## 8) Destructuring + spread/rest (modern convenience)

### Destructuring

```js
const user = { name: 'Asha', email: 'asha@example.com' };
const { name, email } = user;

const nums = [10, 20, 30];
const [first, second] = nums;
```

### Spread (`...`) to copy/merge

```js
const a = [1, 2];
const b = [...a, 3]; // [1,2,3]

const original = { x: 1, y: 2 };
const updated = { ...original, y: 99 };
```

### Rest (`...`) to collect

```js
function sum(...nums) {
	return nums.reduce((acc, n) => acc + n, 0);
}
```

---

## 9) Strings: template literals

```js
const name = 'Asha';
const msg = `Hello, ${name}!`;
```

Backticks (`` ` ``) are also great for multi-line strings.

---

## 10) Nullish coalescing + optional chaining

These reduce annoying “cannot read property of undefined” errors.

### Optional chaining `?.`

```js
const city = user.address?.city; // undefined if address missing
```

### Nullish coalescing `??`

```js
const pageSize = settings.pageSize ?? 20;
```

`??` only falls back on `null` or `undefined` (not on `0` or empty string).

---

## 11) Loops you should actually use

### `for...of` for arrays

```js
for (const item of items) {
	console.log(item);
}
```

### Array methods (very common)

```js
const prices = [4.5, 1.25, 2.75];

const doubled = prices.map((p) => p * 2);
const expensive = prices.filter((p) => p > 2);
const total = prices.reduce((sum, p) => sum + p, 0);
```

---

## 12) DOM basics (the browser’s page tree)

The **DOM** is the live tree of elements the browser built from your HTML.

Common tasks:

### Select an element

```js
const title = document.querySelector('h1');
const button = document.querySelector('#saveBtn');
const cards = document.querySelectorAll('.card');
```

### Read/write text and HTML

```js
title.textContent = 'New Title';
// Avoid using innerHTML unless you really need it
```

### Change styles (okay for demos)

```js
title.style.color = 'rebeccapurple';
```

Prefer toggling classes for real apps:

```js
title.classList.add('highlight');
title.classList.toggle('hidden');
```

---

## 13) Events: clicks, typing, submit

JavaScript in the browser is **event-driven**.

### Click

```js
const btn = document.querySelector('#increment');
const output = document.querySelector('#count');

let count = 0;
btn.addEventListener('click', () => {
	count += 1;
	output.textContent = String(count);
});
```

### Form submit

```js
const form = document.querySelector('form');

form.addEventListener('submit', (event) => {
	event.preventDefault(); // stop page reload
	const data = new FormData(form);
	console.log('email:', data.get('email'));
});
```

---

## 14) Loading order: don’t run JS before the DOM exists

If your script runs before the element exists, `querySelector` returns `null`.

Two good fixes:

### Fix A: `defer`

```html
<script src="app.js" defer></script>
```

### Fix B: place scripts at end of body

```html
<body>
	<!-- content -->
	<script src="app.js"></script>
</body>
```

---

## 15) Modules (modern way to organize JS)

Modules let you split code into files using `import` and `export`.

In HTML:

```html
<script type="module" src="main.js"></script>
```

In `math.js`:

```js
export function add(a, b) {
	return a + b;
}
```

In `main.js`:

```js
import { add } from './math.js';

console.log(add(2, 3));
```

Module note: module scripts run in strict mode and have their own scope.

---

## 16) Async JavaScript: Promises + `async/await`

Many browser operations take time (network calls, timers). JavaScript uses **Promises**.

### The modern default: `async/await`

```js
async function loadItems() {
	const res = await fetch('/items');
	const data = await res.json();
	console.log(data);
}
```

### Error handling

Two important rules:

1. `fetch()` only rejects on network errors.
2. HTTP error status codes like 404/500 do not throw automatically.

So do this:

```js
async function loadItems() {
	const res = await fetch('/items');

	if (!res.ok) {
		throw new Error(`Request failed: ${res.status} ${res.statusText}`);
	}

	return await res.json();
}

loadItems().catch((err) => console.error(err));
```

---

## 17) HTTP requests with `fetch`: GET and POST JSON

### GET

```js
const res = await fetch('http://localhost:3000/items');
const data = await res.json();
```

### POST JSON

```js
const res = await fetch('http://localhost:3000/items', {
	method: 'POST',
	headers: {
		'Content-Type': 'application/json',
		'Accept': 'application/json'
	},
	body: JSON.stringify({ name: 'Marker', price: 2.75 })
});

const created = await res.json();
console.log(created);
```

---

## 18) Debugging survival skills

### 1) Use the Console

```js
console.log('here');
console.table([{ a: 1 }, { a: 2 }]);
console.error('something broke');
```

### 2) Read error messages carefully

- “Cannot read properties of null” usually means your selector didn’t find an element.
- “Unexpected token < in JSON” usually means you tried to parse HTML as JSON (often a 404 page).

### 3) Use breakpoints

In DevTools → Sources:

- Click a line number to set a breakpoint
- Step through code
- Inspect variables

---

## 19) Common beginner pitfalls

### Pitfall: forgetting `return`

```js
const nums = [1, 2, 3];
const doubled = nums.map((n) => {
	return n * 2;
});
```

If you use `{}` in an arrow function body, you need `return`.

### Pitfall: treating strings like numbers

```js
Number('5') + 1 // 6
'5' + 1         // '51'
```

### Pitfall: `this` confusion

Rule for beginners: don’t overuse `this`. Prefer plain functions and objects.

### Pitfall: mutating arrays/objects accidentally

When in doubt, make copies with spread:

```js
const newItems = [...items, newItem];
const newUser = { ...user, name: 'New' };
```

---

## 20) Mini practice checklist

1. Select an element by id and change its `textContent`.
2. Add a click event that updates a counter.
3. Fetch JSON from an endpoint and render it into the page.
4. Split code into two modules with `export`/`import`.

---

## 21) Recommended references

- MDN JavaScript Guide: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide
- MDN DOM intro: https://developer.mozilla.org/en-US/docs/Web/API/Document_Object_Model/Introduction
- MDN fetch: https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API

