namespace ContactManager.WebUI.Models.Authentication
{
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public List<string>? DataValidationErrors { get; set; }
    }
}
