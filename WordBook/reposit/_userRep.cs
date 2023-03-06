using Microsoft.EntityFrameworkCore;
using WordBook.Helpers;
using WordBook.Models;

namespace WordBook.reposit
{
    public class _userRep
    {
        private readonly ApplicationDbContext db;
        public _userRep(ApplicationDbContext context)
        {
            db = context;
        }
        public User? Auth(string? name, string? pass)
        {
            User? strudent = db.Student.Include(p => p.Role).FirstOrDefault(p => p.Name == name);
            //Role role = db.Roles.FirstOrDefault(p => p.Name == strudent.Role.Name);
            if (strudent != null)
            {
                if(HashPassword.Verif(pass, strudent.Password))
                {
                    return strudent;
                }
            }
            return null;
        }

        public string Reg(string name, string pass, string email)
        {
            User? IsStudentName = db.Student.FirstOrDefault(p => p.Name == name);
            User? IsStudentEmail = db.Student.FirstOrDefault(p => p.Email == email);
            Role? role = db.Roles.FirstOrDefault(p => p.Name == "user");
            if (IsStudentName == null && IsStudentEmail == null)
            {
                User student = new User
                {
                    Name = name,
                    Email = email,
                    Password = HashPassword.HashPass(pass),
                    Role = role ?? null,
                };
                db.Student.Add(student);
                db.SaveChanges();

                return "created" ;
            }

            if(IsStudentEmail != null)
            {
                return "try another mail";   
            }

            return "try another name";
        }

        public void setUsed(RefreshToken token)
        {
            token.Used = true;
            db.RefreshTokens.Update(token);
            db.SaveChanges();
        }

        public User? getUserByToken(RefreshToken token) 
        {
            return db.Student.FirstOrDefault(p => p.Id == token.StudentId);
        }

        public RefreshToken? FindToken (string token)
        {
            return db.RefreshTokens.SingleOrDefault(p => p.Token == token);
        }

        public void saveToken (RefreshToken refTokenToResponse)
        {
            db.RefreshTokens.Add(refTokenToResponse);
            db.SaveChanges();
        }

        public bool deleteToken (string token)
        {
            try
            {
                var tk = db.RefreshTokens.SingleOrDefault(p => p.Token == token);
                db.RefreshTokens.Remove(tk);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
