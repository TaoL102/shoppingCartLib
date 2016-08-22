using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shoppingCartLib
{
    class ShoppingCartItem : Comparer<ShoppingCartItem>, INotifyPropertyChanged
    {

        // Fields
        private int quantity;
        private Item item;
        private double totalPrice;
        private string name;
        private double unitPrice;
        private double taxPrice;
        private string category;
        private string picUrl;
        private string id;


        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            string unitPriceAndQuantity = UnitPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")) + "*" + Quantity;
            string totalPrice = TotalPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ"));
            return Name + "\n" + unitPriceAndQuantity + "\t" + totalPrice;


        }

        public int Quantity
        {
            get
            {
                return quantity;
            }

            set
            {
                quantity = value;

                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("Quantity");
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("TotalPrice");
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("TaxPrice");
            }
        }


        public double TotalPrice
        {
            get
            {

                return UnitPrice * Quantity;

            }

            set
            {
                totalPrice = value;

            }
        }

        public string Name
        {
            get
            {
                return item.Name;
            }

            set
            {
                name = value;
            }
        }

        public double UnitPrice
        {
            get
            {
                return item.UnitPrice;
            }

            set
            {
                unitPrice = value;
            }
        }

        public double TaxPrice
        {
            get
            {

                return item.TaxPercentage * TotalPrice * 0.01;
            }

            set
            {
                taxPrice = value;

            }
        }

        public string Category
        {
            get
            {
                return item.Category;
            }

            set
            {
                category = value;
            }
        }

        public string PicUrl
        {
            get
            {
                return item.PicUrl;
            }

            set
            {
                picUrl = value;
            }
        }

        public string Id
        {
            get
            {
                return item.Id;
            }

            set
            {
                id = value;
            }
        }

        public ShoppingCartItem(Item item)
        {
            this.Quantity = 0;
            this.item = item;
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override int Compare(ShoppingCartItem x, ShoppingCartItem y)
        {
            return x.Id.CompareTo(y.Id);
        }
    }
}

