namespace Producer.Tests.PTN;

public class CodecHandlerTests : PtnTestBase<CodecHandlers>
{
    [Theory]
    [InlineData("Nocturnal Animals 2016 VFF 1080p BluRay DTS HEVC-HD2", "hevc")]
    [InlineData("doctor_who_2005.8x12.death_in_heaven.720p_hdtv_x264-fov", "x264")]
    [InlineData("The Vet Life S02E01 Dunk-A-Doctor 1080p ANPL WEB-DL AAC2 0 H 264-RTN", "h264")]
    [InlineData("Gotham S03E17 XviD-AFG", "xvid")]
    [InlineData("Jimmy Kimmel 2017 05 03 720p HDTV DD5 1 MPEG2-CTL", "mpeg2")]
    [InlineData("[Anime Time] Re Zero kara Hajimeru Isekai Seikatsu (Season 2 Part 1) [1080p][HEVC10bit x265][Multi Sub]", "x265")]
    [InlineData("[naiyas] Fate Stay Night - Unlimited Blade Works Movie [BD 1080P HEVC10 QAACx2 Dual Audio]", "hevc")]
    [InlineData("[DB]_Bleach_264_[012073FE].avi", null)]
    [InlineData("[DB]_Bleach_265_[B4A04EC9].avi", null)]
    public void ShouldDetectCodecCorrectly(string releaseName, string expectedCodec)
    {
        var result = Sut.Parse(releaseName);
        Assert.Equal(expectedCodec, result.Codec);
    }
}