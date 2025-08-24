public class AllUsersJobsResponseModel
{
    public List<UserJobResponseModel> Jobs { get; set; } = new List<UserJobResponseModel>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}