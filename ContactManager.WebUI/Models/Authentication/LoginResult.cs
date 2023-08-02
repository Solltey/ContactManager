namespace ContactManager.WebUI.Models.Authentication
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public List<string>? DataValidationErrors { get; set; }
        public string? Token { get; set; }
    }
}
