
# CSS Survival Guide 

CSS (Cascading Style Sheets) controls **how HTML looks**: colors, fonts, spacing, layout, and responsive behavior.

This guide teaches the minimum you need to be productive:

- How to attach CSS to HTML
- Selectors (how to target elements)
- The cascade + specificity (why a style “doesn’t work”)
- The box model (spacing and sizing)
- Layout with Flexbox + Grid
- Responsive design (mobile-friendly)
- Debugging in DevTools + common mistakes

---

## 1) What CSS is (and what it isn’t)

- **HTML** defines structure/content.
- **CSS** styles that structure.
- **JavaScript** changes behavior and can update styles/DOM.

CSS is made of **rules**:

```css
selector {
	property: value;
	property: value;
}
```

Example:

```css
h1 {
	color: rebeccapurple;
	font-size: 40px;
}
```

---

## 2) How to add CSS to a page

### Option A (best): external stylesheet

In HTML:

```html
<link rel="stylesheet" href="styles.css" />
```

In `styles.css`:

```css
body {
	font-family: system-ui, Segoe UI, Arial, sans-serif;
}
```

### Option B: `<style>` tag (fine for demos)

```html
<style>
	body { font-family: system-ui; }
</style>
```

### Option C: inline styles (avoid for real projects)

```html
<p style="color: red;">Hello</p>
```

Inline styles are hard to maintain and have high priority.

---

## 3) Selectors: how you choose *what* to style

Selectors are the #1 skill in CSS.

### Common selectors

**Element selector** (all `<p>`):

```css
p { line-height: 1.5; }
```

**Class selector** (recommended for most styling):

```css
.card { border: 1px solid #ddd; padding: 16px; }
```

Use in HTML:

```html
<div class="card">...</div>
```

**ID selector** (use sparingly):

```css
#mainTitle { color: teal; }
```

**Descendant selector** (any `.price` inside `.product`):

```css
.product .price { font-weight: 700; }
```

**Child selector** (only direct children):

```css
nav > a { margin-right: 12px; }
```

**Multiple selectors**:

```css
h1, h2 { font-family: Georgia, serif; }
```

**Pseudo-classes** (states like hover/focus):

```css
button:hover { background: #111; }
button:focus { outline: 3px solid #60a5fa; }
```

---

## 4) The Cascade: why CSS is called “Cascading”

If multiple rules apply to the same element/property, the browser decides which wins.

The most important factors:

1. **Importance** (`!important` wins… but avoid it)
2. **Specificity** (more specific selectors win)
3. **Order** (later rules win when specificity ties)

### Specificity (beginner mental model)

- Inline style: strongest (avoid)
- ID selectors: strong
- Class selectors: medium
- Element selectors: weak

Example:

```css
p { color: blue; }
.warning { color: orange; }
#urgent { color: red; }
```

HTML:

```html
<p class="warning" id="urgent">Hello</p>
```

This will be red because `#urgent` is more specific.

### Debugging cascade quickly

Use DevTools → Elements → Styles panel:

- You can see which rule applied
- You can see crossed-out rules that lost

---

## 5) The Box Model (spacing + sizing)

Every element is a box:

- **Content**: the actual text/image
- **Padding**: space inside the border
- **Border**: outline
- **Margin**: space outside the border

```css
.box {
	margin: 20px;
	border: 2px solid #333;
	padding: 16px;
}
```

### One of the most important lines in CSS

Put this near the top of your stylesheet:

```css
*, *::before, *::after {
	box-sizing: border-box;
}
```

It makes width/height behave in a more predictable way.

---

## 6) Common properties you’ll use constantly

### Text & fonts

```css
body {
	font-family: system-ui, Segoe UI, Arial, sans-serif;
	font-size: 16px;
	line-height: 1.5;
	color: #111;
}

h1 { font-size: 32px; margin: 0 0 12px; }
```

### Color

```css
.muted { color: #6b7280; }
.brand { color: #2563eb; }
```

### Backgrounds

```css
.panel {
	background: #f3f4f6;
	border-radius: 12px;
}
```

### Spacing (margin/padding shorthand)

```css
.card { padding: 16px; }
.card { padding: 8px 12px; }      /* top/bottom, left/right */
.card { margin: 0 auto; }         /* center block with known width */
```

---

## 7) Layout basics: display and positioning

### `display`

- `block`: takes full width (e.g., `div`, `p`)
- `inline`: flows with text (e.g., `span`, `a`)
- `inline-block`: like inline, but supports width/height
- `flex`: flexible row/column layouts
- `grid`: 2D layouts (rows and columns)

### Avoid old layout hacks

- Don’t use tables for layout.
- Don’t rely on lots of absolute positioning.
- Prefer Flexbox/Grid.

---

## 8) Flexbox: the easiest modern layout tool

Flexbox is for laying out items in a **row or column**.

```css
.row {
	display: flex;
	gap: 12px;
	align-items: center;
	justify-content: space-between;
}
```

Key properties:

- On the container:
	- `display: flex;`
	- `flex-direction: row | column`
	- `justify-content` (main axis alignment)
	- `align-items` (cross axis alignment)
	- `gap` (spacing between items)
- On children:
	- `flex: 1;` (grow to fill)

Example: two columns with a sidebar:

```css
.layout {
	display: flex;
	gap: 16px;
}

.sidebar { width: 240px; }
.content { flex: 1; }
```

---

## 9) Grid: clean two-dimensional layouts

Grid is great when you think “rows and columns”.

Example: responsive card grid:

```css
.grid {
	display: grid;
	grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
	gap: 16px;
}

.card {
	border: 1px solid #e5e7eb;
	border-radius: 12px;
	padding: 16px;
}
```

---

## 10) Responsive design (make it work on phones)

### Use flexible units

- Prefer `%`, `rem`, `vw`, and `minmax()` over hard-coded pixels everywhere.
- Use `max-width` to prevent lines from becoming too long.

```css
main {
	max-width: 900px;
	margin: 0 auto;
	padding: 16px;
}
```

### Media queries

```css
@media (max-width: 600px) {
	.layout {
		flex-direction: column;
	}

	.sidebar {
		width: auto;
	}
}
```

---

## 11) CSS variables (optional but super useful)

Variables help keep design consistent:

```css
:root {
	--text: #111827;
	--muted: #6b7280;
	--brand: #2563eb;
	--border: #e5e7eb;
}

body { color: var(--text); }
.muted { color: var(--muted); }
.btn { background: var(--brand); }
```

---

## 12) Debugging survival skills (DevTools)

Open DevTools (Inspect) and use:

- **Elements tab**: click an element to see which CSS rules apply
- **Styles panel**: shows your rules; crossed-out rules lost the cascade
- **Computed panel**: final computed values (great for debugging sizes)

Fast checklist when “CSS doesn’t work”:

1. Did you link the stylesheet correctly?
2. Is your selector actually matching the element?
3. Is another rule overriding it (specificity/order)?
4. Is the property valid for that element?
5. Are you fighting browser defaults (margin on `body`, `h1`, etc.)?

---

## 13) Common beginner mistakes (and fixes)

### Mistake: forgetting units

```css
/* Wrong */
margin: 10;

/* Right */
margin: 10px;
```

### Mistake: styling with IDs everywhere

IDs make overrides harder. Prefer classes:

```css
.title { color: #2563eb; }
```

### Mistake: overusing `!important`

If you need `!important`, it often means your selector strategy/cascade is messy.

### Mistake: centering confusion

To center text:

```css
.centerText { text-align: center; }
```

To center a block element:

```css
.box {
	width: 300px;
	margin: 0 auto;
}
```

To center items in a container:

```css
.center {
	display: flex;
	align-items: center;
	justify-content: center;
}
```

---

## 14) Mini starter stylesheet (copy/paste)

```css
/* 1) Predictable sizing */
*, *::before, *::after {
	box-sizing: border-box;
}

/* 2) Remove some awkward defaults */
body {
	margin: 0;
	font-family: system-ui, Segoe UI, Arial, sans-serif;
	line-height: 1.5;
	color: #111;
}

/* 3) Media elements behave nicely */
img {
	max-width: 100%;
	height: auto;
	display: block;
}

/* 4) A simple content container */
.container {
	max-width: 900px;
	margin: 0 auto;
	padding: 16px;
}
```

---

## 15) Practice challenges

1. Make a “card” class with padding, border, and border-radius.
2. Make a navigation bar using Flexbox.
3. Create a 3-column layout on desktop that becomes 1 column on mobile.
4. Use DevTools to find out why a rule is being overridden.

