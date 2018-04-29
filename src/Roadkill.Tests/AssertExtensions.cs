using Newtonsoft.Json;
using Xunit;

namespace Roadkill.Tests
{
    public class AssertExtensions
    {
        public static void Equivalent(object expected, object actual)
        {
            string expectedJson = JsonConvert.SerializeObject(expected);
            string actualJson = JsonConvert.SerializeObject(actual);

            Assert.Equal(expectedJson, actualJson);
        }
    }
}