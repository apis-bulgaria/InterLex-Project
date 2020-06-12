using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace EUCases.ConsumerCasesWebApp.BusinessLayer.Entities
{
    public class SubjectMatterMock : ISubjectMatter
    {
        private List<FolderData> _lFolderData;

        public SubjectMatterMock()
        {
            _lFolderData = new List<FolderData>();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PrototypeExampleData", "SubjectMatter.csv");
            foreach (var l in File.ReadLines(path))
            {
                string[] ss = l.Split('|');
                string id = ss[0];
                string key = ss[1];
                string title = key + " " + ss[2];
                FolderData newFolder = new FolderData() { Id = id, Title = title, Key = key, Folder = true, Lazy = false };

                FolderData parentFolder = null;
                if (key.Length > 3)
                {
                    string parentKey = key.Substring(0, key.Length - 3);
                    string mainKey = key.Substring(0, 3);
                    var mainFolder = (from FolderData f in _lFolderData
                                      where f.Key == mainKey
                                      select f).FirstOrDefault();
                    if (parentKey == mainKey)
                        parentFolder = mainFolder;
                    else
                        parentFolder = GetFolderDataByKey(mainFolder, parentKey);
                }
                if (parentFolder == null)
                    _lFolderData.Add(newFolder);
                else
                    parentFolder.Children.Add(newFolder);
            }
        }

        private FolderData GetFolderDataByKey(FolderData fd, string key)
        {

            FolderData result = (from FolderData f in fd.Children
                                 where f.key == key
                                 select f).FirstOrDefault();
            if (result == null)
            {
                foreach (FolderData child in fd.Children)
                {
                    FolderData f = GetFolderDataByKey(child, key);
                    if (f != null)
                        return f;
                }
            }

            return result;
        }

        private FolderData GetFolderDataById(string id)
        {
            FolderData result = null;

            List<FolderData> fd = new List<FolderData>();
            foreach (FolderData f in _lFolderData)
            {
                fd.Add(f);
            }

            foreach (FolderData folderData in fd)
            {
                result = (from FolderData f in folderData.Children
                          where f.id == id
                          select f).FirstOrDefault();
                if (result == null)
                {
                    foreach (FolderData child in folderData.Children)
                    {
                        FolderData f = GetFolderDataById(id);
                        if (f != null)
                            return f;
                    }
                }
            }
            return result;
        }

        public List<FolderData> Get()
        {
            return _lFolderData;
        }

    }
}
