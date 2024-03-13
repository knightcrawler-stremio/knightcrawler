namespace Producer.Tests.PTN;

public class AudioHandlersTests : PtnTestBase<AudioHandlers>
{
    [Theory]
    [InlineData("Nocturnal Animals 2016 VFF 1080p BluRay DTS HEVC-HD2", "dts")]
    [InlineData("Gold 2016 1080p BluRay DTS-HD MA 5 1 x264-HDH", "dts-hd")]
    [InlineData("Rain Man 1988 REMASTERED 1080p BRRip x264 AAC-m2g", "aac")]
    [InlineData("The Vet Life S02E01 Dunk-A-Doctor 1080p ANPL WEB-DL AAC2 0 H 264-RTN", "aac")]
    [InlineData("Jimmy Kimmel 2017 05 03 720p HDTV DD5 1 MPEG2-CTL", "dd5.1")]
    [InlineData("A Dog's Purpose 2016 BDRip 720p X265 Ac3-GANJAMAN", "ac3")]
    [InlineData("Retroactive 1997 BluRay 1080p AC-3 HEVC-d3g", "ac3")]
    [InlineData("Tempete 2016-TrueFRENCH-TVrip-H264-mp3", "mp3")]
    [InlineData("Detroit.2017.BDRip.MD.GERMAN.x264-SPECTRE", "md")]
    [InlineData("The Blacklist S07E04 (1080p AMZN WEB-DL x265 HEVC 10bit EAC-3 5.1)[Bandi]", "eac3")]
    [InlineData("Condor.S01E03.1080p.WEB-DL.x265.10bit.EAC3.6.0-Qman[UTR].mkv", "eac3")]
    [InlineData("The 13 Ghosts of Scooby-Doo (1985) S01 (1080p AMZN Webrip x265 10bit EAC-3 2.0 - Frys) [TAoE]", "eac3")]
    [InlineData("[Thund3r3mp3ror] Attack on Titan - 23.mp4", null)]
    [InlineData("Buttobi!! CPU - 02 (DVDRip 720x480p x265 HEVC AC3x2 2.0x2)(Dual Audio)[sxales].mkv", "2.0")]
    [InlineData("[naiyas] Fate Stay Night - Unlimited Blade Works Movie [BD 1080P HEVC10 QAACx2 Dual Audio]", "aac")]
    [InlineData("Sakura Wars the Movie (2001) (BDRip 1920x1036p x265 HEVC FLACx2, AC3 2.0+5.1x2)(Dual Audio)[sxales].mkv", "2.0")]
    [InlineData("Macross ~ Do You Remember Love (1984) (BDRip 1920x1036p x265 HEVC DTS-HD MA, FLAC, AC3x2 5.1+2.0x3)(Dual Audio)[sxales].mkv", "2.0")]
    [InlineData("Escaflowne (2000) (BDRip 1896x1048p x265 HEVC TrueHD, FLACx3, AC3 5.1x2+2.0x3)(Triple Audio)[sxales].mkv", "2.0")]
    [InlineData("[SAD] Inuyasha - The Movie 4 - Fire on the Mystic Island [BD 1920x1036 HEVC10 FLAC2.0x2] [84E9A4A1].mkv", "flac")]
    [InlineData("Outlaw Star - 23 (BDRip 1440x1080p x265 HEVC AC3, FLACx2 2.0x3)(Dual Audio)[sxales].mkv", "2.0")]
    [InlineData("Spider-Man.No.Way.Home.2021.2160p.BluRay.REMUX.HEVC.TrueHD.7.1.Atmos-FraMeSToR", "7.1 Atmos")]
    public void ShouldDetectAudioCorrectly(string releaseName, string expectedAudio)
    {
        var result = Sut.Parse(releaseName);
        Assert.Equal(expectedAudio, result.Audio);
    }
}