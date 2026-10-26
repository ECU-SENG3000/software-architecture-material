
# HTML Survival Guide 

- What HTML is (and what it is not)
- How a web page loads
- Why the downloaded HTML page is an **initializer**
- What the **DOM** is and why it matters
- Enough tags, patterns, and best practices to build real pages

---

## 1) What HTML is

**HTML (HyperText Markup Language)** is the language used to describe the **structure** of a web page.

Think of HTML like the *blueprint* of a building:

- It describes **what things are** (a heading, a paragraph, a button, a list, a form)
- It does *not* describe advanced styling (that’s **CSS**)
- It does *not* describe behavior/logic (that’s **JavaScript**)

### The 3 core web languages

- **HTML** = structure and meaning (content)
- **CSS** = presentation (layout, colors, fonts)
- **JavaScript** = behavior (interactivity, logic, data fetching)

---

## 2) The “downloaded page is an initializer” idea

When you visit a site, your browser sends an HTTP request like:

```http
GET / HTTP/1.1
Host: example.com
```

The server often responds with an HTML document.

That HTML document is the **initial page** your browser uses to start the app. It’s best to think of it as an **initializer**:

- The HTML is downloaded once (or occasionally) as the initial document
- The browser **parses** the HTML into a live data structure (the DOM)
- The browser may download linked files next (CSS, images, JavaScript)
- JavaScript (if included) can then **change the DOM** after the initial download

In modern websites, the first HTML can be small and acts like “bootstrapping”:

- It sets up the initial structure
- It loads CSS
- It loads JS that may render more content or fetch data

So: **The HTML you download is not just a picture of the page**. It’s the starting instructions that the browser uses to build the page.

---

## 3) Your first complete HTML page

Create a file called `index.html` and type this:

```html
<!doctype html>
<html lang="en">
	<head>
		<meta charset="utf-8" />
		<meta name="viewport" content="width=device-width, initial-scale=1" />
		<title>My First Page</title>
	</head>
	<body>
		<h1>Hello, HTML</h1>
		<p>This is my first webpage.</p>
	</body>
</html>
```

### What those pieces mean

- `<!doctype html>`: tells the browser to use modern HTML rules.
- `<html lang="en">`: the root element. `lang` helps accessibility and translation.
- `<head>`: metadata and links (title, CSS, fonts, etc.). Not the visible page content.
- `<body>`: what you actually see in the page.

---

## 4) Tags, elements, and attributes

### Tags and elements

- **Tag**: the bracket part, like `<p>`
- **Element**: the whole thing, like `<p>Text</p>`

Most elements have an opening tag and closing tag:

```html
<p>This is a paragraph.</p>
```

Some are “void elements” (no closing tag):

```html
<img src="cat.jpg" alt="A cat" />
<br />
<input type="text" />
```

### Attributes

Attributes are extra info inside the opening tag:

```html
<a href="https://example.com" target="_blank">Visit</a>
```

- `href` is the link destination
- `target="_blank"` opens in a new tab

---

## 5) Common content elements you’ll use constantly

### Headings

Use headings to create a clear outline:

```html
<h1>Page Title</h1>
<h2>Section</h2>
<h3>Subsection</h3>
```

Tip: There should typically be **one** `<h1>` per page.

### Paragraphs and emphasis

```html
<p>This is a paragraph with <strong>important</strong> text and <em>emphasis</em>.</p>
```

- `<strong>` means “important” (not just bold)
- `<em>` means “emphasis” (not just italics)

### Lists

Unordered list:

```html
<ul>
	<li>Milk</li>
	<li>Eggs</li>
</ul>
```

Ordered list:

```html
<ol>
	<li>Step one</li>
	<li>Step two</li>
</ol>
```

### Links

```html
<a href="/about.html">About</a>
<a href="https://developer.mozilla.org/" target="_blank" rel="noreferrer">MDN</a>
```

`rel="noreferrer"` (or `noopener`) is recommended when using `target="_blank"`.

### Images

```html
<img src="images/demo.svg" alt="A colorful demo banner" width="320" height="120" />
```

Important:

- `alt` is required for accessibility (screen readers) and shows if the image fails to load.
- `width` and `height` help the browser reserve space to reduce layout shifting.

---

## 6) Page structure: semantic HTML

Semantic elements tell the browser (and accessibility tools) what a section *means*.

Common semantic layout:

```html
<header>
	<h1>Site Name</h1>
	<nav>
		<a href="/">Home</a>
		<a href="/contact">Contact</a>
	</nav>
</header>

<main>
	<article>
		<h2>Blog Post Title</h2>
		<p>Post content...</p>
	</article>
</main>

<footer>
	<small>© 2026</small>
</footer>
```

Try to use these instead of `<div>` everywhere.

### When to use `<div>` and `<span>`

- `<div>`: a generic block container
- `<span>`: a generic inline container

Use them when there isn’t a better semantic tag.

---

## 7) Forms (how users send input)

Forms let users input data and submit it.

```html
<form action="/signup" method="post">
	<label for="email">Email</label>
	<input id="email" name="email" type="email" required />

	<label for="password">Password</label>
	<input id="password" name="password" type="password" minlength="8" required />

	<button type="submit">Sign up</button>
</form>
```

Key ideas:

- `method="post"` usually sends data in the request body
- `name="..."` is the key used when sending form data
- `<label for="...">` improves usability and accessibility

---

## 8) The DOM: what the browser *builds* from your HTML

### What is the DOM?

**DOM** stands for **Document Object Model**.

When the browser downloads HTML, it doesn’t just display it as text. It parses it into a tree of objects.

- Each element becomes a **node** in a tree
- This tree is the **DOM**
- JavaScript can read and change this tree

Example HTML:

```html
<ul>
	<li>Milk</li>
	<li>Eggs</li>
</ul>
```

DOM idea (tree):

- `ul`
	- `li` ("Milk")
	- `li` ("Eggs")

### Why the DOM matters

- CSS styles the DOM
- Screen readers interpret the DOM
- JavaScript interacts with the DOM
- Clicking buttons, updating text, showing/hiding content — that’s usually JavaScript changing the DOM

### Example: changing the page with JavaScript

Put this at the bottom of `<body>` (or use `defer` in `<script>`):

```html
<p id="status">Loading...</p>

<script>
	const el = document.querySelector('#status');
	el.textContent = 'Loaded!';
</script>
```

Notice the “initializer” concept again:

- The downloaded HTML created a `<p>` in the DOM
- JavaScript then found that DOM node and changed its text

---

## 9) Loading order: why scripts sometimes “can’t find” elements

If JavaScript runs before the browser has created the element in the DOM, selectors return `null`.

Two easy fixes:

### Fix A: put `<script>` at the end of `<body>`

```html
<body>
	<!-- page content -->
	<script src="app.js"></script>
</body>
```

### Fix B: use `defer`

```html
<script src="app.js" defer></script>
```

`defer` tells the browser: download the script now, run it after HTML is parsed.

---

## 10) Debugging survival skills (DevTools)

In Chrome/Edge:

- Right click → **Inspect**
- **Elements tab**: see the live DOM (what the browser built)
- **Console tab**: run JavaScript and see errors
- **Network tab**: see requests for HTML, CSS, JS, images

Huge beginner insight:

- **“View Source”** shows the original downloaded HTML.
- **“Elements”** shows the current DOM after JavaScript has modified it.

They can be different!

---

## 11) Accessibility basics (don’t skip this)

Start with these habits:

- Use semantic tags (`<button>` for buttons, headings in order)
- Always include `alt` text on images
- Use `<label>` for every form input
- Don’t use click handlers on `<div>` when a `<button>` is appropriate

---

## 12) Mini practice checklist

1. Make a page with a title, one `<h1>`, two `<h2>` sections, and a list.
2. Add a link to another page.
3. Add an image with a good `alt`.
4. Add a simple form with one input and a submit button.
5. Use DevTools “Elements” tab to confirm the DOM tree matches what you wrote.

---

## 13) Common beginner mistakes

- Forgetting to close tags (or nesting them incorrectly)
- Using headings for styling instead of meaning
- Putting everything in `<div>`s
- Missing `alt` on images
- Writing JavaScript before the DOM exists (no `defer` / wrong script location)

---

## 14) Where to learn more

- MDN HTML basics: https://developer.mozilla.org/en-US/docs/Learn/Getting_started_with_the_web/HTML_basics
- HTML element reference: https://developer.mozilla.org/en-US/docs/Web/HTML/Element

