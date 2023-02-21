namespace REDAirCalculator.Utilities
{
    public static class MailTexts
    {
        public static string CreateMessageForSendProject(
            string name,
            string email,
            string company,
            string body)
        {
            string message = body.Replace("[Name]", name)
                .Replace("[Email]", email);
            message = message.Replace("[Company]", company);
            return message;
        }

        public static string CreateMessageForChangePassword(
            string link,
            string body)
        {
            string message = body.Replace("/[RecoveryLink]", link);
            return message;
        }
    }
}