namespace Producer.Tests.PTN;

public class EpisodeCodeHandlerTests : PtnTestBase<EpisodeCodeHandlers>
{
    
    [Theory]
    [InlineData("[Golumpa] Fairy Tail - 214 [FuniDub 720p x264 AAC] [5E46AC39].mkv", "5E46AC39")]
    [InlineData("[Exiled-Destiny]_Tokyo_Underground_Ep02v2_(41858470).mkv", "41858470")]
    [InlineData("[ACX]El_Cazador_de_la_Bruja_-_19_-_A_Man_Who_Protects_[SSJ_Saiyan_Elite]_[9E199846].mkv", "9E199846")]
    [InlineData("[CBM]_Medaka_Box_-_11_-_This_Is_the_End!!_[720p]_[436E0E90]", "436E0E90")]
    [InlineData("Gankutsuou.-.The.Count.Of.Monte.Cristo[2005].-.04.-.[720p.BD.HEVC.x265].[FLAC].[Jd].[DHD].[b6e6e648].mkv", "B6E6E648")]
    [InlineData("[D0ugyB0y] Nanatsu no Taizai Fundo no Shinpan - 01 (1080p WEB NF x264 AAC[9CC04E06]).mkv", "9CC04E06")]
    [InlineData("Lost.[Perdidos].6x05.HDTV.XviD.[www.DivxTotaL.com].avi", null)]
    public void ShouldDetectEpisodeCodeCorrectly(string releaseName, string expectedEpisodeCode)
    {
        var result = Sut.Parse(releaseName);
        Assert.Equal(expectedEpisodeCode, result.EpisodeCode);
    }
}