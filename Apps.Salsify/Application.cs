using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Salsify;

public class Application : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.ECommerce];
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}