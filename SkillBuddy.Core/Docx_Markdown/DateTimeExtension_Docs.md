# DateTimeExtension.cs – Full Documentation

## Overview
The `DateTimeExtension` class provides extension methods for working with dates, Unix time, object emptiness checks, and deep cloning.  
It is defined as a static class in the `SkillBuddy.Core` namespace.

---

## ⚙️ Methods

### 🔹 ToDateTime
```csharp
public static DateTime ToDateTime(this long epoch)
```
- Converts Unix epoch seconds (since 1970-01-01) to UTC `DateTime`.
- Uses `DateTime.UnixEpoch` for clarity.
- Always returns UTC time.

**Example:**
```csharp
long epoch = 1700000000;
DateTime dt = epoch.ToDateTime();
```

---

### 🔹 ToUnixTimeSeconds
```csharp
public static double ToUnixTimeSeconds(this DateTime dateTime)
```
- Converts a `DateTime` to Unix epoch seconds.
- Ensures the input is normalized to UTC before conversion.

**Example:**
```csharp
DateTime now = DateTime.UtcNow;
double epoch = now.ToUnixTimeSeconds();
```

---

### 🔹 IsEmpty
```csharp
public static bool IsEmpty(this object? obj)
```
- Checks if an object is "empty".
- Handles multiple types:
  - `null` → empty  
  - `string` → empty if `null` or `""`  
  - `Array` → empty if length is 0  
  - `DateTime` → empty if default value  
  - `Value types` (e.g. int, long) → empty if equal to default  
  - `Class objects` → considered empty if all public properties are empty

**Example:**
```csharp
string? text = "";
bool empty = text.IsEmpty(); // true

int number = 0;
bool emptyNumber = number.IsEmpty(); // true

DateTime date = default;
bool emptyDate = date.IsEmpty(); // true
```

---

### 🔹 Clone
```csharp
public static T? Clone<T>(this T source)
```
- Creates a deep copy of an object using **JSON serialization**.
- Uses `Newtonsoft.Json` with `ObjectCreationHandling.Replace` to avoid default constructor values being added back.
- Private members are **not cloned**.

**Example:**
```csharp
var person = new Person { Name = "Alice", Age = 30 };
var copy = person.Clone();
```

---

## 🔐 Notes on Security & Performance
- **Clone<T>** is convenient but not the fastest for performance-critical applications. For high-speed scenarios, implement `ICloneable` manually.  
- **IsEmpty** uses reflection for class properties → avoid in hot paths.

---

## ✅ Usage Examples
```csharp
long epoch = 1700000000;
DateTime date = epoch.ToDateTime(); // Convert epoch → DateTime

DateTime now = DateTime.UtcNow;
double epochValue = now.ToUnixTimeSeconds(); // DateTime → epoch

string text = "";
bool check = text.IsEmpty(); // true

var obj = new { Id = 1, Name = "SkillBuddy" };
var cloned = obj.Clone(); // deep copy
```

---

## ⚠️ Best Practices
1. Always ensure `DateTime` is in UTC before using `ToUnixTimeSeconds`.  
2. Use `IsEmpty` carefully for large objects (reflection cost).  
3. Prefer strongly typed null/empty checks where possible instead of generic `IsEmpty`.  
4. Use `Clone` only when you need a **full deep copy** — for shallow copies, `MemberwiseClone()` or manual mapping is faster.  
