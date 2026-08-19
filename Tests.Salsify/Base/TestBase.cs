using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Extensions.Configuration;
using Tests.Salsify.Base;

public class TestBase
{
    public static List<IEnumerable<AuthenticationCredentialsProvider>> CredentialGroups { get; private set; }
    
    public static List<InvocationContext> InvocationContexts { get; private set; }
    
    public TestContext? TestContext { get; set; }

    public FileManager FileManager { get; set; }

    static TestBase()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        
        CredentialGroups = config.GetSection("ConnectionDefinition")
            .GetChildren()
            .Select(section =>
                section.GetChildren()
                    .Select(child => new AuthenticationCredentialsProvider(child.Key, child.Value ?? string.Empty))
            )
            .ToList();

        InvocationContexts = CredentialGroups.Select(group => new InvocationContext
        {
            AuthenticationCredentialsProviders = group
        }).ToList();
    }
    
    public TestBase()
    {
        FileManager = new FileManager();
    }
    
    public InvocationContext GetInvocationContext(string connectionType)
    {
        var context = InvocationContexts.FirstOrDefault(x => 
            x.AuthenticationCredentialsProviders.Any(y => y.Value == connectionType));
            
        return context ?? throw new Exception($"Invocation context not found for: {connectionType}");
    }
}