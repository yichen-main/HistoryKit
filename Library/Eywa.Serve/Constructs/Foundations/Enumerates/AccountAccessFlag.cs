namespace Eywa.Serve.Constructs.Foundations.Enumerates;
public enum AccountAccessFlag
{
    IncorrectUserRole,
    UserRoleNotEmpty,

    IncorrectAccessPermissions,
    IncorrectModulePermissions,

    TimeIntervalNotEmpty,
    TimeIntervalQueryFormatIncorrect,
    StartTimeFormatError,
    EndTimeFormatError,
    StartTimeMismatchEndTime,
}