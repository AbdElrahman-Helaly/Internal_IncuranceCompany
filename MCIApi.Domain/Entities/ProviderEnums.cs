namespace MCIApi.Domain.Entities
{
    // Enum for Provider Class (A, B, C, P)
    public enum ProviderClass
    {
        A = 1,
        B = 2,
        C = 3,
        P = 4
    }

    // Enum for Importance Level (A, AA, AAK, X, Y)
    public enum ImportanceLevel
    {
        A = 1,
        AA = 2,
        AAK = 3,
        X = 4,
        Y = 5
    }

    // Enum for Provider Review Status (Need Review dropdown)
    public enum ReviewStatus
    {
        NeedReview = 1,
        FullyReviewed = 2,
        MissingDataEntry = 3,
        MissingAttachments = 4,
        MissingDataAndAttachments = 5
    }

    // Enum for Provider Priority
    public enum Priority
    {
        BAD = 1,
        G_D = 2,
        IMPORTANT = 3,
        M_G = 4
    }
}

