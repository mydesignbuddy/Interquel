using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Document;
using Raven.Json.Linq;

namespace Interquel.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            RavenDbSample();
            Console.Read();
        }

        public static void RavenDbSample()
        {
            var documentStore = new DocumentStore { Url = "http://localhost:8080/" };
            documentStore.Initialize();

            // Saving changes using the session API
            using (IDocumentSession session = documentStore.OpenSession())
            {
                // Operations against session

                string[] availableDatabases = session.Advanced.DatabaseCommands.GetDatabaseNames(100);
                Console.WriteLine("Available Databases:");
                Console.WriteLine("\tDefault"); // default db is not in the array
                IDatabaseCommands databaseCommandsDefault = session.Advanced.DatabaseCommands.ForDatabase("Default");
                foreach (string indexName in databaseCommandsDefault.GetIndexNames(0, 100))
                {
                    Console.WriteLine("\t\t{0}", indexName);
                }

                foreach (string availableDatabase in availableDatabases)
                {
                    Console.WriteLine("\t{0}", availableDatabase);
                    IDatabaseCommands databaseCommands = session.Advanced.DatabaseCommands.ForDatabase(availableDatabase);
                    foreach (string indexName in databaseCommands.GetIndexNames(0, 100))
                    {
                        Console.WriteLine("\t\t{0}", indexName);
                    }
                    
                }

                // Creating a new instance of the BlogPost class
                BlogPost post = new BlogPost()
                {
                    Title = "Hello RavenDB",
                    Category = "RavenDB",
                    Content = "This is a blog about RavenDB",
                    Comments = new BlogComment[]
                                    {
                                        new BlogComment() {Title = "Unrealistic", Content = "This example is unrealistic"},
                                        new BlogComment() {Title = "Nice", Content = "This example is nice"}
 
                                    }
                };

                // Saving the new instance to RavenDB
                session.Store(post);
                session.SaveChanges();

                Console.WriteLine("BlogPost ID: {0}", post.Id);

                // blogposts/1 is entity of type BlogPost with Id of 1
                BlogPost existingBlogPost = session.Load<BlogPost>(post.Id);

                existingBlogPost.Title = "new title for new document";

                string docUrl = session.Advanced.GetDocumentUrl(existingBlogPost);
                RavenJObject metaData = session.Advanced.GetMetadataFor(existingBlogPost);
                // Get the last modified time stamp, which is known to be of type DateTime
                DateTime lastModified = metaData.Value<DateTime>("Last-Modified");

                JsonDocument jsonDoc = session.Advanced.DatabaseCommands.Get(post.Id);
                RavenJObject json = jsonDoc.ToJson();

                Console.WriteLine("DocUrl: {0}", docUrl);
                Console.WriteLine("MetaData: {0}", metaData.ToString());
                Console.WriteLine("Last Modified Date: {0}", lastModified);
                Console.WriteLine("JSON: {0}", json.ToString());


                session.SaveChanges();

                Thread.Sleep(3000);
                Console.WriteLine("Delete...");

                session.Delete(existingBlogPost);
                session.SaveChanges();

                // Flush those changes
                session.SaveChanges();
            }
        }
    }
    public class BlogPost
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
        public DateTime PublishedAt { get; set; }
        public string[] Tags { get; set; }
        public BlogComment[] Comments { get; set; }
    }

    public class BlogComment
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
