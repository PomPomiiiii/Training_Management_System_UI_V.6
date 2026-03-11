namespace TrainingManagementSystem.Api.Common.Results
{
    public class Response<T>
    {
        public bool IsSuccess { get; set; }
        public bool IsFailure => !IsSuccess;
        public T Result { get; set; }
        public string Message { get; set; }

        private Response(bool isSuccess, T result, string message)
        {
            IsSuccess = isSuccess;
            Result = result;
            Message = message;
        }

        public static Response<T> Success(T Result) 
            => new Response<T>(true, Result, "Successful Request");
        public static Response<T> Failure(string message) 
            => new Response<T>(false, default!, message);

    }
}
