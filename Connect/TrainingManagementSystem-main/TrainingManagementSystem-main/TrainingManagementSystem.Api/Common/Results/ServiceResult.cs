namespace TrainingManagementSystem.Api.Common.Results
{

    public class ServiceResult<T> : ServiceResult 
    {
        public T Value { get; set; }

        protected ServiceResult(bool isSuccess,T value, string message) 
            : base(isSuccess, message)
        {
            Value = value;
        }

        public static ServiceResult<T> Success(T value)
            => new(true, value, "Successful Request");

        public static ServiceResult<T> Failure(string error)
            => new(false, default!, error);

    }
    
    public class ServiceResult
    {
        public bool IsSuccess { get; set; } = false;
        public bool IsFailure => !IsSuccess;
        public string? Message { get; set; }

        protected ServiceResult(bool isSuccess, string message) 
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static ServiceResult Success => new(true, "Successful request");
        public static ServiceResult Failure(string errorMessage) => new(false, errorMessage);
    }
}
