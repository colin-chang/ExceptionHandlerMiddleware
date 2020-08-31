namespace ColinChang.ExceptionHandler.Abstractions
{
    public interface IOperationResult
    {
        /// <summary>
        /// custom operation code
        /// </summary>
        int Code { get; set; }

        /// <summary>
        /// error message
        /// </summary>
        string ErrorMessage { get; set; }
    }

    /// <summary>
    /// actual response data type that will return to client
    /// </summary>
    /// <typeparam name="T">response type</typeparam>
    public interface IOperationResult<T> : IOperationResult
    {
        /// <summary>
        /// response data
        /// </summary>
        T Data { get; set; }
    }
}