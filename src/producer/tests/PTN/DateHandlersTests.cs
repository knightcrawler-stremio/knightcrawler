namespace Producer.Tests.PTN;

public class DateHandlersTests : PtnTestBase<DateHandlers>
{
    [Theory]
    [InlineData("Stephen Colbert 2019 10 25 Eddie Murphy 480p x264-mSD [eztv]", "2019-10-25")]
    [InlineData("Stephen Colbert 25/10/2019 Eddie Murphy 480p x264-mSD [eztv]", "2019-10-25")]
    [InlineData("Jimmy.Fallon.2020.02.14.Steve.Buscemi.WEB.x264-XLF[TGx]", "2020-02-14")]
    [InlineData("The Young And The Restless - S43 E10986 - 2016-08-12", "2016-08-12")]
    [InlineData("Indias Best Dramebaaz 2 Ep 19 (13 Feb 2016) HDTV x264-AquoTube", "2016-02-13")]
    [InlineData("07 2015 YR/YR 07-06-15.mp4", "2015-06-07")]
    [InlineData("SIX.S01E05.400p.229mb.hdtv.x264-][ Collateral ][ 16-Feb-2017 mp4", "2017-02-16")]
    [InlineData("SIX.S01E05.400p.229mb.hdtv.x264-][ Collateral ][ 16-Feb-17 mp4", "2017-02-16")]
    [InlineData("WWE Smackdown - 11/21/17 - 21st November 2017 - Full Show", "2017-11-21")]
    public void ShouldDetectDateCorrectly(string releaseName, string expectedCodec)
    {
        var result = Sut.Parse(releaseName);
        Assert.Equal(expectedCodec, result.Date);
    }
}