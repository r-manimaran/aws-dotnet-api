using Microsoft.EntityFrameworkCore;

namespace api_lambda_deployment.Services;

public class UrlShortnerService
{
    private readonly ApplicationDbContext _context;
    public const int NumberOfCharsInShortLink = 7;
    private const string CharSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private readonly Random _random = new Random();
    public UrlShortnerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateUniqueCode()
    {
        var codeChars = new char[NumberOfCharsInShortLink];

        while (true)
        {
            for (int i = 0; i < NumberOfCharsInShortLink; i++)
            {
                var randomIndex = _random.Next(CharSet.Length);
                codeChars[i] = CharSet[randomIndex];
            }
            var code = new string(codeChars);
            if (!await _context.shortenedUrls.AnyAsync(s => s.Code == code))
            {
                return code;
            }
        }
    }
}