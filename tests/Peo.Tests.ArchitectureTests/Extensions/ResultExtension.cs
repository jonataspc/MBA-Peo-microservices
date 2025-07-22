using NetArchTest.Rules;

namespace Peo.Tests.ArchitectureTests.Extensions
{
    internal static class ResultExtension
    {
        public static string? GetDetails(this TestResult result)
        {
            if (!result.FailingTypes.Any())
            {
                return null;
            }

            return string.Join(", ", result.FailingTypes.Select(x => $"{x.FullName}: {x.Explanation}"));
        }
    }
}