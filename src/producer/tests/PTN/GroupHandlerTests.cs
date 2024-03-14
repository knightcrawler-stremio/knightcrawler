namespace Producer.Tests.PTN;

public class GroupHandlerTests : PtnTestBase<GroupHandlers>
{
    [Theory]
    [InlineData("Nocturnal Animals 2016 VFF 1080p BluRay DTS HEVC-HD2", "HD2")]
    [InlineData("Gold 2016 1080p BluRay DTS-HD MA 5 1 x264-HDH", "HDH")]
    [InlineData("Hercules (2014) 1080p BrRip H264 - YIFY", "YIFY")]
    [InlineData("The.Expanse.S05E02.720p.WEB.x264-Worldmkv.mkv", "Worldmkv")]
    [InlineData("The.Expanse.S05E02.PROPER.720p.WEB.h264-KOGi[rartv]", "KOGi")]
    [InlineData("The.Expanse.S05E02.1080p.AMZN.WEB.DDP5.1.x264-NTb[eztv.re].mp4", "NTb")]
    [InlineData("Western - L'homme qui n'a pas d'étoile-1955.Multi.DVD9", null)]
    [InlineData("Power (2014) - S02E03.mp4", null)]
    [InlineData("Power (2014) - S02E03", null)]
    [InlineData("3-Nen D-Gumi Glass no Kamen - 13", null)]
    [InlineData("3-Nen D-Gumi Glass no Kamen - Ep13", null)]
    [InlineData("[AnimeRG] One Punch Man - 09 [720p].mkv", "AnimeRG")]
    [InlineData("[Mazui]_Hyouka_-_03_[DF5E813A].mkv", "Mazui")]
    [InlineData("[H3] Hunter x Hunter - 38 [1280x720] [x264]", "H3")]
    [InlineData("[KNK E MMS Fansubs] Nisekoi - 20 Final [PT-BR].mkv", "KNK E MMS Fansubs")]
    [InlineData("[ToonsHub] JUJUTSU KAISEN - S02E01 (Japanese 2160p x264 AAC) [Multi-Subs].mkv", "ToonsHub")]
    [InlineData("[HD-ELITE.NET] -  The.Art.Of.The.Steal.2014.DVDRip.XviD.Dual.Aud", "HD-ELITE.NET")]
    [InlineData("[Russ]Lords.Of.London.2014.XviD.H264.AC3-BladeBDP", "BladeBDP")]
    [InlineData("Jujutsu Kaisen S02E01 2160p WEB H.265 AAC -Tsundere-Raws (B-Global).mkv", "B-Global")]
    [InlineData("[DVD-RIP] Kaavalan (2011) Sruthi XVID [700Mb] [TCHellRaiser]", null)]
    [InlineData("[DvdMux - XviD - Ita Mp3 Eng Ac3 - Sub Ita Eng] Sanctuary S01e01", null)]
    [InlineData("the-x-files-502.mkv", null)]
    public void ShouldDetectGroupCorrectly(string releaseName, string expectedGroup)
    {
        var result = Sut.Parse(releaseName);
        Assert.Equal(expectedGroup, result.Group);
    }
}