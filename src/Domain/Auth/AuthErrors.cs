using SharedKernel;

namespace Domain.Auth;

public static class AuthErrors
{
    public static readonly Error InvalidCredential = Error.Unauthorized(
        "Auth.InvalidCredential",
        "The provided credentials are invalid");
    
    public static readonly Error IncorrectPassword = Error.Validation(
        "Auth.IncorrectPassword",
        "The password provided is incorrect.");
    
    public static readonly Error InvalidRefreshToken = Error.Validation(
        "Auth.InvalidRefreshToken",
        "The provided refresh token is invalid.");
    
    public static readonly Error ExpiredRefreshToken = Error.Validation(
        "Auth.ExpiredRefreshToken",
        "The provided refresh token has expired.");

    public static readonly Error InvalidEmailVerificationToken = Error.Validation(
        "Auth.InvalidEmailVerificationToken",
        "The provided email verification token is invalid.");
    
    public static readonly Error ExpiredEmailVerificationToken = Error.Conflict(
        "Auth.ExpiredEmailVerificationToken",
        "The provided email verification token has expired.");
}
