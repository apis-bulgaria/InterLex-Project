using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer.Models
{
    public class PieChartData
    {
        public string Color { get; set; }

        public int Id { get; set; }

        public double Data { get; set; }

        public string Label { get; set; }

        public PieChartData() { }

        public PieChartData(string label, double data) : this(label, data, "") { }

        public PieChartData(string label, double data, string color) 
        {
            this.Color = color;
            this.Data = data;
            this.Label = label;
        }
    }
}
