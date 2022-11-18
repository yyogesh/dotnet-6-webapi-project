using System.Security.Cryptography;
using ASPPRODUCT.Models;
using Microsoft.EntityFrameworkCore;


namespace ASPPRODUCT.Handler;
public class RefereshTokenGenerator : IRefereshTokenGenerator
{
    private readonly LearnContext _learnContext;
    public RefereshTokenGenerator(LearnContext learnContext)
    {
        _learnContext = learnContext;
    }
    public async Task<string> GenerateToken(string username)
    {
        var randomnumber = new byte[32];
        using (var ramdomnumbergenerator = RandomNumberGenerator.Create())
        {
            ramdomnumbergenerator.GetBytes(randomnumber);
            string refreshtoken = Convert.ToBase64String(randomnumber);
            var token = await this._learnContext.TblRefreshtokens.FirstOrDefaultAsync(item => item.UserId == username);
            if (token != null)
            {
                token.RefreshToken = refreshtoken;
            }
            else
            {
                this._learnContext.TblRefreshtokens.Add(new TblRefreshtoken()
                {
                    UserId = username,
                    TokenId = new Random().Next().ToString(),
                    RefreshToken = refreshtoken,
                    IsActive = true
                });
            }
            await this._learnContext.SaveChangesAsync();
            return refreshtoken;
        }
    }
}