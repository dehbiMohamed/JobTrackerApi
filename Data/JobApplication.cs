namespace JobTracker.api.Data
{
    public class JobApplication
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CompanyName { get; set; } = "";
        public string Title { get; set; } = "";
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAtUtc { get; set; }


    }
}
