namespace Devon4Net.Infrastructure.AnsibleTower.Dto
{
    public class PaginatedResultDto<T>
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public T[] results { get; set; }

    }
}
