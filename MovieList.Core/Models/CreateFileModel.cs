namespace MovieList.Models
{
    public class CreateFileModel
    {
        public CreateFileModel(string file, string listName)
        {
            this.File = file;
            this.ListName = listName;
        }

        public string File { get; }
        public string ListName { get; }
    }
}
