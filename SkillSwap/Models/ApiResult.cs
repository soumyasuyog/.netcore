namespace SkillSwap.Models
{
    public class ApiResult
    {
        public object? Data { get; set; }
        public bool IsSuccessfull { get; set; }
        public string? Message { get; set; }
    }

    //public class ApiResult<T>
    //{
    //    public bool IsSuccessfull { get; set; }
    //    public string? Message { get; set; }
    //    public object? Data { get; set; }
    //}

}
