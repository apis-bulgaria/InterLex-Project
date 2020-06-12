namespace Interlex.BusinessLayer.Models
{
    using System;
    using System.Collections.Generic;
    using Interlex.DataLayer;

    /* public enum LawDateType 
     {
         DocDate = 0,
         DatePublication = 1,
         DateOfEffect = 2,
         DateEnd = 3,
         DateNotification = 4,
         DateTransposition = 5,
         DateSignature = 6,
         DateLodget = 7
     }*/





    public interface ICheckTreeView
    {
        string SelectedIds { get; set; }
    }

    public class CheckTreeView : ICheckTreeView
    {
        private int _langId;

        public CheckTreeView(int langId)
        {
            this._langId = langId;
        }

        private Guid[] _selIds;
        public string SelectedIds
        {
            get
            {
                if (_selIds == null)
                    return String.Empty;
                return String.Join(",", _selIds);
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    _selIds = Array.ConvertAll<string, Guid>(value.Split(','), Guid.Parse);
            }
        }

        private string[] _keyPaths;
        public string KeyPaths
        {
            get
            {
                if (_keyPaths == null)
                    _keyPaths = GetSelectedIdPaths();
                return String.Join("|", _keyPaths);
            }
        }

        private string[] GetSelectedIdPaths()
        {
            List<string> keyPaths = new List<string>();
            if (_selIds != null)
            {
                foreach (Guid id in _selIds)
                {
                    keyPaths.Add(DB.GetClassifierId_KeyPath(id, true));
                }
            }
            return keyPaths.ToArray();
        }

        private string[] _titlePaths;
        public string[] TitlePaths
        {
            get
            {
                if (_titlePaths == null)
                    _titlePaths = GetSelectedTitlePaths();
                return _titlePaths;
            }
        }

        private string[] GetSelectedTitlePaths()
        {
            List<string> titlePaths = new List<string>();
            if (_selIds != null)
            {
                foreach (Guid id in _selIds)
                {
                    titlePaths.Add(DB.GetClassifierId_TitlePath(id, this._langId, true));
                }
            }
            return titlePaths.ToArray();
        }
    }

    public class SearchBox
    {
        private int _langId;

        public bool? ShowFreeDocuments { get; set; }

        public string SearchText { get; set; }
        public string SearchTextMultiLingual { get; set; }

        public bool ExactMatch { get; set; }

        public string GlobalId { get; set; }

        /// <summary>
        /// A global search filter used to open classifiers from document metadata section
        /// </summary>
        public Guid? ClassifierFilter { get; set; }

        public bool ByXmlId { get; set; }

        public string ClassifierFilterTitle { get; set; }

        /// <summary>
        /// Folder search in home page
        /// </summary>
        public HomeSearchData HomeSearch { get; set; }

        public DocLinksFilter DocInLinks;

        public SearchCases Cases { get; set; }
        public SearchLaw Law { get; set; }

        public SearchFinances Finances { get; set; }

        public SearchMultiDict MultiDict { get; set; }

        public SearchBox(int langId)
        {
            ExactMatch = false;
            this.GlobalId = Guid.NewGuid().ToString();
            this._langId = langId;
        }

        /// <summary>
        /// Проверява дали търсенето е от папки от началния екран, за които има rel sort
        /// </summary>
        public bool HasRelSort
        {
            get
            {
                if (this.HomeSearch?.Query == "props:pr1 (classificators:(af88ca517522455aaefeec0d3c2d6a37 || 987e4eef3e5543be9052bb98ef1dfd83 -f83a4979034843e0a7b6d50c07d4eee9))" ||
                    this.HomeSearch?.Query == "props:pr1 (classificators:(af88ca517522455aaefeec0d3c2d6a37 -f83a4979034843e0a7b6d50c07d4eee9))")
                    return true;

                return false;
            }
        }
    }
}