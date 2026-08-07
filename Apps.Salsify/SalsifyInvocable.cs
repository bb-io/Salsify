using Apps.Salsify.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Salsify;

public class SalsifyInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected SalsifyClient Client { get; }
    public SalsifyInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds);
    }
}