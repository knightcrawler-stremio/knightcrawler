namespace Producer.Features.DataProcessing
{
    public class LengthAwareRatioScorer : IRatioScorer
    {
        private readonly IRatioScorer _defaultScorer = new DefaultRatioScorer();

        public int Score(string input1, string input2)
        {
            var score = _defaultScorer.Score(input1, input2);
            var lengthRatio = (double)Math.Min(input1.Length, input2.Length) / Math.Max(input1.Length, input2.Length);
            var result = (int)(score * lengthRatio);
            return result > 100 ? 100 : result;
        }

        public int Score(string input1, string input2, PreprocessMode preprocessMode)
        {
            var score = _defaultScorer.Score(input1, input2, preprocessMode);
            var lengthRatio = (double)Math.Min(input1.Length, input2.Length) / Math.Max(input1.Length, input2.Length);
            var result = (int)(score * lengthRatio);

            return result > 100 ? 100 : result;
        }
    }
}