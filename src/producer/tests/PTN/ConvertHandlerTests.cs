namespace Producer.Tests.PTN;

public class ConvertHandlerTests : PtnTestBase<ConvertHandlers>
{
    [Theory]
    [InlineData("Better.Call.Saul.S03E04.CONVERT.720p.WEB.h264-TBS", true)]
    [InlineData("Have I Got News For You S53E02 EXTENDED 720p HDTV x264-QPEL", false)]
    public void ShouldDetectConvertCorrectly(string releaseName, bool expectedConvert)
    {
        var result = Sut.Parse(releaseName);
        Assert.Equal(expectedConvert, result.Convert);
    }
}