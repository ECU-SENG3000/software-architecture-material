
# HTTP (Hypertext Transfer Protocol) — a practical, detailed guide

HTTP is the application-layer protocol that powers the web. It defines **how clients (browsers, apps, scripts) request resources** from **servers** and how servers respond.

At a high level:

1. A client figures out the server’s IP address (usually via DNS).
2. The client connects to the server (TCP for HTTP/1.1 and HTTP/2; QUIC/UDP for HTTP/3).
3. The client sends an **HTTP request message**.
4. The server returns an **HTTP response message**.
5. The connection may be reused for additional requests.

> Important: HTTP itself does **not** require encryption. When HTTP is sent over TLS encryption, we call it **HTTPS**.

---

## 1) Client/server model and “resources”

HTTP is a **request/response** protocol:

- **Client**: initiates requests (browser, `curl`, mobile app)
- **Server**: receives requests and returns responses (NGINX, Apache, Node, Python, etc.)

A server exposes **resources**, identified by URLs (e.g., `https://example.com/products/123`). A “resource” can be HTML, JSON, images, files, API endpoints, etc.

---

## 2) URLs and what the parts mean

Example URL:

`https://api.example.com:443/v1/users/42?include=posts#section`

Breakdown:

- **Scheme**: `https` (usually `http` or `https`)
- **Host**: `api.example.com`
- **Port**: `443` (default 443 for HTTPS, 80 for HTTP)
- **Path**: `/v1/users/42`
- **Query string**: `?include=posts` (key/value parameters)
- **Fragment**: `#section` (handled by the browser; not sent to the server in HTTP requests)

---

## 3) What happens before the first HTTP byte

### DNS lookup
If you request `https://example.com/`, the client typically resolves `example.com` to an IP address via DNS.

### Connection setup (transport)

- **HTTP/1.1** and **HTTP/2** usually run over **TCP**.
	- TCP handshake: SYN → SYN/ACK → ACK
- **HTTPS** adds **TLS handshake** on top of TCP (for HTTP/1.1 and HTTP/2).
- **HTTP/3** runs over **QUIC** (over UDP), integrating transport + security.

After the connection is established (and TLS if applicable), HTTP messages can flow.

---

## 4) HTTP message anatomy (requests and responses)

HTTP messages are plain text for HTTP/1.x (with binary framing in HTTP/2+). The *logical* structure stays the same:

### Request message (HTTP/1.1)

An HTTP/1.1 request contains:

1. **Start-line** (request line): method, target, version
2. **Headers**: metadata lines (`Name: value`)
3. Blank line
4. Optional **body** (payload)

Example (raw):

```http
GET /products?category=shoes&page=2 HTTP/1.1
Host: shop.example
User-Agent: Mozilla/5.0
Accept: text/html,application/xhtml+xml
Accept-Language: en-US,en;q=0.9
Accept-Encoding: gzip, br
Connection: keep-alive

```

Notes:

- The `Host` header is **required** in HTTP/1.1.
- Headers are separated from the body by a **blank line**.
- In HTTP/1.x, lines end with CRLF (`\r\n`).

### Response message (HTTP/1.1)

An HTTP/1.1 response contains:

1. **Status line**: version, status code, reason phrase
2. **Headers**
3. Blank line
4. Optional **body**

Example (raw):

```http
HTTP/1.1 200 OK
Date: Tue, 13 Jan 2026 20:15:10 GMT
Content-Type: text/html; charset=utf-8
Content-Length: 1256
Cache-Control: max-age=60

<!doctype html>
<html>...</html>
```

Notes:

- The **reason phrase** (`OK`) is mostly informational; programs rely on the status code.
- The body format is described by `Content-Type`.

---

## 5) Methods (verbs): what action the request is asking for

HTTP methods indicate the intent of the request.

### Common methods

- **GET**: Read a resource (fetch a page, get JSON data). Typically no body.
- **POST**: Submit data / create a subordinate resource (create a new item, login).
- **PUT**: Replace a resource completely at a known URL (full update).
- **PATCH**: Partially update a resource (partial update).
- **DELETE**: Remove a resource.
- **HEAD**: Same as GET but without the response body (useful to check metadata).
- **OPTIONS**: Ask the server what is allowed (often used for CORS “preflight”).

### Safety and idempotency (important for APIs)

- **Safe** methods don’t change server state: GET, HEAD, OPTIONS (and TRACE, rarely used).
- **Idempotent** means repeating the request has the same effect: GET, HEAD, PUT, DELETE, OPTIONS.
- POST is **not** idempotent in general (posting twice might create two records).

---

## 6) Status codes: what the server is telling you

Status codes are grouped by first digit:

- **1xx** Informational (rare in everyday web coding)
- **2xx** Success
- **3xx** Redirection
- **4xx** Client error (problem with the request)
- **5xx** Server error (server failed to fulfill a valid request)

Common examples:

- **200 OK**: success
- **201 Created**: new resource created (often from POST)
- **204 No Content**: success, no response body (common for DELETE)
- **301 Moved Permanently**: permanent redirect
- **302 Found**: temporary redirect (legacy semantics)
- **307 Temporary Redirect**: redirect, preserve method
- **308 Permanent Redirect**: redirect, preserve method
- **304 Not Modified**: cache validation success
- **400 Bad Request**: malformed request
- **401 Unauthorized**: missing/invalid authentication
- **403 Forbidden**: authenticated but not allowed
- **404 Not Found**: no resource at that URL
- **409 Conflict**: request conflicts with current server state
- **415 Unsupported Media Type**: unknown/unsupported `Content-Type`
- **429 Too Many Requests**: rate limited
- **500 Internal Server Error**: generic server failure
- **502 Bad Gateway**: upstream failure (common with proxies)
- **503 Service Unavailable**: overloaded or down for maintenance

---

## 7) Headers: the metadata that controls behavior

Headers are `Name: value` pairs.

There are hundreds of headers; you’ll see a small set constantly.

### Common request headers (with examples)

- **Host**: which virtual host you want (HTTP/1.1 requirement)
	- `Host: api.example.com`
- **User-Agent**: client identifier (browsers, tools)
	- `User-Agent: curl/8.5.0`
- **Accept**: which response media types you can handle
	- `Accept: application/json`
	- `Accept: text/html,application/xhtml+xml;q=0.9,*/*;q=0.8`
- **Accept-Language**: preferred languages
	- `Accept-Language: en-US,en;q=0.9`
- **Accept-Encoding**: supported compressions
	- `Accept-Encoding: gzip, br`
- **Content-Type**: the format of the request body (when you send one)
	- `Content-Type: application/json`
	- `Content-Type: application/x-www-form-urlencoded`
	- `Content-Type: multipart/form-data; boundary=----WebKitFormBoundary...`
- **Content-Length**: size of body in bytes (often automatically set)
	- `Content-Length: 68`
- **Authorization**: credentials/token for protected resources
	- `Authorization: Bearer <token>`
	- `Authorization: Basic <base64(username:password)>`
- **Cookie**: send stored cookies to the server
	- `Cookie: session_id=abc123; theme=dark`
- **Referer**: where the request came from (note spelling: “Referer”)
	- `Referer: https://shop.example/products`
- **Origin**: used by browsers for CORS
	- `Origin: https://app.example`
- **Cache-Control**: caching directives
	- `Cache-Control: no-cache`
- **If-None-Match**: conditional request using ETag
	- `If-None-Match: "686897696a7c876b7e"`
- **If-Modified-Since**: conditional request using timestamps
	- `If-Modified-Since: Tue, 13 Jan 2026 12:00:00 GMT`
- **Range**: request only part of a resource (useful for video/resume downloads)
	- `Range: bytes=0-999`

### Common response headers (with examples)

- **Content-Type**: format of the response body
	- `Content-Type: application/json; charset=utf-8`
- **Content-Length**: size of response body in bytes (when known)
	- `Content-Length: 239`
- **Content-Encoding**: compression used
	- `Content-Encoding: gzip`
- **Cache-Control**: caching rules
	- `Cache-Control: public, max-age=3600`
- **ETag**: version identifier for cache validation
	- `ETag: "686897696a7c876b7e"`
- **Last-Modified**: last modification timestamp
	- `Last-Modified: Tue, 13 Jan 2026 12:00:00 GMT`
- **Vary**: indicates which request headers affect the cached response
	- `Vary: Accept-Encoding`
- **Location**: redirect target or created resource URL
	- `Location: /login`
	- `Location: https://api.example.com/v1/users/43`
- **Set-Cookie**: instruct the client to store a cookie
	- `Set-Cookie: session_id=abc123; HttpOnly; Secure; SameSite=Lax; Path=/`
- **Date**: server timestamp for the response
	- `Date: Tue, 13 Jan 2026 20:15:10 GMT`
- **Server**: server software (often present but not required)
	- `Server: nginx`
- **Access-Control-Allow-Origin**: key CORS header for browsers
	- `Access-Control-Allow-Origin: https://app.example`

---

## 8) Bodies and content types (what’s inside the payload)

The body is optional. When it exists, its format is described by `Content-Type`.

Common body types:

- **HTML**: `text/html`
- **Plain text**: `text/plain`
- **JSON**: `application/json`
- **Form data (simple)**: `application/x-www-form-urlencoded`
- **Multipart form data** (file uploads): `multipart/form-data`
- **Binary files**: `application/octet-stream` or a specific type (`image/png`, `application/pdf`)

---

## 9) Examples: raw requests and responses

These examples show what a server might receive/send in HTTP/1.1.

### Example A: Basic GET (fetch HTML)

Request:

```http
GET / HTTP/1.1
Host: example.com
User-Agent: curl/8.5.0
Accept: text/html

```

Response:

```http
HTTP/1.1 200 OK
Content-Type: text/html; charset=utf-8
Content-Length: 42

<html><body>Hello</body></html>
```

### Example B: GET with query string (search)

Request:

```http
GET /search?q=cat%20videos&page=1 HTTP/1.1
Host: video.example
Accept: application/json

```

Response:

```http
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8

{"query":"cat videos","results":[],"page":1}
```

### Example C: POST JSON (create a resource)

Request:

```http
POST /v1/users HTTP/1.1
Host: api.example
Content-Type: application/json
Accept: application/json

{"name":"Asha","email":"asha@example.com"}
```

Response (created):

```http
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Location: /v1/users/43

{"id":43,"name":"Asha","email":"asha@example.com"}
```

### Example D: PUT vs PATCH (full vs partial update)

PUT (replace entire resource):

```http
PUT /v1/users/43 HTTP/1.1
Host: api.example
Content-Type: application/json

{"id":43,"name":"Asha Singh","email":"asha@example.com"}
```

PATCH (update only the provided fields):

```http
PATCH /v1/users/43 HTTP/1.1
Host: api.example
Content-Type: application/json

{"name":"Asha Singh"}
```

### Example E: DELETE (no response body)

Request:

```http
DELETE /v1/users/43 HTTP/1.1
Host: api.example

```

Response:

```http
HTTP/1.1 204 No Content
Date: Tue, 13 Jan 2026 20:15:10 GMT

```

### Example F: HTML form submission (`application/x-www-form-urlencoded`)

Request:

```http
POST /login HTTP/1.1
Host: app.example
Content-Type: application/x-www-form-urlencoded

username=asha&password=correct-horse-battery-staple
```

Response sets a cookie:

```http
HTTP/1.1 302 Found
Location: /dashboard
Set-Cookie: session_id=abc123; HttpOnly; Secure; SameSite=Lax; Path=/

```

### Example G: File upload (`multipart/form-data`)

Request (simplified):

```http
POST /upload HTTP/1.1
Host: files.example
Content-Type: multipart/form-data; boundary=----boundary

------boundary
Content-Disposition: form-data; name="description"

Profile photo
------boundary
Content-Disposition: form-data; name="file"; filename="me.png"
Content-Type: image/png

<binary bytes here>
------boundary--
```

### Example H: Authentication with Bearer token

Request:

```http
GET /v1/me HTTP/1.1
Host: api.example
Accept: application/json
Authorization: Bearer eyJhbGciOi...

```

Response (missing/invalid token):

```http
HTTP/1.1 401 Unauthorized
Content-Type: application/json

{"error":"invalid_token"}
```

---

## 10) Cookies and sessions (how “state” is often handled)

HTTP is **stateless**: each request can be processed independently.

To keep user state (like “logged in”), sites commonly use **cookies**:

- Server sends `Set-Cookie` in a response.
- Browser stores it.
- Browser automatically sends it back in future requests via `Cookie`.

Typical secure cookie flags:

- `HttpOnly`: JavaScript can’t read it (helps reduce cookie theft via XSS).
- `Secure`: only sent over HTTPS.
- `SameSite=Lax` or `SameSite=Strict`: helps protect against CSRF.

---

## 11) Caching basics (why some requests are fast)

Caching reduces network traffic and speeds up websites.

Two major styles:

### Expiration caching
Server tells the client how long it can reuse the response:

- `Cache-Control: max-age=3600` (cache for 1 hour)

### Validation caching (ETag / Last-Modified)
Client re-checks if the resource changed:

Request:

```http
GET /app.css HTTP/1.1
Host: static.example
If-None-Match: "686897696a7c876b7e"

```

Response (unchanged):

```http
HTTP/1.1 304 Not Modified
ETag: "686897696a7c876b7e"

```

No body is sent for `304`, saving bandwidth.

---

## 12) Redirects (3xx) and why they matter

Redirect responses include a `Location` header.

Example:

```http
HTTP/1.1 301 Moved Permanently
Location: https://www.example.com/

```

Redirect codes behave slightly differently:

- `301` / `302` can cause some clients to switch POST to GET (legacy behavior)
- `307` / `308` explicitly preserve the method and body on redirect

---

## 13) Connections, keep-alive, and performance

### HTTP/1.1

- Supports persistent connections (keep-alive) so multiple requests can reuse one TCP/TLS connection.
- Has “head-of-line blocking” at the application level if you pipeline poorly.

### HTTP/2

- Still usually runs over TCP + TLS.
- Uses **multiplexing**: many streams over one connection.
- Compresses headers (HPACK).

### HTTP/3

- Runs over **QUIC** (UDP-based).
- Reduces head-of-line blocking compared to TCP.

As a beginner, the big idea is: newer versions reduce latency and improve parallelism.

---

## 14) HTTPS in plain language (HTTP + encryption)

HTTPS = HTTP over TLS.

TLS provides:

- **Confidentiality**: others can’t read your traffic.
- **Integrity**: others can’t modify traffic unnoticed.
- **Authentication**: certificates help prove you’re talking to the right server.

Without HTTPS, attackers on the same network could read or alter HTTP requests/responses.

---

## 15) Quick “cheat sheet”

### Request checklist

- Method: GET / POST / PUT / PATCH / DELETE
- Target: `/path?query`
- Version: `HTTP/1.1`
- Key headers: `Host`, `Accept`, `Content-Type` (if body), `Authorization` (if needed)

### Response checklist

- Status: `200`, `404`, `500`, etc.
- Headers: `Content-Type`, caching headers, cookies, CORS, etc.
- Body: HTML/JSON/binary depending on content

---

If you want, I can also add a short section with `curl` commands that generate each example so students can reproduce them locally.

