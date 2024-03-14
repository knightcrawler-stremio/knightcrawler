namespace Producer.Tests.PTN;

public class DubbedHandlerTests : PtnTestBase<DubbedHandlers>
{
    [Theory]
    [InlineData("Yo-Kai Watch S01E71 DUBBED 720p HDTV x264-W4F", true)]
    [InlineData("[Golumpa] Kochoki - 11 (Kochoki - Wakaki Nobunaga) [English Dub] [FuniDub 720p x264 AAC] [MKV] [4FA0D898]", true)]
    [InlineData("[Aomori-Raws] Juushinki Pandora (01-13) [Dubs & Subs]", true)]
    [InlineData("[LostYears] Tsuredure Children (WEB 720p Hi10 AAC) [Dual-Audio]", true)]
    [InlineData("[DB] Gamers! [Dual Audio 10bit 720p][HEVC-x265]", true)]
    [InlineData("[DragsterPS] Yu-Gi-Oh! S02 [480p] [Multi-Audio] [Multi-Subs]", true)]
    [InlineData("A Freira (2018) Dublado HD-TS 720p", true)]
    [InlineData("Toy.Story.1080p.BluRay.x264-HD[Dubbing PL].mkv", true)]
    [InlineData("Fame (1980) [DVDRip][Dual][Ac3][Eng-Spa]", true)]
    [InlineData("[Hakata Ramen] Hoshiai No Sora (Stars Align) 01 [1080p][HEVC][x265][10bit][Dual-Subs] HR-DR", false)]
    [InlineData("[IceBlue] Naruto (Season 01) - [Multi-Dub][Multi-Sub][HEVC 10Bits] 800p BD", true)]
    public void ShouldDetectDubbedCorrectly(string releaseName, bool expectedDubbed)
    {
        var result = Sut.Parse(releaseName);
        Assert.Equal(expectedDubbed, result.Dubbed);
    }
}