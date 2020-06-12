namespace Interlex.BusinessLayer.Models
{
    using System;
    using Interlex.BusinessLayer.Enums;
    using Interlex.DataLayer;
    using System.Data;
    using System.Collections.Generic;

    public class OrderStatusHistory
    {
        public DateTime Date { get; set; }

        public string DateAsString { get; set; }

        public int OrderStatusTypeId { get; set; }

        public OrderStatusTypes OrderStatusType { get; set; }

        public OrderStatusHistory(DateTime date, int orderStatusTypeId)
        {
            this.Date = date;
            this.DateAsString = date.ToShortDateString();
            this.OrderStatusTypeId = orderStatusTypeId;
            this.OrderStatusType = (OrderStatusTypes)orderStatusTypeId;
        }

        public OrderStatusHistory(IDataRecord row)
        {
            this.Date = Convert.ToDateTime(row["date"]);
            this.DateAsString = row["date"].ToString();
            this.OrderStatusTypeId = int.Parse(row["order_status_type_id"].ToString());
            this.OrderStatusType = (OrderStatusTypes)this.OrderStatusTypeId;
        }
        
        public static List<OrderStatusHistory> GetOrderStatusHistory(int orderId)
        {
            var statusHistory = new List<OrderStatusHistory>();

            foreach (var item in DB.GetOrderStatusHistory(orderId))
            {
                statusHistory.Add(new OrderStatusHistory(item));
            }

            return statusHistory;
        }
    }
}
