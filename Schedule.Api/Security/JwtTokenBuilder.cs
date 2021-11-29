using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Schedule.Security
{
    public class JwtTokenBuilder
    {
        private readonly Dictionary<string, string> _claims = new Dictionary<string, string>();
        private string _audience = string.Empty;
        private double _expiryInHours = 1;
        private string _issuer = string.Empty;
        private SecurityKey _securityKey;
        private string _subject = string.Empty;
        private string _algorithm = string.Empty;

        public JwtTokenBuilder AddSecurityKey(SecurityKey securityKey)
        {
            _securityKey = securityKey;
            return this;
        }

        public JwtTokenBuilder AddAlgorithm(string algorithm)
        {
            _algorithm = algorithm;
            return this;
        }

        public JwtTokenBuilder AddSubject(string subject)
        {
            _subject = subject;
            return this;
        }

        public JwtTokenBuilder AddIssuer(string issuer)
        {
            _issuer = issuer;
            return this;
        }

        public JwtTokenBuilder AddAudience(string audience)
        {
            _audience = audience;
            return this;
        }

        public JwtTokenBuilder AddClaim(string type, string value)
        {
            _claims.Add(type, value);
            return this;
        }

        public JwtTokenBuilder AddClaimRole(string type, string value)
        {
            _claims.Add(type, value);
            return this;
        }

        public JwtTokenBuilder AddExpiry(double expiryInHours)
        {
            _expiryInHours = expiryInHours;
            return this;
        }

        public JwtToken Build()
        {
            EnsureArguments();
            var claimsIdentity = new ClaimsIdentity();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var (key, value) in _claims) claims.Add(new Claim(key, value));

            claimsIdentity.AddClaims(claims);

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claimsIdentity.Claims,
                expires: DateTime.UtcNow.AddHours(_expiryInHours),
                signingCredentials: new SigningCredentials(_securityKey, _algorithm));

            return new JwtToken(token);
        }

        private void EnsureArguments()
        {
            if (_securityKey == null)
            {
                Log.Error("Jwt Authentication - Security Key is null");
                throw new ArgumentNullException("SecurityKey");
            }

            if (string.IsNullOrEmpty(_subject))
            {
                Log.Error("Jwt Authentication - Subject is empty or null");
                throw new ArgumentNullException("Subject");
            }

            if (string.IsNullOrEmpty(_issuer))
            {
                Log.Error("Jwt Authentication - Issuer is empty or null");
                throw new ArgumentNullException("Issuer");
            }

            if (string.IsNullOrEmpty(_audience))
            {
                Log.Error("Jwt Authentication - Audience is empty or null");
                throw new ArgumentNullException("Audience");
            }
        }
    }
}
