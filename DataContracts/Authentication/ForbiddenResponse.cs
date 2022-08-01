
namespace DataContracts.Authentication
{
    public class ForbiddenResponse
    {
        public ForbiddenReason ForbiddenReason { get; set; }
    }

    public enum ForbiddenReason
    {
        MaxConcurrentLoginsReached,
        SubscriptionExpired,
        EmailNotVerified,
        SubscriptionNotFound,
        SubscriptionCancelled,
        NoSubscription
    };
}
