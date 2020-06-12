namespace Interlex.App.ViewModels
{
    using System.Collections.Generic;
    using Interlex.BusinessLayer.Models.Folders;

    public class UserFolderDataJson
    {
        public int key { get; set; }
        public string title { get; set; }
        public bool folder { get; set; } = true; // needed by fancytree
        public bool lazy { get; set; } = true; // for fancytree lazy loading (if folder has child folders)
        public int documentsCount { get; set; } = 0;

        public UserFolderDataJson()
        {
        }

        public UserFolderDataJson(UserFolderData ufd)
        {
            this.key = ufd.Id;
            this.title = ufd.Title;
            if(ufd.IsEmptyFolder)
            {
                this.lazy = false;
            }

            this.documentsCount = ufd.DocumentsCount;
        }

        public UserFolderDataJson(int key, string title)
        {
            this.key = key;
            this.title = title;
        }

        public static IEnumerable<UserFolderDataJson> FromData(IEnumerable<UserFolderData> ufData)
        {
            var jsonData = new List<UserFolderDataJson>();
            foreach (var userFolderData in ufData)
            {
                var data = new UserFolderDataJson(userFolderData);
                if(userFolderData.IsEmptyFolder)
                {
                    data.lazy = false;
                }

                jsonData.Add(data);
            }

            return jsonData;
        }
    }
}
