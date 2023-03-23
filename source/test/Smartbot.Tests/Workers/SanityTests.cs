using System.Linq;
using System.Threading.Tasks;
using Smartbot.Utilities.Handlers.Sanity;
using Xunit;

namespace Smartbot.Tests.Workers;

public class SanityTests
{
    [Fact]
    public async Task FetchMedia()
    {
        var sanity = new SanityHelper();
        var media = await sanity.GetMoviesAndSeries();
        Assert.NotEmpty(media);
        var first = media.First();
        Assert.Equal("movie", first.SanityType );
        Assert.Equal("The Darjeeling Limited", first.Title );
        Assert.Equal("A year after their father's funeral, three brothers travel across India by train in an attempt to bond with each other.\n\n", first.Description );
        Assert.Equal("https://www.imdb.com/title/tt0838221/", first.IMDBUrl );
        Assert.Equal("2018", first.Year );
        Assert.Equal("Samme regissør som Budapest Hotel. Meganiz. French dispatch er en herlig surrealistisk affære (som kan være litt langdryg)", first.Quotes.First().Text );
        Assert.Equal("ef", first.Quotes.First().Author.Nickname );
        
    }
}
