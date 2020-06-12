namespace Interlex.App.ViewModels
{
    using Interlex.BusinessLayer.Enums;
    using Interlex.BusinessLayer.Models;

    public class ProductFeaturesInfo
    {//
        public FunctionalityTypes FunctionalityType { get; set; }
        public Document Document { get; set; }

        public ProductFeaturesInfo()
        {

        }

        public ProductFeaturesInfo(FunctionalityTypes funcTypeId)
        {
            this.FunctionalityType = funcTypeId;
        }

        public ProductFeaturesInfo(FunctionalityTypes funcType, Document document)
        {
            this.FunctionalityType = funcType;
            this.Document = document;
        }
    }
}