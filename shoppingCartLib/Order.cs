using shoppingCartLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartLib
{
    class Order : INotifyPropertyChanged
    {

        private ObservableCollection<ShoppingCartItem> shoppingCartList;
        private double netPrice;
        private double taxPrice;
        private double totalPrice;

        public event PropertyChangedEventHandler PropertyChanged;

        internal ObservableCollection<ShoppingCartItem> ShoppingCartList
        {
            get
            {

                return shoppingCartList;
            }

            set
            {
                shoppingCartList = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("ShoppingCartList");
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("NetPrice");
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("TaxPrice");
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("TotalPrice");

            }
        }

        public double NetPrice
        {
            get
            {
                return this.ShoppingCartList.Sum(item => item.TotalPrice);
            }

            set
            {
                netPrice = value;

            }
        }

        public double TaxPrice
        {
            get
            {
                return this.CalculateTaxPrice();
            }

            set
            {
                taxPrice = value;

            }
        }

        public double TotalPrice
        {
            get
            {
                return this.NetPrice + this.TaxPrice;
            }

            set
            {
                totalPrice = value;

            }
        }

        public ShoppingCartItem SearchByItem(Item item)
        {
            if (item != null)
            {


                var result = from ShoppingCartItem in ShoppingCartList where ShoppingCartItem.Id.Equals(item.Id) select ShoppingCartItem;
                return result.FirstOrDefault();
            }
            else return null;
        }


        public void IncrementQuantityByOne(ShoppingCartItem item)
        {
            if (item != null)
            {



                if (item.Quantity > 0)
                {
                    item.Quantity++;

                }
                else
                {
                    item.Quantity++;
                    this.ShoppingCartList.Add(item);
                }



            }


        }

        public void DecrementQuantityByOne(ShoppingCartItem item)
        {
            if (item != null)
            {


                if (item.Quantity > 1)
                {
                    item.Quantity--;

                }
                else if (item.Quantity == 1)
                {
                    item.Quantity--;
                    this.ShoppingCartList.Remove(item);
                }




            }

        }

        public void RemoveItem(ShoppingCartItem item)
        {
            if (item != null)
            {
                item.Quantity = 0;
                this.shoppingCartList.Remove(item);
            }


        }

        public void RemoveAllItem()
        {
            ShoppingCartList.All(item => { item.Quantity = 0; return true; });
            ShoppingCartList.Clear();
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ShoppingCartItem item in ShoppingCartList)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Netprice:" + NetPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")));
            sb.AppendLine("Tax:" + TaxPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")));
            sb.AppendLine("TotalPrice:" + TotalPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")));

            return sb.ToString();
        }



        public Order()
        {
            shoppingCartList = new ObservableCollection<ShoppingCartItem>();

        }



        public double CalculateNetPrice()
        {
            return this.ShoppingCartList.Sum(item => item.TotalPrice);
        }

        public double CalculateTaxPrice()
        {
            return this.shoppingCartList.Sum(item => item.TaxPrice);
        }






        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }
    }
}
