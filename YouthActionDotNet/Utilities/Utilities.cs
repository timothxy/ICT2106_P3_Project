using System;
using System.Security.Cryptography;


public static class u{

    public static string hashpassword(string password){
        
        SHA256 sha256 = SHA256.Create();
        var secretPw = Convert.ToHexString(sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        sha256.Dispose();
        return secretPw;
    }
}