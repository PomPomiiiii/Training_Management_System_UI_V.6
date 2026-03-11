using System.Security.Cryptography;
using System.Text;

public static class PasswordGenerator
{
    private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
    private const string Digits = "0123456789";
    private const string Special = "!@#$%^&*()-_=+[]{}|;:,.<>?";

    public static string Generate(int length = 12)
    {
        if (length < 8)
            throw new ArgumentException("Password length must be at least 8 characters.");

        var password = new StringBuilder();
        var allChars = Uppercase + Lowercase + Digits + Special;

        // Ensure required characters
        password.Append(GetRandomChar(Uppercase));
        password.Append(GetRandomChar(Lowercase));
        password.Append(GetRandomChar(Digits));
        password.Append(GetRandomChar(Special));

        // Fill remaining length
        for (int i = password.Length; i < length; i++)
        {
            password.Append(GetRandomChar(allChars));
        }

        return Shuffle(password.ToString());
    }

    private static char GetRandomChar(string chars)
    {
        byte[] buffer = new byte[4];
        RandomNumberGenerator.Fill(buffer);
        uint num = BitConverter.ToUInt32(buffer, 0);
        return chars[(int)(num % (uint)chars.Length)];
    }

    private static string Shuffle(string input)
    {
        var array = input.ToCharArray();
        for (int i = array.Length - 1; i > 0; i--)
        {
            byte[] buffer = new byte[4];
            RandomNumberGenerator.Fill(buffer);
            int j = BitConverter.ToInt32(buffer, 0) & int.MaxValue % (i + 1);

            (array[i], array[j]) = (array[j], array[i]);
        }
        return new string(array);
    }
}
