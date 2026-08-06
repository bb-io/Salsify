using Apps.Salsify.Connections;
using Apps.Salsify.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Tests.Salsify.Base;

namespace Tests.Salsify;

[TestClass]
public class ConnectionValidatorTests : TestBaseMultipleConnections
{
    [TestMethod, TargetConnections]
    public async Task ValidateConnection_ValidData_ShouldBeSuccessful(InvocationContext context)
    {
        var validator = new ConnectionValidator(context);

        var result = await validator.ValidateConnection(context.AuthenticationCredentialsProviders, CancellationToken.None);
        Console.WriteLine(result.Message);
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod, TargetConnections]
    public async Task ValidateConnection_InvalidData_ShouldFail(InvocationContext context)
    {
        var validator = new ConnectionValidator(context);
        var newCredentials = context.AuthenticationCredentialsProviders
            .Select(x => new AuthenticationCredentialsProvider(
                x.KeyName,
                x.KeyName is CredsNames.ConnectionType or CredsNames.OrgId ? x.Value : x.Value + "_incorrect"));

        var result = await validator.ValidateConnection(newCredentials, CancellationToken.None);
        Console.WriteLine(result.Message);
        Assert.IsFalse(result.IsValid);
    }
}