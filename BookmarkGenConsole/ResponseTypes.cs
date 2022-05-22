namespace BookmarkGenConsole;

public class GitHubResponse
{
    public GitHubResponseData data { get; set; }
}

public class GitHubResponseData
{
    public User user { get; set; }
}

public class User
{
    public string email { get; set; }
    public DateTime createdAt { get; set; }
    public ContributionsCollection contributionsCollection { get; set; }
}

public class ContributionsCollection
{
    public ContributionCalendar contributionCalendar { get; set; }
}

public class ContributionCalendar
{
    public int totalContributions { get; set; }
    public WeeklyContribution[] weeks { get; set; }
}

public class WeeklyContribution
{
    public ContributionDay[] contributionDays { get; set; }
}

public class ContributionDay
{
    public DayOfWeek weekday { get; set; }
    public DateTime date { get; set; }
    public int contributionCount { get; set; }
}