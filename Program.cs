using NLog;

string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

string? choice;
do
{
  // present the user with a menu
  Console.WriteLine("1. Display all blogs");
  Console.WriteLine("2. Add Blog");
  Console.WriteLine("3. Create Post");
  Console.WriteLine("4. Display Posts");

  // get user input
  Console.Write("Enter a number: ");
  choice = Console.ReadLine();
  logger.Info("User choice: {choice}", choice);

  var db = new DataContext();

  if (choice == "1")
  {
    // Display all Blogs from the database
    var query = db.Blogs.OrderBy(b => b.Name);

    Console.WriteLine("All blogs in the database:");
    foreach (var item in query)
    {
      Console.WriteLine(item.Name);
    }
  }
  else if (choice == "2")
  {
    // Create and save a new Blog
    Console.Write("Enter a name for a new Blog: ");
    var name = Console.ReadLine();

    var blog = new Blog { Name = name };

    db.AddBlog(blog);
    logger.Info("Blog added - {name}", name);
  
  }
} while (choice == "1" || choice == "2" || choice == "3" || choice == "4");


logger.Info("Program ended");
