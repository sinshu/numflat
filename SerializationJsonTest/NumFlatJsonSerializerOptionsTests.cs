using System.Linq;
using System.Numerics;
using System.Text.Json;
using NumFlat;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class NumFlatJsonSerializerOptionsTests
    {
        [Test]
        public void AddNumFlatConvertersDoesNotAddDuplicates()
        {
            var options = new JsonSerializerOptions();

            options.AddNumFlatConverters().AddNumFlatConverters();

            Assert.That(options.Converters.Count(converter => converter.CanConvert(typeof(Vec<int>))), Is.EqualTo(1));
            Assert.That(options.Converters.Count(converter => converter.CanConvert(typeof(Vec<float>))), Is.EqualTo(1));
            Assert.That(options.Converters.Count(converter => converter.CanConvert(typeof(Vec<double>))), Is.EqualTo(1));
            Assert.That(options.Converters.Count(converter => converter.CanConvert(typeof(Vec<Complex>))), Is.EqualTo(1));
            Assert.That(options.Converters.Count(converter => converter.CanConvert(typeof(Mat<int>))), Is.EqualTo(1));
            Assert.That(options.Converters.Count(converter => converter.CanConvert(typeof(Mat<float>))), Is.EqualTo(1));
            Assert.That(options.Converters.Count(converter => converter.CanConvert(typeof(Mat<double>))), Is.EqualTo(1));
            Assert.That(options.Converters.Count(converter => converter.CanConvert(typeof(Mat<Complex>))), Is.EqualTo(1));
            Assert.That(options.Converters.OfType<KMeansJsonConverter>().Count(), Is.EqualTo(1));
            Assert.That(options.Converters.OfType<GaussianMixtureModelJsonConverter>().Count(), Is.EqualTo(1));
            Assert.That(options.Converters.OfType<DiagonalGaussianMixtureModelJsonConverter>().Count(), Is.EqualTo(1));
            Assert.That(options.Converters.OfType<PrincipalComponentAnalysisJsonConverter>().Count(), Is.EqualTo(1));
            Assert.That(options.Converters.OfType<LinearDiscriminantAnalysisJsonConverter>().Count(), Is.EqualTo(1));
        }


        [Test]
        public void SourceGeneratedContextCanRoundTripNativeAotStyleVector()
        {
            var source = new Vec<double>(new[] { 1.0, 2.0, 3.0 });

            var json = JsonSerializer.Serialize(source, NumFlatJsonSerializerContext.Default.VectorDouble);
            var actual = JsonSerializer.Deserialize(json, NumFlatJsonSerializerContext.Default.VectorDouble);

            Assert.That(json, Is.EqualTo("[1,2,3]"));
            Assert.That(actual.ToArray(), Is.EqualTo(new[] { 1.0, 2.0, 3.0 }));
        }
    }
}
