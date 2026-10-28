# 🌐 REST Client (VS Code Extension) — Beginner Cheatsheet

This guide helps you use the **REST Client** extension in VS Code to send HTTP requests from `.http` files.

Extension ID: `humao.rest-client`

---

## ✅ 1) Install the extension

In VS Code:

1. Open **Extensions** (`Ctrl+Shift+X`)
2. Search for **REST Client**
3. Install **REST Client** by **Huachao Mao**

---

## ✅ 2) Create your first request file

Create a file like:

- `requests.http`
- or any file ending with `.http` / `.rest`

Add:

```http
GET https://jsonplaceholder.typicode.com/todos/1
```

Then click **Send Request** above the request line.

---

## ✅ 3) Request format basics

General shape:

```http
METHOD URL HTTP/VERSION
Header-Name: value

{optional body}
```

Most of the time, version is optional in REST Client:

```http
GET https://api.example.com/users
```

---

## ✅ 4) Common request examples

### GET

```http
GET https://jsonplaceholder.typicode.com/posts/1
Accept: application/json
```

### POST (JSON body)

```http
POST https://jsonplaceholder.typicode.com/posts
Content-Type: application/json

{
  "title": "hello",
  "body": "from REST Client",
  "userId": 1
}
```

### PUT (replace)

```http
PUT https://jsonplaceholder.typicode.com/posts/1
Content-Type: application/json

{
  "id": 1,
  "title": "updated title",
  "body": "updated body",
  "userId": 1
}
```

### PATCH (partial update)

```http
PATCH https://jsonplaceholder.typicode.com/posts/1
Content-Type: application/json

{
  "title": "patched title"
}
```

### DELETE

```http
DELETE https://jsonplaceholder.typicode.com/posts/1
```

---

## ✅ 5) Separate multiple requests in one file

Use `###` between requests:

```http
GET https://jsonplaceholder.typicode.com/users
Accept: application/json

###

GET https://jsonplaceholder.typicode.com/users/1
Accept: application/json
```

---

## ✅ 6) Use variables (great for local/dev/prod)

### File variables

```http
@baseUrl = https://jsonplaceholder.typicode.com
@postId = 1

GET {{baseUrl}}/posts/{{postId}}
Accept: application/json
```

### Environment-style blocks

```http
@env = dev

###
# @name getTodo
GET https://jsonplaceholder.typicode.com/todos/1
Accept: application/json
```

> Tip: Start with simple `@baseUrl` variables first. They cover most beginner use cases.

---

## ✅ 7) Add authentication headers

### Bearer token

```http
GET https://api.example.com/me
Authorization: Bearer YOUR_TOKEN_HERE
Accept: application/json
```

### Basic auth

```http
GET https://api.example.com/admin
Authorization: Basic YOUR_BASE64_VALUE
```

---

## ✅ 8) Reuse response values in later requests

Name a request with `# @name`:

```http
# @name createUser
POST https://jsonplaceholder.typicode.com/users
Content-Type: application/json

{
  "name": "Asha"
}
```

Use returned fields:

```http
GET https://jsonplaceholder.typicode.com/users/{{createUser.response.body.$.id}}
Accept: application/json
```

---

## ✅ 9) Troubleshooting quick checks

- **No "Send Request" link?**
  - Ensure file ends with `.http` or `.rest`
  - Ensure REST Client extension is installed and enabled

- **415 Unsupported Media Type**
  - Add correct `Content-Type` header (often `application/json`)

- **401 Unauthorized / 403 Forbidden**
  - Check `Authorization` header and token expiry

- **DNS/Network errors**
  - Verify URL and internet/VPN access

- **Invalid JSON body**
  - Check commas, quotes, and braces

---

## ✅ 10) Safe credential handling (important)

- Do **not** commit real secrets/tokens in `.http` files
- Use placeholder values in shared files
- Prefer local environment/secret storage for real credentials
- Add personal request files to `.gitignore` if needed

Example:

```http
Authorization: Bearer {{apiToken}}
```

---

## ⚡ Quick workflow summary

1. Create `requests.http`
2. Add a `GET` request
3. Click **Send Request**
4. Add headers/body for `POST`, `PUT`, `PATCH`
5. Add `@baseUrl` and reuse variables
6. Keep credentials out of source control

