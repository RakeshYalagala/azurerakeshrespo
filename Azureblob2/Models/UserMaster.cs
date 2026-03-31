using System;
using System.Collections.Generic;

namespace Azureblob2.Models;

public partial class UserMaster
{
    public string UserId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string? Password { get; set; }

    public string UserEmail { get; set; } = null!;

    public string? UserContactNo { get; set; }

    public string? Address { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public bool IsActive { get; set; }

    public string? RepMgrtoken { get; set; }

    public string? UserType { get; set; }

    public DateTime? LockoutEndTime { get; set; }

    public DateTime? LastFailedAttempt { get; set; }

    public bool IsBlocked { get; set; }

    public int FailedAttempts { get; set; }
}
