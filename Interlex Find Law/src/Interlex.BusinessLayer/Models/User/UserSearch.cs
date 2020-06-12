namespace Interlex.BusinessLayer.Models
{
    using System;

    public class UserSearch
    {
        public int Id { get; set; }

        private DateTime date;

        public DateTime Date 
        {
            get 
            {
                return this.date;
            }
            set 
            {
                this.date = value;
                this.DateAsString = value.ToString();
            }
        }

        public string DateAsString { get; set; }

        public SearchDetails Details { get; set; }

        public string Text { get; set; }
    }
}
