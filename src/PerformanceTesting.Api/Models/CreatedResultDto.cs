namespace PerformanceTesting.Api.Models
{
    public sealed class CreatedResultDto
    {
        public CreatedResultDto(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}
