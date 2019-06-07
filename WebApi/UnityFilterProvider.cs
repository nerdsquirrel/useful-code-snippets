public class UnityFilterProvider : IFilterProvider
{
    private readonly IUnityContainer _container;
    private readonly ActionDescriptorFilterProvider _defaultProvider = new ActionDescriptorFilterProvider();

    public UnityFilterProvider(IUnityContainer container)
    {
        _container = container;
    }  

    public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
    {
        var attributes = _defaultProvider.GetFilters(configuration, actionDescriptor).ToList();

        foreach (var attr in attributes)
        {
            _container.BuildUp(attr.Instance.GetType(), attr.Instance);
        }
        return attributes;
    }
}