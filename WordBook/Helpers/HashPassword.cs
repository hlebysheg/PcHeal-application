namespace WordBook.Helpers
{
    public class HashPassword
    {
        private HashPassword()
        {

        }
        public static string HashPass(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password + "adasd");
        }
        public static bool Verif (string pass, string hashPass)
        {
            return BCrypt.Net.BCrypt.Verify(pass + "adasd", hashPass);
        }
       
    }
}
