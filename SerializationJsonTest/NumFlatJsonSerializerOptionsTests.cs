using System.Linq;
using System.Text.Json;
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

            Assert.That(options.Converters.OfType<VecJsonConverterFactory>().Count(), Is.EqualTo(1));
            Assert.That(options.Converters.OfType<MatJsonConverterFactory>().Count(), Is.EqualTo(1));
            Assert.That(options.Converters.OfType<PrincipalComponentAnalysisJsonConverter>().Count(), Is.EqualTo(1));
            Assert.That(options.Converters.OfType<LinearDiscriminantAnalysisJsonConverter>().Count(), Is.EqualTo(1));
        }
    }
}
