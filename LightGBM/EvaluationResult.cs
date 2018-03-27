//using EvaluationResult = System.ValueTuple<string, string, double, bool>;
namespace LightGBM
{
    public struct EvaluationResult
    {
        public readonly string TestName;
        public readonly string MetricName;
        public readonly double Score;
        public readonly bool BiggerIsBetter;
        public EvaluationResult(string testName, string metricName, double score, bool biggerIsBetter)
        {
            TestName = testName;
            MetricName = metricName;
            Score = score;
            BiggerIsBetter = biggerIsBetter;
        }
    }
}