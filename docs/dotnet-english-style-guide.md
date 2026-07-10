# .NET English Style for Documentation and Exceptions

Use this guide for English C# XML documentation comments and exception messages. It summarizes Microsoft guidance plus a few explicit project conventions.

## General rules

- Write complete sentences in summaries, remarks, exception conditions, and exception messages. Prefer one fact per sentence.
- Use sentence-style capitalization. End sentences and conventional documentation phrases with a period.
- Use present tense and active voice where natural: `Returns ...`, not `... is returned`.
- State observable behavior or the exact requirement; omit irrelevant implementation details.
- Prefer a concrete condition to a vague judgment: `The matrix must be square.`, not `The matrix is invalid.`
- Use one term consistently for each concept. Preserve the official casing of identifiers and products.
- Keep the tone neutral. Do not use rhetorical questions, exclamation marks, apologies, or emotional wording.

## XML documentation comments

Document the API contract: purpose, inputs, output, and exceptional conditions.

### Standard openings

| Target | Pattern |
| --- | --- |
| Type | `Represents ...` |
| Constructor | `Initializes a new instance of ...` |
| Method | A third-person verb such as `Adds ...`, `Computes ...`, `Creates ...`, `Returns ...`, or `Searches ...` |
| Property | `Gets ...` or `Gets or sets ...` |
| Boolean method | `Determines whether ...`, `Indicates whether ...`, or `Returns a value indicating whether ...` |
| Boolean property | `Gets a value indicating whether ...` |
| Overridable member | `When overridden in a derived class, ...` |

- Keep `<summary>` concise, normally one sentence. Put supplemental details in `<remarks>`.
- Start method summaries with a third-person singular verb: `Computes`, not `Compute`. Choose the verb that names the actual action.
- Use concise noun phrases for `<param>`, `<typeparam>`, `<returns>`, and `<value>`; this is normal BCL style. For Boolean results, use `<see langword="true"/> if ...; otherwise, <see langword="false"/>.`
- In `<exception>`, state only the condition. Do not write `Thrown when ...` or repeat the exception type.
- Use semantic XML references instead of plain text:
  - `<paramref name="matrix"/>` for a parameter.
  - `<typeparamref name="T"/>` for a type parameter.
  - `<see cref="Matrix"/>` for a type or member.
  - `<see langword="null"/>` for a language keyword.
  - `<c>0</c>` for a literal or short code expression.

```csharp
/// <summary>
/// Computes the inverse of the specified matrix.
/// </summary>
/// <param name="matrix">The matrix to invert.</param>
/// <returns>The inverse of <paramref name="matrix"/>.</returns>
/// <exception cref="ArgumentException">
/// <paramref name="matrix"/> is not square.
/// </exception>
```

## Exception messages

Explain the particular failure, not the API contract in general.

- Write a complete sentence with ending punctuation.
- State what is wrong and, when not obvious, what is required: `The destination is too short. Provide a destination with at least 16 elements.`
- For argument exceptions, pass the parameter name separately with `nameof(...)`; do not repeat it merely to identify the argument.
- Do not repeat the exception class name or begin with `Error:` or `Exception:`.
- Do not include secrets, credentials, sensitive data, or unnecessary internal details.
- Localize messages when the application requires localization.
- Common .NET convention: enclose inserted values or names in single quotation marks: `The storage format 'csr' is not supported.` This is not a universal rule.

```csharp
throw new ArgumentOutOfRangeException(
    nameof(rank),
    "The rank must be greater than zero.");
```

## Preferred condition patterns

| Situation | Pattern |
| --- | --- |
| Requirement | `The ... must ...` |
| Prohibited value | `The ... cannot ...` |
| Range | `The ... must be in the range ...` |
| Size mismatch | `The number of ... must match ...` |
| Empty input | `The ... contains no elements.` |
| Unsupported case | `The operation is not supported for ...` |
| Invalid state | `The ... is ... and cannot ...` |

## Compatibility note

The BCL contains legacy inconsistencies, including sentence fragments such as `Non-negative number required.`, `Thrown when ...` exception descriptions, missing punctuation, and less-common openings such as `Instantiates ...`. Do not copy an isolated legacy string; use the consistent patterns above for new text.

## Sources

- [Recommended XML tags for C# documentation comments](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/recommended-tags)
- [Best practices for exceptions](https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)
- [Microsoft Writing Style Guide](https://learn.microsoft.com/en-us/style-guide/welcome/)
- [.NET API documentation source](https://github.com/dotnet/dotnet-api-docs/tree/main/xml)
- [.NET runtime library source](https://github.com/dotnet/runtime/tree/main/src/libraries)
- [CoreLib resource strings](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/Resources/Strings.resx)
