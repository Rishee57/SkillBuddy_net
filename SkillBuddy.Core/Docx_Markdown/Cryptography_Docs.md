# Cryptography.cs – Full Documentation

## Overview
The `Cryptography` class provides **AES encryption and decryption** utilities for secure string handling.  
It supports **two usage models**:

1. **Static API** — uses a global encryption key + salt.  
   ```csharp
   Cryptography.Encrypt("my text");
   Cryptography.Decrypt(cipherText);
   ```

2. **Instance API (preferred)** — each instance of `Cryptography` uses its own key + salt.  
   ```csharp
   var crypto = new Cryptography(myKey, mySalt);
   var cipher = crypto.Encrypt("secret");
   var plain = crypto.Decrypt(cipher);
   ```

---

## ⚙️ Class Breakdown

### 🔹 Fields
```csharp
private static string ENCRYPTION_KEY = string.Empty;
private static byte[] SALT = Array.Empty<byte>();
```
- **Static global values** used by static methods.
- Updated automatically when you create a new instance via constructor.

```csharp
public string EncKey { get; }
public byte[] EncSalt { get; }
```
- **Instance values**, bound at object creation.
- Immutable (`get;` only).

### 🔹 Constructor
```csharp
public Cryptography(string encryptionKey, byte[] salt)
```
- Requires a **non-empty key** and a **non-null salt**.  
- Initializes instance fields (`EncKey`, `EncSalt`) and updates static values (`ENCRYPTION_KEY`, `SALT`).  

### 🔹 Static API
```csharp
public static string Encrypt(string inputText)
public static string Decrypt(string inputText)
```
- Use **global key/salt** (`ENCRYPTION_KEY` and `SALT`).  

### 🔹 Internal Helper Methods
```csharp
internal static string EncryptInternal(string key, byte[] salt, string inputText)
internal static string DecryptInternal(string key, byte[] salt, string inputText)
```
- Core AES operations using **PBKDF2 (Rfc2898DeriveBytes)** with SHA256, 100,000 iterations.

### 🔹 Extension Class
```csharp
public static class CryptographyExtension
```
Adds **instance extension methods** for convenience:

```csharp
public static string Encrypt(this Cryptography cryptography, string inputText)
public static string Decrypt(this Cryptography cryptography, string inputText)
```

---

## 🔐 Security Details
- **AES** with 32-byte key + 16-byte IV.  
- **PBKDF2 with SHA256** and 100,000 iterations.  
- **Salt** must be random and unique per app.  
- Strings encoded in UTF-16 (`Encoding.Unicode`).  
- Ciphertext returned as Base64.

---

## ✅ Usage Examples

### Console App
```csharp
string key = "myStrongKey123!";
byte[] salt = Encoding.UTF8.GetBytes("mySaltValue");

var crypto = new Cryptography(key, salt);

string cipher = crypto.Encrypt("SkillBuddy");
string plain  = crypto.Decrypt(cipher);
```

### ASP.NET Core with Dependency Injection
```csharp
builder.Services.AddSingleton(sp =>
{
    var key = Environment.GetEnvironmentVariable("ENCRYPTION_KEY");
    var salt = Convert.FromBase64String(Environment.GetEnvironmentVariable("ENCRYPTION_SALT"));
    return new Cryptography(key, salt);
});
```

---

## ⚠️ Best Practices
1. **Never hardcode keys/salts**.  
2. **Use long, random salts** (≥16 bytes).  
3. **Do not log keys/salts**.  
4. **Prefer instance API**.  
5. **Use secret managers / env vars** for production deployments.
