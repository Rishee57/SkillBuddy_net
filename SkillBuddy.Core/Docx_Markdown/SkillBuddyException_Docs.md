# SkillBuddy Exceptions – Full Documentation

## Overview
The SkillBuddy project defines a hierarchy of custom exceptions for clearer error handling.  
These exceptions inherit from a common base class `SkillBuddyException`.  
This provides a consistent way to catch and manage domain-specific errors across the application.

---

## ⚙️ Exception Classes

### 🔹 SkillBuddyException (Base)
```csharp
public class SkillBuddyException : Exception
```
- The base class for all SkillBuddy exceptions.
- Inherits from `System.Exception`.
- Provides standard constructors for message and inner exception.

**Use when:**  
You want to catch **any SkillBuddy-specific error** without handling each subclass separately.

---

### 🔹 ServiceProviderException
```csharp
public class ServiceProviderException : SkillBuddyException
```
- Thrown when there is an error with an **external service provider** integration.

**Example:**
```csharp
throw new ServiceProviderException("Payment provider failed to respond.");
```

---

### 🔹 MessageFormatException
```csharp
public class MessageFormatException : SkillBuddyException
```
- Used when an input message has **invalid format** or **validation errors**.
- Includes a property:
```csharp
public IDictionary<string, List<string>> ValidationErrors { get; }
```
- Allows capturing multiple validation errors per field.

**Example:**
```csharp
var errors = new Dictionary<string, List<string>>
{
    { "Email", new List<string>{ "Email is required", "Invalid format" } }
};

throw new MessageFormatException("Invalid message format.", errors);
```

---

### 🔹 UrlShortenerException
```csharp
public class UrlShortenerException : SkillBuddyException
```
- Raised when URL shortening fails (e.g., third-party API issue).

**Example:**
```csharp
throw new UrlShortenerException("TinyURL API returned error 500.");
```

---

### 🔹 CallbackException
```csharp
public class CallbackException : SkillBuddyException
```
- Used when a callback (e.g., webhook, async handler) fails to execute properly.

**Example:**
```csharp
throw new CallbackException("Webhook delivery failed due to timeout.");
```

---

### 🔹 MessageSendException
```csharp
public class MessageSendException : SkillBuddyException
```
- Raised when sending a message (SMS, Email, Push) fails.

**Example:**
```csharp
throw new MessageSendException("Failed to send email notification.");
```

---

## 🔐 Best Practices
1. **Catch specific exceptions** where possible (`MessageFormatException`, `ServiceProviderException`).  
2. Use **base `SkillBuddyException`** to handle all custom exceptions in a global handler.  
3. Populate `ValidationErrors` in `MessageFormatException` for detailed error reporting.  
4. Avoid catching `System.Exception` everywhere — prefer the SkillBuddy hierarchy.  
5. Keep exception messages clear and user/developer friendly.

---

## ✅ Usage Example

```csharp
try
{
    throw new MessageFormatException("Invalid input", new Dictionary<string, List<string>>
    {
        { "Username", new List<string>{ "Username cannot be empty" } }
    });
}
catch (MessageFormatException ex)
{
    Console.WriteLine(ex.Message);
    foreach (var kvp in ex.ValidationErrors)
    {
        Console.WriteLine($"{kvp.Key}: {string.Join(", ", kvp.Value)}");
    }
}
catch (SkillBuddyException ex)
{
    Console.WriteLine($"A SkillBuddy error occurred: {ex.Message}");
}
```

---

## ⚠️ Summary
- `SkillBuddyException` → Base for all custom errors.  
- `ServiceProviderException` → External service failures.  
- `MessageFormatException` → Input validation/formatting errors.  
- `UrlShortenerException` → URL shortening issues.  
- `CallbackException` → Webhook or async callback issues.  
- `MessageSendException` → Message delivery failures.  
