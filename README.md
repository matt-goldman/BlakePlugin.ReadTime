# BlakePlugin.ReadTime

A Blake plugin that calculates estimated reading time for content based on 200 words per minute reading speed.

## Features

- Automatically calculates reading time for all markdown pages
- Uses industry standard 200 words per minute reading speed
- Adds reading time to page metadata as `readTimeMinutes`
- Works seamlessly with Blake's build process

## Installation

Install the NuGet package:

```bash
dotnet add package BlakePlugin.ReadTime
```

## Usage

Blake automatically discovers plugins added to your project, and runs them during the build process. Once added to your project, just bake your site as usual, and the plugin will process all markdown pages to calculate reading time.

```bash
blake bake
```

The plugin will automatically process all markdown pages during the build and add a `readTimeMinutes` metadata property to each page.

## Accessing Reading Time

Each page's estimated reading time can be accessed from the page's metadata. This is accessible via the global generated content index.

```csharp
// Example in a Razor template
var readingTime = GeneratedContentIndex.GetPages().FirstOrDefault(p => p.Slug == "your-page-slug")?.Metadata["readTimeMinutes"];
```

## How It Works

The plugin:

1. Runs after the bake process (`AfterBakeAsync`)
2. Counts words in each markdown page using regex pattern `\b\w+\b`
3. Calculates reading time using 200 words per minute (approximate adult reading average)
4. Rounds up to the nearest minute
5. Adds the result to the page metadata

## License

This project is licensed under MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
