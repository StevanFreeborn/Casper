namespace Casper.Console.Edit;

internal sealed class Feedback : Exception
{
  public BlogPost OriginalBlogPost { get; init; } = new BlogPost(
    string.Empty,
    string.Empty,
    string.Empty,
    string.Empty
  );

  public Comment[] Comments { get; init; } = [];

  public Feedback()
  {
  }

  public Feedback(BlogPost originalBlogPost, Comment[] comments)
  {
    OriginalBlogPost = originalBlogPost;
    Comments = comments;
  }

  public Feedback(string message) : base(message)
  {
  }

  public Feedback(string message, Exception innerException) : base(message, innerException)
  {
  }

  public override string ToString()
  {
    var sb = new StringBuilder();

    sb.AppendLine("[Original Blog Post]");
    sb.AppendLine(OriginalBlogPost.ToMarkdown());
    sb.AppendLine();

    sb.AppendLine("[Feedback]");
    sb.AppendLine();

    foreach (var (c, i) in Comments.Select((v, i) => (v, i + 1)))
    {
      sb.AppendLine(CultureInfo.InvariantCulture, $"Comment {i}:");
      sb.AppendLine(CultureInfo.InvariantCulture, $"- Original Snippet: {c.OriginalSnippet}");
      sb.AppendLine(CultureInfo.InvariantCulture, $"- Issue: {c.Issue}");
      sb.AppendLine(CultureInfo.InvariantCulture, $"- Suggestion: {c.Suggestion}");
    }

    return sb.ToString();
  }
}