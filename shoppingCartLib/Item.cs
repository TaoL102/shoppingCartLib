using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shoppingCartLib
{
    public class Item
    {
        private string id;
        private string name;
        private double unitPrice;
        private double taxPercentage;
        private string category;
        private string picUrl;


        public double UnitPrice
        {
            get
            {
                return unitPrice;
            }

            protected set
            {
                if (value >= 0)
                {
                    unitPrice = value;
                }
                else
                {
                    unitPrice = 0;
                }

            }
        }

        public double TaxPercentage
        {
            get
            {
                return taxPercentage;
            }

            protected set
            {
                if (value >= 0 && value <= 100)
                {
                    taxPercentage = value;
                }
                else
                {
                    taxPercentage = 100;
                }

            }
        }

        public string Category
        {
            get
            {
                return category;
            }

            protected set
            {
                category = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            protected set
            {
                name = value;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }

            protected set
            {
                id = value;
            }
        }

        public string PicUrl
        {
            get
            {
                return "/image/" + picUrl;
            }

            protected set
            {
                picUrl = value;
            }
        }

        public Item(string id, string name, double uniPrice, double taxPercentage, string category, string picUrl)
        {
            this.Id = id;
            this.Name = name;
            this.UnitPrice = uniPrice;
            this.TaxPercentage = taxPercentage;
            this.Category = category;
            this.PicUrl = picUrl;
        }

    }
}
