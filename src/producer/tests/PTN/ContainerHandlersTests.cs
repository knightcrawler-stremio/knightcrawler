namespace Producer.Tests.PTN;

public class ContainerHandlersTests : PtnTestBase<ContainerHandlers>
{
    [Theory]
    [InlineData("Kevin Hart What Now (2016) 1080p BluRay x265 6ch -Dtech mkv", "mkv")]
    [InlineData("The Gorburger Show S01E05 AAC MP4-Mobile", "mp4")]
    [InlineData("[req]Night of the Lepus (1972) DVDRip XviD avi", "avi")]
    public void ShouldDetectContainerCorrectly(string releaseName, string expectedContainer)
    {
        var result = Sut.Parse(releaseName);
        Assert.Equal(expectedContainer, result.Container);
    }
}