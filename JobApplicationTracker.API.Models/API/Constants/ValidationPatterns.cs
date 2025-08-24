namespace JobApplicationTracker.API.Models.API.Constants
{
    public static class ValidationPatterns
    {
        // Requires at least one lowercase, one uppercase, one digit, and one non-alphanumeric symbol
        public const string PasswordComplexity = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$";
    }
}


