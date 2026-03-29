namespace MCIApi.Domain.Entities
{
    /// <summary>
    /// Enum for Approval Medicine Status
    /// </summary>
    public enum ApprovalStatusId
    {
        APPROVED = 1,
        REJECTED = 2
    }

    /// <summary>
    /// Enum for Approval Source
    /// </summary>
    public enum ApprovalSource
    {
        Email = 1,
        WhatsApp = 2,
        Mediconsult = 3,
        Manual = 4,
        Portal = 5,
        ProviderPortal = 6,
        API = 7
    }

    /// <summary>
    /// Enum for Approval Status (calculated from medicines/services)
    /// </summary>
    public enum ApprovalStatus
    {
        Received = 1,
        Approved = 2,
        PartiallyApproved = 3,
        Dispensed = 4,
        PartiallyDispensed = 5,
        Rejected = 6
    }

    public enum DurationType
    {
        DayCase = 1,    // حالة نهارية
        Day = 2         // يوم
    }
}

