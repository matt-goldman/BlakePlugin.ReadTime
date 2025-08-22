using Blake.BuildTools;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace BlakePlugin.ReadTime;

public class Plugin : IBlakePlugin
{
    private const int WORDS_CORRECT_PER_MINUTE = 200;

    // TODO: Blake plugins currently don't allow modifying the input markdown pages directly. So we can't use before bake.
    //  Instead, we will use the AfterBakeAsync method and add the read time to the metadata, instead of the front matter.
    //  Need to consider whether plugin architecture should allow modifying the input markdown pages directly.
    public Task AfterBakeAsync(BlakeContext context, ILogger? logger = null)
    {
        logger?.LogDebug("BeforeBakeAsync called in ReadTime plugin.");

        int wpm = WORDS_CORRECT_PER_MINUTE;

        // check arguments for readtime:wpm
        if (context.Arguments.Any(arg => arg.StartsWith("--readtime:wpm=", StringComparison.OrdinalIgnoreCase)))
        {
            var wpmArg = context.Arguments.FirstOrDefault(arg => arg.StartsWith("--readtime:wpm=", StringComparison.OrdinalIgnoreCase));
            if (wpmArg != null)
            {
                var wpmValue = wpmArg.Split('=')[1];
                if (int.TryParse(wpmValue, out var wpmVal))
                {
                    wpm = wpmVal;
                }
                else
                {
                    logger?.LogWarning("Invalid read time words per minute value: {value}. Using default {defaultWPM}.", wpmValue, WORDS_CORRECT_PER_MINUTE);
                }
            }
        }

        foreach (var page in context.GeneratedPages)
        {
            var mdPage = context.MarkdownPages.FirstOrDefault(p => p.Slug == page.Page.Slug);

            if (mdPage == null)
            {
                logger?.LogWarning("Markdown page not found for slug: {slug}", page.Page.Slug);
                continue;
            }

            var wordcount = Regex.Matches(mdPage.RawMarkdown, @"\b\w+\b").Count;
            var readTime = (int)Math.Ceiling(wordcount / (double)wpm);

            // add read time to metadata
            page.Page.Metadata["readTimeMinutes"] = readTime.ToString();
        }

        return Task.CompletedTask;
    }
}
