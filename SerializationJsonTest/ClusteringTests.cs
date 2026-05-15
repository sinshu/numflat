using System;
using System.Linq;
using System.Text.Json;
using NumFlat;
using NumFlat.Clustering;
using NumFlat.Distributions;
using NumFlat.Serialization.Json;

namespace SerializationJsonTest
{
    public class ClusteringTests
    {
        [Test]
        public void RoundTripKMeansKeepsFittedParametersAndPredictBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = new KMeans(new[]
            {
                new Vec<double>(new[] { 1.0, 2.0 }),
                new Vec<double>(new[] { 10.0, 20.0 })
            });
            var x = new Vec<double>(new[] { 2.0, 3.0 });

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<KMeans>(json, options)!;

            Assert.That(json, Is.EqualTo(@"{""centroids"":[[1,2],[10,20]]}"));
            Assert.That(actual.Centroids[0].ToArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
            Assert.That(actual.Centroids[1].ToArray(), Is.EqualTo(new[] { 10.0, 20.0 }));
            Assert.That(actual.Predict(x), Is.EqualTo(source.Predict(x)));
        }

        [Test]
        public void DeserializeKMeansWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""centroids"":[[1,2],[3]]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<KMeans>(json, options)), Throws.TypeOf<JsonException>());
        }

        [Test]
        public void RoundTripGmmKeepsFittedParametersAndPredictionBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateGmm();
            var x = new Vec<double>(new[] { 1.2, 2.1 });
            var expectedProbabilities = source.PredictProbability(x);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<GaussianMixtureModel>(json, options)!;
            var actualProbabilities = actual.PredictProbability(x);

            Assert.That(json, Is.EqualTo(@"{""components"":[{""weight"":0.4,""gaussian"":{""mean"":[1,2],""covariance"":[[2,0.1],[0.1,3]]}},{""weight"":0.6,""gaussian"":{""mean"":[10,20],""covariance"":[[4,0.2],[0.2,5]]}}]}"));
            Assert.That(actual.Components[0].Weight, Is.EqualTo(0.4));
            Assert.That(actual.Components[0].Gaussian.Mean.ToArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
            Assert.That(actual.Components[0].Gaussian.Covariance[0, 1], Is.EqualTo(0.1));
            Assert.That(actual.Components[1].Weight, Is.EqualTo(0.6));
            Assert.That(actual.Predict(x), Is.EqualTo(source.Predict(x)));
            Assert.That(actualProbabilities.ToArray(), Is.EqualTo(expectedProbabilities.ToArray()).Within(1.0e-12));
        }

        [Test]
        public void DeserializeGmmWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""components"":[{""weight"":1,""gaussian"":{""mean"":[1,2],""covariance"":[[1,0],[0,1]]}},{""weight"":1,""gaussian"":{""mean"":[3],""covariance"":[[1]]}}]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<GaussianMixtureModel>(json, options)), Throws.TypeOf<JsonException>());
        }

        [Test]
        public void RoundTripDiagonalGmmKeepsFittedParametersAndPredictionBehavior()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            var source = CreateDiagonalGmm();
            var x = new Vec<double>(new[] { 1.2, 2.1 });
            var expectedProbabilities = source.PredictProbability(x);

            var json = JsonSerializer.Serialize(source, options);
            var actual = JsonSerializer.Deserialize<DiagonalGaussianMixtureModel>(json, options)!;
            var actualProbabilities = actual.PredictProbability(x);

            Assert.That(json, Is.EqualTo(@"{""components"":[{""weight"":0.4,""gaussian"":{""mean"":[1,2],""variance"":[2,3]}},{""weight"":0.6,""gaussian"":{""mean"":[10,20],""variance"":[4,5]}}]}"));
            Assert.That(actual.Components[0].Weight, Is.EqualTo(0.4));
            Assert.That(actual.Components[0].Gaussian.Mean.ToArray(), Is.EqualTo(new[] { 1.0, 2.0 }));
            Assert.That(actual.Components[0].Gaussian.Variance.ToArray(), Is.EqualTo(new[] { 2.0, 3.0 }));
            Assert.That(actual.Components[1].Weight, Is.EqualTo(0.6));
            Assert.That(actual.Predict(x), Is.EqualTo(source.Predict(x)));
            Assert.That(actualProbabilities.ToArray(), Is.EqualTo(expectedProbabilities.ToArray()).Within(1.0e-12));
        }

        [Test]
        public void DeserializeDiagonalGmmWithInvalidShapeThrowsJsonException()
        {
            var options = NumFlatJsonSerializerOptions.Create();
            const string json = @"{""components"":[{""weight"":1,""gaussian"":{""mean"":[1,2],""variance"":[1,1]}},{""weight"":1,""gaussian"":{""mean"":[3],""variance"":[1]}}]}";

            Assert.That((Action)(() => JsonSerializer.Deserialize<DiagonalGaussianMixtureModel>(json, options)), Throws.TypeOf<JsonException>());
        }

        private static GaussianMixtureModel CreateGmm()
        {
            var component1 = new GaussianMixtureModel.Component(
                0.4,
                new Gaussian(
                    new Vec<double>(new[] { 1.0, 2.0 }),
                    new Mat<double>(2, 2, 2, new[]
                    {
                        2.0, 0.1,
                        0.1, 3.0
                    })));
            var component2 = new GaussianMixtureModel.Component(
                0.6,
                new Gaussian(
                    new Vec<double>(new[] { 10.0, 20.0 }),
                    new Mat<double>(2, 2, 2, new[]
                    {
                        4.0, 0.2,
                        0.2, 5.0
                    })));
            return new GaussianMixtureModel(new[] { component1, component2 });
        }

        private static DiagonalGaussianMixtureModel CreateDiagonalGmm()
        {
            var component1 = new DiagonalGaussianMixtureModel.Component(
                0.4,
                new DiagonalGaussian(new Vec<double>(new[] { 1.0, 2.0 }), new Vec<double>(new[] { 2.0, 3.0 })));
            var component2 = new DiagonalGaussianMixtureModel.Component(
                0.6,
                new DiagonalGaussian(new Vec<double>(new[] { 10.0, 20.0 }), new Vec<double>(new[] { 4.0, 5.0 })));
            return new DiagonalGaussianMixtureModel(new[] { component1, component2 });
        }
    }
}
