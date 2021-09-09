namespace OCSServices.Matrixx.Agent.Business
{
    public class Result
    {
        private readonly int _result;

        public Result(int result)
        {
            _result = result;
        }

        public bool IsSuccess { get { return _result == 0; } }

        public bool IsFailed { get { return !IsSuccess; }}
    }
}
