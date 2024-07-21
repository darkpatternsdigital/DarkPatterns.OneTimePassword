
using System.Security.Cryptography;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace DarkPatterns.OneTimePassword.Persistence;

public static class PasswordHashing
{
	const int saltByteCount = 16;
	const int iterationCount = 100_000;
	const int hashByteCount = 32;

	public static byte[] GeneratePasswordHash(string otp)
	{
		var salt = RandomNumberGenerator.GetBytes(saltByteCount);
		var hash = KeyDerivation.Pbkdf2(
			password: otp,
			salt: salt,
			prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: iterationCount,
			numBytesRequested: hashByteCount);
		var saltedHash = new byte[hashByteCount + saltByteCount];
		salt.CopyTo(saltedHash, 0);
		hash.CopyTo(saltedHash, saltByteCount);

		return saltedHash;
	}

	internal static bool VerifyHash(byte[] passwordHash, string otp)
	{
		var salt = passwordHash[..saltByteCount];
		var actualHash = passwordHash[saltByteCount..];
		var expectedHash = KeyDerivation.Pbkdf2(
			password: otp,
			salt: salt,
			prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: iterationCount,
			numBytesRequested: hashByteCount);
		return actualHash.SequenceEqual(expectedHash);
	}

}
