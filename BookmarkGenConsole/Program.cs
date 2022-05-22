using System.Globalization;
using BookmarkGen;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.CommandLine;
using System.IO;
using BookmarkGenConsole;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

var tokenOption = new Argument<string>(
    "--token",
    description: "Your GitHub Personal Access Token"
);

var widthOption = new Option<int>(
    "--width",
    getDefaultValue: () => 200,
    description: "The width in mm of the bookmark to generate"
);

var heightOption = new Option<int>(
    "--height",
    getDefaultValue: () => 40,
    description: "The height in mm of the bookmark to generate"
);

var minHeightOption = new Option<int>(
    "--min-height",
    getDefaultValue: () => 20,
    description: "The minimum height of the lowest contribution bar"
);

var cornerRadiusOption = new Option<int>(
    "--corner-radius",
    getDefaultValue: () => 10,
    description: "The corner radius to use in the generated bookmark. Must be greater than or equal to half the min-height."
);

var usernameOption = new Argument<string>(
    "--username",
    description: "The username whose contributions should be graphed"
);

var fromOption = new Option<DateTime>(
    "--from",
    getDefaultValue: () => DateTime.UtcNow.Date.AddYears(-1),
    description: "The date to begin collecting contributions from"
);

var toOption = new Option<DateTime>(
    "--to",
    getDefaultValue: () => DateTime.UtcNow.Date,
    description: "The date to end collecting contributions from"
);

var outputOption = new Option<string>(
    "--output",
    getDefaultValue: () => "contributions.svg",
    description: "The output file path for the contributions bookmark SVG"
);

var labelOption = new Option<string>(
    "--label",
    getDefaultValue: () => "",
    description: "The label to display on the right hand side of the bookmark"
);

var rootCommand = new RootCommand
{
    tokenOption,
    widthOption,
    heightOption,
    minHeightOption,
    cornerRadiusOption,
    usernameOption,
    outputOption,
    fromOption,
    toOption,
    labelOption
};

rootCommand.Description = "Generates an SVG bookmark from GitHub contributions a user has made";

rootCommand.SetHandler(async (
    string token, 
    int width,
    int height,
    int minHeight,
    int cornerRadius,
    string username,
    string output,
    DateTime from,
    DateTime to,
    string labelText) => {
    var generator = new SvgGenerator(width, minHeight, height, cornerRadius, username, labelText);

    var githubEndpoint = "https://api.github.com/graphql";
    var toString = JsonSerializer.Serialize(to);
    var fromString = JsonSerializer.Serialize(from);

    var graphQLClient = new GraphQLHttpClient(githubEndpoint, new NewtonsoftJsonSerializer());
    graphQLClient.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
    var query = new GraphQLRequest
    {
    Query = $@"
    query {{ 
    user(login: ""{username}"") {{
        email
        createdAt
        contributionsCollection(
        from: {fromString}
        to: {toString}
        ) {{
        contributionCalendar {{
            totalContributions
            weeks {{
            contributionDays {{
                weekday
                date 
                contributionCount
            }}
            }}
            months  {{
            name
            year
            firstDay 
            totalWeeks
            }}
        }}
        }}
    }}
    }}
    "
    };

    var response = await graphQLClient.SendQueryAsync<GitHubResponseData>(query);

    var contributions =
    response.Data.user.contributionsCollection.contributionCalendar.weeks.Select(week =>
        (float)week.contributionDays.Sum(day => day.contributionCount))
        .ToArray();

    File.WriteAllText(output, generator.GenerateBookmark(contributions));
}, tokenOption, widthOption, heightOption, minHeightOption, cornerRadiusOption, usernameOption, outputOption, fromOption, toOption, labelOption);

return await rootCommand.InvokeAsync(args);