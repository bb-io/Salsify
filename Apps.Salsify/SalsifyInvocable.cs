using Apps.Salsify.Api;
using Apps.Salsify.Constants;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;

namespace Apps.Salsify;

public class SalsifyInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds => InvocationContext.AuthenticationCredentialsProviders.ToArray();
    protected string OrgId => Creds.Get(CredsNames.OrgId).Value;

    protected SalsifyClient Client { get; }
    protected ExternalRestClient ExternalClient { get; }

    protected SalsifyInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new(Creds);
        ExternalClient = new();
    }
}