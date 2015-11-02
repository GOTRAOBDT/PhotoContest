namespace PhotoContest.App.Dropbox
{
    using System;
    using System.Threading.Tasks;
    using Dropbox.Api;

    public class DropboxAuthorization
    {
        
    }
    class Program
    {
        static void Main(string[] args)
        {
            var task = Task.Run((Func<Task>)Program.Run);
            task.Wait();
        }

        static async Task Run()
        {
            using (var dbx = new DropboxClient("YOUR ACCESS TOKEN"))
            {
                var full = await dbx.Users.GetCurrentAccountAsync();
                Console.WriteLine("{0} - {1}", full.Name.DisplayName, full.Email);
            }
        }
    }
}