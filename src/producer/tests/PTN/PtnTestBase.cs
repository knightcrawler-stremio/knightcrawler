namespace Producer.Tests.PTN;

public abstract class PtnTestBase<THandler> where THandler : class, IPtnHandler
{
    protected IParseTorrentName? Sut;

    protected PtnTestBase() => Sut = GetPtnWithSUT();

    private IParseTorrentName GetPtnWithSUT()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IPtnHandler, THandler>();
        serviceCollection.AddSingleton<IParseTorrentName, ParseTorrentName>();
        
        return serviceCollection.BuildServiceProvider().GetService<IParseTorrentName>();
    }
}