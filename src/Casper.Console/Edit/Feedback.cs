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
}