namespace Producer.Tests.PTN;

public class HdrHandlerTests : PtnTestBase<HdrHandlers>
{
    [Theory]
    [InlineData("The.Mandalorian.S01E06.4K.HDR.2160p 4.42GB", new string[] { "HDR" })]
    [InlineData("Spider-Man - Complete Movie Collection (2002-2022) 1080p.HEVC.HDR10.1920x800.x265. DTS-HD", new string[] { "HDR" })]
    [InlineData("Bullet.Train.2022.2160p.AMZN.WEB-DL.x265.10bit.HDR10Plus.DDP5.1-SMURF", new string[] { "HDR10+" })]
    [InlineData("Belle (2021) 2160p 10bit 4KLight DOLBY VISION BluRay DDP 7.1 x265-QTZ", new string[] { "DV" })]
    [InlineData("Андор / Andor [01x01-03 из 12] (2022) WEB-DL-HEVC 2160p | 4K | Dolby Vision TV | NewComers, HDRezka Studio", new string[] { "DV" })]
    [InlineData("АBullet.Train.2022.2160p.WEB-DL.DDP5.1.DV.MKV.x265-NOGRP", new string[] { "DV" })]
    [InlineData("Bullet.Train.2022.2160p.WEB-DL.DoVi.DD5.1.HEVC-EVO[TGx]", new string[] { "DV" })]
    [InlineData("Спайдерхед / Spiderhead (2022) WEB-DL-HEVC 2160p | 4K | HDR | Dolby Vision Profile 8 | P | NewComers, Jaskier", new string[] { "DV", "HDR" })]
    [InlineData("House.of.the.Dragon.S01E07.2160p.10bit.HDR.DV.WEBRip.6CH.x265.HEVC-PSA", new string[] { "DV", "HDR" })]
    [InlineData("Флешбэк / Memory (2022) WEB-DL-HEVC 2160p | 4K | HDR | HDR10+ | Dolby Vision Profile 8 | Pazl Voice", new string[] { "DV", "HDR10+", "HDR" })]
    public void ShouldDetectHdrSourceCorrectly(string releaseName, string[] expectedHdr)
    {
        var result = Sut.Parse(releaseName);
        Assert.Equal(expectedHdr, result.Hdr);
    }
}