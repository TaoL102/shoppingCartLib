using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace shoppingCartLib
{
    public class Item : Comparer<Item>, INotifyPropertyChanged
    {

        // Fields
        private int quantity;
        private double totalPrice;
        private string name;
        private double unitPrice;
        private double taxPrice;
        private double taxPercentage;
        private string category;
        private string picUrl;
        private string id;
        private double discountPercentage;
        private double discountedTotalPrice;
        private bool isDiscountApplied;


        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            string unitPriceAndQuantity = UnitPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")) + "*" + Quantity;
            string discount=IsDiscountApplied?( "Dis.:"+ DiscountPercentage+"%\t-"+ DiscountedTotalPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ"))):null;
            string totalPrice = TotalPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ"));
            return Name + "\n" + unitPriceAndQuantity + "\t" + totalPrice + "\t"+ discount;


        }

        public Item(string id, string name, double uniPrice, double taxPercentage, string category, string picUrl)
        {
            this.Id = id;
            this.Name = name;
            this.UnitPrice = uniPrice;
            this.TaxPercentage = taxPercentage;
            this.Category = category;
            this.PicUrl = picUrl;
            this.Quantity = 0;
            this.isDiscountApplied = false;
            this.DiscountPercentage = 0;
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
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("DiscountedTotalPrice");
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
                return name;
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



        public double TaxPrice
        {
            get
            {

                return TaxPercentage * TotalPrice * 0.01;
            }

            set
            {
                taxPrice = value;

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

            set
            {
                category = value;
            }
        }

        public string PicUrl
        {
            get
            {
                return picUrl;
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
                return id;
            }

            set
            {
                id = value;
            }
        }

        public double DiscountPercentage
        {
            get
            {
                return discountPercentage;
            }

            set
            {
                if (value >= 0 && value < 100)
                {
                    discountPercentage = value;
                }
                else
                {
                    discountPercentage = 100;
                }

                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("DiscountPercentage");

            }
        }

        public double DiscountedTotalPrice
        {
            get
            {
                return TotalPrice*(DiscountPercentage*0.01);
            }

            set
            {
                discountedTotalPrice = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("DiscountedTotalPrice");
            }
        }

        public bool IsDiscountApplied
        {
            get
            {
                return isDiscountApplied;
            }

            set
            {
                isDiscountApplied = value;
                
                // Call OnPropertyChanged whenever the property is updated
            OnPropertyChanged("IsDiscountApplied");
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("DiscountedTotalPrice");
            }
            
        }


        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override int Compare(Item x, Item y)
        {
            return x.Id.CompareTo(y.Id);
        }
    }
}

