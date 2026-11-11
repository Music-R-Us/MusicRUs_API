using MusicRUs.API.Backend.Logic.Encryption;
using MusicRUs.API.Backend.Logic.TwoFactor;
using MusicRUs.API.DTOs;
using MusicRUs.API.Models.Entities;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Services.Interfaces;

namespace MusicRUs.API.Services;

public class TwoFactorService : ITwoFactorService
{
    private readonly IUserRepository _userRepo;
    private readonly IEncryptionService _encryptionService;
    private readonly ITotpService _totpService;

    public TwoFactorService(IUserRepository userRepo, IEncryptionService encryptionService, ITotpService totpService)
    {
        _userRepo = userRepo;
        _encryptionService = encryptionService;
        _totpService = totpService;
    }

    public async Task<TwoFactorSetupResult> SetupAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user is null) return new(null, TwoFactorError.NotFound);
        if (user.TwoFactorEnabled) return new(null, TwoFactorError.AlreadyEnabled);

        // New secret every time until 2FA is enabled. Stored encrypted, like the e-mail
        var secret = _totpService.GenerateSecret();
        user.TotpSecret = _encryptionService.Encrypt(secret);
        user.TotpLastUsedStep = null;
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();

        var setup = new TwoFactorSetupDto
        {
            Secret = secret,
            QrCodeDataUri = _totpService.GetQrCodeDataUri(secret, user.Username)
        };
        return new(setup, TwoFactorError.None);
    }

    public async Task<TwoFactorError> EnableAsync(int userId, string code)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user is null) return TwoFactorError.NotFound;
        if (user.TwoFactorEnabled) return TwoFactorError.AlreadyEnabled;
        if (user.TotpSecret is null) return TwoFactorError.NotSetUp;
        if (!VerifyAndConsumeCode(user, code)) return TwoFactorError.InvalidCode;

        user.TwoFactorEnabled = true;
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
        return TwoFactorError.None;
    }

    public async Task<TwoFactorError> DisableAsync(int userId, string code)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user is null) return TwoFactorError.NotFound;
        if (!user.TwoFactorEnabled) return TwoFactorError.NotSetUp;
        if (!VerifyAndConsumeCode(user, code)) return TwoFactorError.InvalidCode;

        user.TwoFactorEnabled = false;
        user.TotpSecret = null;
        user.TotpLastUsedStep = null;
        _userRepo.Update(user);
        await _userRepo.SaveChangesAsync();
        return TwoFactorError.None;
    }

    // Checks the code and marks its time step as used, so the same code cannot be used twice.
    // Does not save: the caller saves.
    public bool VerifyAndConsumeCode(User user, string code)
    {
        if (user.TotpSecret is null) return false;

        var secret = _encryptionService.Decrypt(user.TotpSecret);
        if (!_totpService.VerifyCode(secret, code, out var timeStep)) return false;

        // Reject a code from the same or an earlier 30-second window than the last one used
        if (user.TotpLastUsedStep is not null && timeStep <= user.TotpLastUsedStep) return false;

        user.TotpLastUsedStep = timeStep;
        return true;
    }
}