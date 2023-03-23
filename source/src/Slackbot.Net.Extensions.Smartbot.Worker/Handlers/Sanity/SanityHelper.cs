using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Sanity.Linq;
using Sanity.Linq.CommonTypes;

namespace Smartbot.Utilities.Handlers.Sanity;

public class SanityHelper
{
    private readonly SanityDataContext _sanity;

    public SanityHelper(IHttpClientFactory factory)
    {
        var options = new SanityOptions
        {
            ProjectId = "hs7ia5dp",
            Dataset = "production",
            UseCdn = true,
            ApiVersion = "v1"
        };

        _sanity = new SanityDataContext(options, clientFactory:factory);
    }

    private string Query =
"""
*[_type in ["series", "movie"]] {
    ...,
    quotes[] {
      ...,
      author->
    },
    streamingservice ->
} | order(_createdAt desc)
""";

    public async Task<IEnumerable<MovieOrSeries>> GetMoviesAndSeries()
    {
        var res = await _sanity.Client.FetchAsync<IEnumerable<MovieOrSeries>>(Query); 
        return res.Result;
    }
}

public class MovieOrSeries : SanityDocument
{
    public Slug Slug { get; set; }
    
    public string Title { get; set; }
    public string Description { get; set; }
    
    public string IMDBUrl { get; set; }
    
    public string StreamUrl { get; set; }

    public IEnumerable<Quote> Quotes { get; set; } = Enumerable.Empty<Quote>();
    public string Year { get; set; }
    
    public StreamingService StreamingService { get; set; }
}

public class StreamingService
{
    public string Name { get; set; }
}

public class Quote
{
    public Author Author { get; set; }
    public string Text { get; set; }
}

public class Author
{
    public string Nickname { get; set; }
}

public class Slug
{
    public string Current { get; set; }
}
