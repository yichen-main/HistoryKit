namespace Eywa.Serve.Constructs.Foundations.Enumerates;
public enum EnterpriseIntegrationFlag
{
    CarrierWrongPassword,
    CarrierPasswordIsRequired,
    CarrierSystemAccountIsRequired,
    CarrierSystemAccountAlreadyExists,
    CarrierSystemAccountNotSet,

    CarrierUserIdIsRequired,
    CarrierUserNoIsRequired,
    CarrierUserNoIndex,
    CarrierUserNameIsRequired,
    CarrierEmailIsRequired,
    CarrierEmailFormatError,
    CarrierEmailIndex,
    CarrierSignInFailed,
    CarrierTokenInvalid,
    CarrierUserIdDoesNotExist,

    CarrierAccountInvalid,
    CarrierAccountIsRequired,
    CarrierAccountDoesNotExist,

    CarrierNameIdentifierIsRequired,
    CarrierRefreshTokenIsRequired,
    CarrierRolePolicyMismatch,
    CarrierAccessTypeMismatch,
    CarrierNoPermissionToCreateAdministrator,

    CarrierModuleIdIsRequired,
    CarrierModuleIdDoesNotExist,
    CarrierModuleUserAlreadyExists,
    CarrierFieldTypeExisted,
    CarrierFieldTypeMismatch,

    LinkPositionIdIsRequired,
    LinkPositionIdDoesNotExist,
}