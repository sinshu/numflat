# NumFlat.Serialization.Json

This package provides JSON serialization and deserialization support for NumFlat’s main types.

## Example

### Code

```cs
using NumFlat;
using NumFlat.Serialization.Json;

Mat<double> x =
[
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9],
];

var options = new JsonSerializerOptions().AddNumFlatConverters();
var json = JsonSerializer.Serialize(x, options);

Console.WriteLine(json);

var deserialized = JsonSerializer.Deserialize<Mat<double>>(json, options);
```

#### Output

```console
[[1,2,3],[4,5,6],[7,8,9]]
```