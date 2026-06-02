# NumFlat.Serialization.Json

This package provides JSON serialization and deserialization support for NumFlat’s main types.

## Supported vector and matrix element types

`Vec<T>` and `Mat<T>` converters are registered for `int`, `float`, `double`, and `System.Numerics.Complex`.

## Example

Use the source-generated metadata exposed by `NumFlatJsonSerializerContext` and pass its `JsonTypeInfo<T>` properties to `JsonSerializer`.

```cs
using System.Text.Json;
using NumFlat;
using NumFlat.Serialization.Json;

Mat<double> x =
[
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9],
];

var json = JsonSerializer.Serialize(x, NumFlatJsonSerializerContext.Default.MatrixDouble);

Console.WriteLine(json);

var deserialized = JsonSerializer.Deserialize(json, NumFlatJsonSerializerContext.Default.MatrixDouble);
```

Output:

```console
[[1,2,3],[4,5,6],[7,8,9]]
Matrix 3x3-Double
1  2  3
4  5  6
7  8  9
```

## Vector example

```cs
using System.Text.Json;
using NumFlat;
using NumFlat.Serialization.Json;

Vec<double> x = [1, 2, 3];

var json = JsonSerializer.Serialize(x, NumFlatJsonSerializerContext.Default.VectorDouble);
var deserialized = JsonSerializer.Deserialize(json, NumFlatJsonSerializerContext.Default.VectorDouble);
```
