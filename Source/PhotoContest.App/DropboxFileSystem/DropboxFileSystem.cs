namespace PhotoContest.App.DropboxFileSystem
{
    using System;
    using System.Threading.Tasks;
    using Dropbox.Api;
    using System.Linq;
    using System.IO;
    using System.Text;

    public class DropboxFileSystem
    {
        public DropboxClient dropboxClient;

        public DropboxFileSystem()
        {
            this.dropboxClient = new DropboxClient("dropbox-access-token");
        }
        
        public async Task ListRootFolder(DropboxClient dbx)
        {
            var list = await dbx.Files.ListFolderAsync(string.Empty);

            // show folders, then files
            foreach (var item in list.Entries.Where(i => i.IsFolder))
            {
                Console.WriteLine("D  {0}/", item.Name);
            }

            foreach (var item in list.Entries.Where(i => i.IsFile))
            {
                Console.WriteLine("F{0,8} {1}", item.AsFile.Size, item.Name);
            }
        }

        public async Task<string> Download(string folder, string file)
        {
            using (var response = await dropboxClient.Files.DownloadAsync(folder + "/" + file))
            {
                return(await response.GetContentAsStringAsync());
            }
        }

        public async Task Upload(DropboxClient dbx, string folder, string file, string content)
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var updated = await dbx.Files.UploadAsync(
                    folder + "/" + file,
                    Dropbox.Api.Files.WriteMode.Overwrite.Instance,
                    body: mem);
                Console.WriteLine("Saved {0}/{1} rev {2}", folder, file, updated.Rev);
            }
        }
    }
}