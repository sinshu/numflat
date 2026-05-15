using System;
using System.Linq;
using System.Text.Json;
using NumFlat;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class VectorMatrixTests
    {
        [Test]
        public void DeserializeVector()
        {
            var options = NumFlatJsonSerializerOptions.Create();

            var vector = JsonSerializer.Deserialize<Vec<int>>("[1,2,3]", options);

            Assert.That(vector.Count, Is.EqualTo(3));
            Assert.That(vector.Stride, Is.EqualTo(1));
            Assert.That(vector.ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void DeserializeEmptyVector()
        {
            var options = NumFlatJsonSerializerOptions.Create();

            var vector = JsonSerializer.Deserialize<Vec<int>>("[]", options);

            Assert.That(vector.IsEmpty, Is.True);
            Assert.That(vector.Count, Is.EqualTo(0));
        }

        [Test]
        public void DeserializeMatrix()
        {
            var options = NumFlatJsonSerializerOptions.Create();

            var matrix = JsonSerializer.Deserialize<Mat<double>>("[[1,2,3],[4,5,6]]", options);

            Assert.That(matrix.RowCount, Is.EqualTo(2));
            Assert.That(matrix.ColCount, Is.EqualTo(3));
            Assert.That(matrix.Stride, Is.EqualTo(2));
            Assert.That(matrix[0, 0], Is.EqualTo(1));
            Assert.That(matrix[0, 1], Is.EqualTo(2));
            Assert.That(matrix[0, 2], Is.EqualTo(3));
            Assert.That(matrix[1, 0], Is.EqualTo(4));
            Assert.That(matrix[1, 1], Is.EqualTo(5));
            Assert.That(matrix[1, 2], Is.EqualTo(6));
        }

        [Test]
        public void DeserializeEmptyMatrix()
        {
            var options = NumFlatJsonSerializerOptions.Create();

            var matrix = JsonSerializer.Deserialize<Mat<double>>("[]", options);

            Assert.That(matrix.IsEmpty, Is.True);
            Assert.That(matrix.RowCount, Is.EqualTo(0));
            Assert.That(matrix.ColCount, Is.EqualTo(0));
        }

        [Test]
        public void RoundTripVectorKeepsArrayShape()
        {
            var options = new JsonSerializerOptions().AddNumFlatConverters();
            var source = new Vec<int>(3, 2, new[] { 1, -1, 2, -1, 3 });

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<Vec<int>>(json, options);

            Assert.That(json, Is.EqualTo("[1,2,3]"));
            Assert.That(actual.ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void RoundTripEmptyVectorKeepsArrayShape()
        {
            var options = new JsonSerializerOptions().AddNumFlatConverters();
            var source = default(Vec<int>);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<Vec<int>>(json, options);

            Assert.That(json, Is.EqualTo("[]"));
            Assert.That(actual.IsEmpty, Is.True);
            Assert.That(actual.Count, Is.EqualTo(0));
        }

        [Test]
        public void RoundTripMatrixKeepsArrayOfRowsShape()
        {
            var options = new JsonSerializerOptions().AddNumFlatConverters();
            var source = new Mat<int>(2, 3, 4, new[] { 1, 4, -1, -1, 2, 5, -1, -1, 3, 6 });

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<Mat<int>>(json, options);

            Assert.That(json, Is.EqualTo("[[1,2,3],[4,5,6]]"));
            Assert.That(actual.RowCount, Is.EqualTo(2));
            Assert.That(actual.ColCount, Is.EqualTo(3));
            Assert.That(actual[0, 0], Is.EqualTo(1));
            Assert.That(actual[0, 1], Is.EqualTo(2));
            Assert.That(actual[0, 2], Is.EqualTo(3));
            Assert.That(actual[1, 0], Is.EqualTo(4));
            Assert.That(actual[1, 1], Is.EqualTo(5));
            Assert.That(actual[1, 2], Is.EqualTo(6));
        }

        [Test]
        public void RoundTripEmptyMatrixKeepsArrayOfRowsShape()
        {
            var options = new JsonSerializerOptions().AddNumFlatConverters();
            var source = default(Mat<int>);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<Mat<int>>(json, options);

            Assert.That(json, Is.EqualTo("[]"));
            Assert.That(actual.IsEmpty, Is.True);
            Assert.That(actual.RowCount, Is.EqualTo(0));
            Assert.That(actual.ColCount, Is.EqualTo(0));
        }

        [Test]
        public void DeserializeInvalidMatrixThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();

            Assert.That((Action)(() => JsonSerializer.Deserialize<Mat<int>>("[[]]", options)), Throws.TypeOf<JsonException>());
            Assert.That((Action)(() => JsonSerializer.Deserialize<Mat<int>>("[[1,2],[3]]", options)), Throws.TypeOf<JsonException>());
        }
    }
}
