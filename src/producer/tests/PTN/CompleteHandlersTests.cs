namespace Producer.Tests.PTN;

public class CompleteHandlersTests : PtnTestBase<CompleteHandlers>
{
    [Theory]
    [InlineData("[Furi] Avatar - The Last Airbender [720p] (Full 3 Seasons + Extr", true)]
    [InlineData("Harry.Potter.Complete.Collection.2001-2011.1080p.BluRay.DTS-ETRG", true)]
    [InlineData("Game of Thrones All 7 Seasons 1080p ~∞~ .HakunaMaKoko", true)]
    [InlineData("Avatar: The Last Airbender Full Series 720p", true)]
    [InlineData("Dora the Explorer - Ultimate Collection", true)]
    [InlineData("Mr Bean Complete Pack (Animated, Tv series, 2 Movies) DVDRIP (WA", true)]
    [InlineData("American Pie - Complete set (8 movies) 720p mkv - YIFY", true)]
    [InlineData("Charlie Chaplin - Complete Filmography (87 movies)", true)]
    [InlineData("Monster High Movies Complete 2014", true)]
    [InlineData("Harry Potter All Movies Collection 2001-2011 720p Dual KartiKing", true)]
    [InlineData("The Clint Eastwood Movie Collection", true)]
    [InlineData("Clint Eastwood Collection - 15 HD Movies", true)]
    [InlineData("Official  IMDb  Top  250  Movies  Collection  6/17/2011", true)]
    [InlineData("The Texas Chainsaw Massacre Collection (1974-2017) BDRip 1080p", true)]
    [InlineData("Snabba.Cash.I-II.Duology.2010-2012.1080p.BluRay.x264.anoXmous", true)]
    [InlineData("Star Wars Original Trilogy 1977-1983 Despecialized 720p", true)]
    [InlineData("The.Wong.Kar-Wai.Quadrology.1990-2004.1080p.BluRay.x264.AAC.5.1-", true)]
    [InlineData("Lethal.Weapon.Quadrilogy.1987-1992.1080p.BluRay.x264.anoXmous", true)]
    [InlineData("X-Men.Tetralogy.BRRip.XviD.AC3.RoSubbed-playXD", true)]
    [InlineData("Mission.Impossible.Pentalogy.1996-2015.1080p.BluRay.x264.AAC.5.1", true)]
    [InlineData("Mission.Impossible.Hexalogy.1996-2018.SweSub.1080p.x264-Justiso", true)]
    [InlineData("American.Pie.Heptalogy.SWESUB.DVDRip.XviD-BaZZe", true)]
    [InlineData("The Exorcist 1, 2, 3, 4, 5 - Complete Horror Anthology 1973-2005", true)]
    [InlineData("Harry.Potter.Complete.Saga. I - VIII .1080p.Bluray.x264.anoXmous", true)]
    [InlineData("[Erai-raws] Ninja Collection - 05 [720p][Multiple Subtitle].mkv", true)]
    public void ShouldDetectCompleteCollectionCorrectly(string releaseName, bool expectedComplete)
    {
        var result = Sut.Parse(releaseName);
        Assert.Equal(expectedComplete, result.Complete);
    }
}