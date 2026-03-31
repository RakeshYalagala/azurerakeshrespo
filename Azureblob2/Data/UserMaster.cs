using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Azureblob2.Data
{
  
        [Table("USER_MASTER", Schema = "dbo")]
        public class UserMaster
        {
            [Key]
            [MaxLength(50)]
            [Column("USER_ID")]
            [JsonPropertyName("USER_ID")]
            public string UserId { get; set; } = null!;

            [Required]
            [MaxLength(50)]
            [Column("USER_NAME")]
            [JsonPropertyName("USER_NAME")]
            public string UserName { get; set; } = null!;

            [MaxLength(100)] // bcrypt hash length
            [Column("PASSWORD")]
            [JsonPropertyName("PASSWORD")]
            public string? Password { get; set; }

            [Required]
            [MaxLength(50)]
            [EmailAddress]
            [Column("USER_EMAIL")]
            [JsonPropertyName("USER_EMAIL")]
            public string UserEmail { get; set; } = null!;

            [MaxLength(20)]
            [Column("USER_CONTACT_NO")]
            [JsonPropertyName("USER_CONTACT_NO")]
            public string? UserContactNo { get; set; }

            [MaxLength(200)]
            [Column("ADDRESS")]
            [JsonPropertyName("ADDRESS")]
            public string? Address { get; set; }

            [MaxLength(50)]
            [Column("CREATED_BY")]
            [JsonPropertyName("CREATED_BY")]
            public string? CreatedBy { get; set; }

            [Column("CREATED_ON")]
            [JsonPropertyName("CREATED_ON")]
            public DateTime? CreatedOn { get; set; }

            [MaxLength(50)]
            [Column("MODIFIED_BY")]
            [JsonPropertyName("MODIFIED_BY")]
            public string? ModifiedBy { get; set; }

            [Column("MODIFIED_ON")]
            [JsonPropertyName("MODIFIED_ON")]
            public DateTime? ModifiedOn { get; set; }

            [Required]
            [Column("IS_ACTIVE")]
            [JsonPropertyName("IS_ACTIVE")]
            public bool IsActive { get; set; } = true;

            [MaxLength(50)]
            [Column("REP_MGRTOKEN")]
            [JsonPropertyName("REP_MGRTOKEN")]
            public string? RepMgrToken { get; set; }

            [MaxLength(10)]
            [Column("USER_TYPE")]
            [JsonPropertyName("USER_TYPE")]
            public string? UserType { get; set; } = "AD";

            [Column("FAILED_ATTEMPTS")]
            [JsonPropertyName("FAILED_ATTEMPTS")]
            public int FailedAttempts { get; set; } = 0;

            [Column("IS_BLOCKED")]
            [JsonPropertyName("IS_BLOCKED")]
            public bool IsBlocked { get; set; } = false;

            [Column("LAST_FAILED_ATTEMPT")]
            [JsonPropertyName("LAST_FAILED_ATTEMPT")]
            public DateTime? LastFailedAttempt { get; set; }

            [Column("LOCKOUT_END_TIME")]
            [JsonPropertyName("LOCKOUT_END_TIME")]
            public DateTime? LockoutEndTime { get; set; }
        }
    }

