using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace JobTracker.Models
{
    [Table("jobs")]
    public class Job : BaseModel
    {
        [PrimaryKey("id", false)]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("company")]
        public string Company { get; set; } = string.Empty;

        [Column("role")]
        public string Role { get; set; } = string.Empty;

        [Column("status")]
        public string Status { get; set; } = "applied";

        [Column("applied_date")]
        public DateTime AppliedDate { get; set; } = DateTime.Today;

        [Column("interview_date")]
        public DateTime? InterviewDate { get; set; }

        [Column("interview_type")]
        public string? InterviewType { get; set; }

        [Column("website_link")]
        public string? WebsiteLink { get; set; }

        [Column("location")]
        public string? Location { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}

