namespace ColinChang.ExceptionHandler.Abstractions
{
    public class OperationResult<T> : IOperationResult<T>
    {
        public T Data { get; set; }

        public int Code { get; set; }
        public string ErrorMessage { get; set; }


        public OperationResult(T data, string errorMessage = null)
        {
            Data = data;
            ErrorMessage = errorMessage;
        }

        public OperationResult(T data, int code, string errorMessage = null) : this(data, errorMessage) => Code = code;
    }
}