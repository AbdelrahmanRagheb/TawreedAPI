using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
class Program {
    static void Main() {
        string oldHash = "zH42ipi5/1vZEOde4rMTgw==.Ojo4Rc6Uf2z7Wur0CIAX4kuX57iB1c/6kOGvJxmBQMY=";
        Console.WriteLine("Old hash: " + oldHash);
        Console.WriteLine("Verify old: " + VerifyPassword("Tawreed@123", oldHash));
        
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        var hash = KeyDerivation.Pbkdf2("Tawreed@123", salt, KeyDerivationPrf.HMACSHA256, 100000, 32);
        string newHash = Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        Console.WriteLine("New hash: " + newHash);
        Console.WriteLine("Verify new: " + VerifyPassword("Tawreed@123", newHash));
    }
    static bool VerifyPassword(string password, string storedHash) {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var storedHashBytes = Convert.FromBase64String(parts[1]);
        var hash = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100000, 32);
        return CryptographicOperations.FixedTimeEquals(storedHashBytes, hash);
    }
}
