using System.Text;
using Microsoft.AspNetCore.Mvc;
using ASPPRODUCT.Models;
using Microsoft.AspNetCore.Authorization;
using ASPPRODUCT.Container;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ASPPRODUCT.Handler;

namespace ASPPRODUCT.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly LearnContext _learnContext;
    private readonly JwtSettings jwtSettings;
    private readonly IRefereshTokenGenerator _refereshTokenGenerator;
    public UserController(LearnContext learnContext, IOptions<JwtSettings> options, IRefereshTokenGenerator refereshTokenGenerator)
    {
        _learnContext = learnContext;
        jwtSettings = options.Value;
        _refereshTokenGenerator = refereshTokenGenerator;
    }

    [HttpPost("Authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] UserCred userCred)
    {
        var user = await this._learnContext.TblUsers.FirstOrDefaultAsync(item => item.Userid == userCred.username && item.Password == userCred.password);
        if (user == null)
        {
            return Unauthorized();
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.UTF8.GetBytes(this.jwtSettings.securityKey);

        // var claim = new[] { new Claim(ClaimTypes.Name, username) };
        var tokenDesc = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, user.Userid), new Claim(ClaimTypes.Role, user.Role) }),
            Expires = DateTime.Now.AddSeconds(20),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDesc);
        string finalToken = tokenHandler.WriteToken(token);

        var response = new TokenResponse()
        {
            jwttoken = finalToken,
            refreshtoken = await this._refereshTokenGenerator.GenerateToken(userCred.username)
        };
        return Ok(response);
    }

    [NonAction]
    public async Task<TokenResponse> TokenAuthenticate(string user, Claim[] claims)
    {
        var tokenKey = Encoding.UTF8.GetBytes(this.jwtSettings.securityKey);
        var token = new JwtSecurityToken(
          claims: claims,
          expires: DateTime.Now.AddSeconds(20),
          signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
        );

        var jwttoken = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenResponse()
        {
            jwttoken = jwttoken,
            refreshtoken = await this._refereshTokenGenerator.GenerateToken(user)
        };
    }

    [HttpPost("RefToken")]
    public async Task<IActionResult> RefToken([FromBody] TokenResponse tokenResponse)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.UTF8.GetBytes(this.jwtSettings.securityKey);
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(tokenResponse.jwttoken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
            ValidateIssuer = false,
            ValidateAudience = false,
        }, out securityToken);

        var token = securityToken as JwtSecurityToken;
        if (token != null && !token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
        {
            return Unauthorized();
        }

        var username = principal.Identity?.Name;

        var user = await this._learnContext.TblRefreshtokens.FirstOrDefaultAsync(item => item.UserId == username && item.RefreshToken == tokenResponse.refreshtoken);

        if (user == null)
        {
            return Unauthorized();
        }

        var response = this.TokenAuthenticate(username, principal.Claims.ToArray()).Result;

        return Ok(response);
    }
}