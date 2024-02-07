﻿using MimeKit;
using Monstersoft.VennWms.Main.Application.Repositories.AuthenticationRepositories;
using Monstersoft.VennWms.Main.Application.Services.Abstract.AuthenticationServices;
using Orhanization.Core.CrossCuttingConcerns.Exceptions.Types;
using Orhanization.Core.Mailing;
using Orhanization.Core.Security.EmailAuthenticator;
using Orhanization.Core.Security.Entities;
using Orhanization.Core.Security.Enums;
using Orhanization.Core.Security.OtpAuthenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monstersoft.VennWms.Main.Application.Services.Concrete.AuthenticationServices;

public class AuthenticatorManager : IAuthenticatorService
{
    private readonly IEmailAuthenticatorHelper _emailAuthenticatorHelper;
    private readonly IEmailAuthenticatorRepository _emailAuthenticatorRepository;
    private readonly IMailService _mailService;
    private readonly IOtpAuthenticatorHelper _otpAuthenticatorHelper;
    private readonly IOtpAuthenticatorRepository _otpAuthenticatorRepository;

    public AuthenticatorManager(
        IEmailAuthenticatorHelper emailAuthenticatorHelper,
        IEmailAuthenticatorRepository emailAuthenticatorRepository,
        IMailService mailService,
        IOtpAuthenticatorHelper otpAuthenticatorHelper,
        IOtpAuthenticatorRepository otpAuthenticatorRepository
    )
    {
        _emailAuthenticatorHelper = emailAuthenticatorHelper;
        _emailAuthenticatorRepository = emailAuthenticatorRepository;
        _mailService = mailService;
        _otpAuthenticatorHelper = otpAuthenticatorHelper;
        _otpAuthenticatorRepository = otpAuthenticatorRepository;
    }

    public async Task<EmailAuthenticator> CreateEmailAuthenticator(User user)
    {
        EmailAuthenticator emailAuthenticator =
            new()
            {
                UserId = user.Id,
                ActivationKey = await _emailAuthenticatorHelper.CreateEmailActivationKey(),
                IsVerified = false
            };
        return emailAuthenticator;
    }

    public async Task<OtpAuthenticator> CreateOtpAuthenticator(User user)
    {
        OtpAuthenticator otpAuthenticator =
            new()
            {
                UserId = user.Id,
                SecretKey = await _otpAuthenticatorHelper.GenerateSecretKey(),
                IsVerified = false
            };
        return otpAuthenticator;
    }

    public async Task<string> ConvertSecretKeyToString(byte[] secretKey)
    {
        string result = await _otpAuthenticatorHelper.ConvertSecretKeyToString(secretKey);
        return result;
    }

    public async Task SendAuthenticatorCode(User user)
    {
        if (user.AuthenticatorType is AuthenticatorType.Email)
            await SendAuthenticatorCodeWithEmail(user);
    }

    public async Task VerifyAuthenticatorCode(User user, string authenticatorCode)
    {
        if (user.AuthenticatorType is AuthenticatorType.Email)
            await VerifyAuthenticatorCodeWithEmail(user, authenticatorCode);
        else if (user.AuthenticatorType is AuthenticatorType.Otp)
            await VerifyAuthenticatorCodeWithOtp(user, authenticatorCode);
    }

    private async Task SendAuthenticatorCodeWithEmail(User user)
    {
        EmailAuthenticator? emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(e => e.UserId == user.Id);
        if (emailAuthenticator is null)
            throw new NotFoundException("Email Authenticator not found.");
        if (!emailAuthenticator.IsVerified)
            throw new BusinessException("Email Authenticator must be is verified.");

        string authenticatorCode = await _emailAuthenticatorHelper.CreateEmailActivationCode();
        emailAuthenticator.ActivationKey = authenticatorCode;
        await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);

        var toEmailList = new List<MailboxAddress> { new(name: $"{user.FirstName} {user.LastName}", user.Email) };

        _mailService.SendMail(
            new Mail
            {
                ToList = toEmailList,
                Subject = "Authenticator Code - RentACar",
                TextBody = $"Enter your authenticator code: {authenticatorCode}"
            }
        );
    }

    private async Task VerifyAuthenticatorCodeWithEmail(User user, string authenticatorCode)
    {
        EmailAuthenticator? emailAuthenticator = await _emailAuthenticatorRepository.GetAsync(e => e.UserId == user.Id);
        if (emailAuthenticator is null)
            throw new NotFoundException("Email Authenticator not found.");
        if (emailAuthenticator.ActivationKey != authenticatorCode)
            throw new BusinessException("Authenticator code is invalid.");
        emailAuthenticator.ActivationKey = null;
        await _emailAuthenticatorRepository.UpdateAsync(emailAuthenticator);
    }

    private async Task VerifyAuthenticatorCodeWithOtp(User user, string authenticatorCode)
    {
        OtpAuthenticator? otpAuthenticator = await _otpAuthenticatorRepository.GetAsync(e => e.UserId == user.Id);
        if (otpAuthenticator is null)
            throw new NotFoundException("Otp Authenticator not found.");
        bool result = await _otpAuthenticatorHelper.VerifyCode(otpAuthenticator.SecretKey, authenticatorCode);
        if (!result)
            throw new BusinessException("Authenticator code is invalid.");
    }
}
