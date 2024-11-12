using NLog;

string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

Console.Clear();

logger.Info("Program started");

string? choice;
do
{
  // present the user with a menu
  Console.WriteLine("1. Display all blogs");
  Console.WriteLine("2. Add Blog");
  Console.WriteLine("3. Create Post");
  Console.WriteLine("4. Display Posts");
  Console.WriteLine("Enter q to quit");

  // get user input
  Console.Write("Enter a number: ");
  choice = Console.ReadLine();
  logger.Info("User choice: {choice}", choice);

  var db = new DataContext();

  if (choice == "1")
  {
    Console.Clear();
    // Display all Blogs from the database
    var query = db.Blogs.OrderBy(b => b.Name);
    var count = query.Count();
    Console.WriteLine($"{count} blogs listed");

    Console.WriteLine("All blogs in the database:");
    foreach (var item in query)
    {
      Console.WriteLine(item.Name);
    }
  }
  else if (choice == "2")
  {
    Console.Clear();
    // Create and save a new Blog
    Console.Write("Enter a name for a new Blog: ");
    var name = Console.ReadLine();
    if (name == null || name == "")
    {
      logger.Error("Blog name cannot be blank");
      Console.WriteLine("Blog name cannot be blank");
      continue;
    }

    var blog = new Blog { Name = name };

    db.AddBlog(blog);
    logger.Info("Blog added - {name}", name);

  }
  else if (choice == "3")
  {
    Console.Clear();
    // create and save new post
    var query = db.Blogs.OrderBy(b => b.Name);

    Console.WriteLine("All blogs in the database:");
    foreach (var item in query)
    {
      Console.WriteLine(item.BlogId + " " + item.Name);
    }

    Console.Write("Select a blog to add a post to: ");
    var blogId = Console.ReadLine();
    if (blogId == null || blogId == "")
    {
      logger.Error("Blog ID cannot be blank");
      Console.WriteLine("Blog ID cannot be blank");
      continue;
    }
    else
    {
      var blog = db.Blogs.Find(int.Parse(blogId));
      if (blog == null)
      {
        logger.Error("Blog ID {blogId} not found", blogId);
        Console.WriteLine("Blog ID not found");
        continue;
      }

      Console.Write("Enter a title for the post: ");
      var title = Console.ReadLine();
      if (title == null || title == "")
      {
        logger.Error("Post title cannot be blank");
        Console.WriteLine("Post title cannot be blank");
        continue;
      }

      Console.Write("Enter the content for the post: ");
      var content = Console.ReadLine();
      if (content == null || content == "")
      {
        logger.Error("Post content cannot be blank");
        Console.WriteLine("Post content cannot be blank");
        continue;
      }

      var post = new Post { Title = title, Content = content, BlogId = blog.BlogId };
      db.Posts.Add(post);
      db.SaveChanges();
      logger.Info("Post added - {title}", title);
    }
  }
  else if (choice == "4")
  {
    Console.Clear();
    // Display Posts from all blogs or a specific blog
    while (true)
    {
      var blogs = db.Blogs.ToList();
      Console.WriteLine("Select which posts to view:");
      Console.WriteLine("1. Show all posts");

      for (int i = 0; i < blogs.Count; i++)
      {
        Console.WriteLine($"{i + 2}. {blogs[i].Name}");
      }

      Console.WriteLine("Press q to return to the main menu");

      var blogIdInput = Console.ReadLine();

      if (string.IsNullOrWhiteSpace(blogIdInput))
      {
        logger.Error("Post selection cannot be blank");
        Console.WriteLine("Post selection cannot be blank");
        continue;
      }

      if (blogIdInput.ToLower() == "q")
      {
        Console.Clear();
        break;
      }

      if (!int.TryParse(blogIdInput, out int blogId) || blogId > blogs.Count + 1)
      {
        logger.Error("Invalid post selection entered");
        Console.WriteLine("Invalid post selection entered");
        continue;
      }

      if (blogId == 1)
      {
        Console.Clear();
        // Show all posts
        var allPosts = db.Posts.ToList();
        foreach (var post in allPosts)
        {
          Console.WriteLine($"\nBlog: {post.Blog.Name} \nTitle: {post.Title} \nContent: {post.Content}\n");
        }
      }
      else
      {
        Console.Clear();
        var selectedBlog = blogs[blogId - 2];
        var posts = db.Posts.Where(p => p.BlogId == selectedBlog.BlogId).ToList();
        foreach (var post in posts)
        {
          Console.WriteLine($"\nBlog: {post.Blog.Name} \nTitle: {post.Title} \nContent: {post.Content}\n");
        }
      }
    }
  }
} while ((choice == "1" || choice == "2" || choice == "3" || choice == "4") && choice != "q");


logger.Info("Program ended");
