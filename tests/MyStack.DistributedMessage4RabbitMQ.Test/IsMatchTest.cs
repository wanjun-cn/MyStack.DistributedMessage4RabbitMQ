namespace MyStack.DistributedMessage4RabbitMQ.Test
{
    public class IsMatchTest
    {
        public bool IsMatch(string pattern, string input)
        {
            pattern = pattern.Replace(".", @"\.").Replace("*", "[^.]").Replace("#", ".*");
            return System.Text.RegularExpressions.Regex.IsMatch(input, $"^{pattern}$");
        }
        [Test]
        public void TestIsMatch()
        {
            var testCases = new (string pattern, string input, bool expectedResult)[]
            {
                ("a.*.c", "a.b.c", true),
                ("a.*.b", "a.c.d.b", false),
                ("a.#.c", "a.b.c", true),
                ("a.#.b", "a.c.d.b", true)
            };


            foreach (var (pattern, input, expectedResult) in testCases)
            {
                var actualResult = IsMatch(pattern, input);
                Assert.That(actualResult, Is.EqualTo(expectedResult));
            }
        }
    }
}