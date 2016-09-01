using shoppingCartLib;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ShoppingCartLib
{
    class Order : INotifyPropertyChanged
    {
        private static int orderNumber = 0;

        private ObservableCollection<Item> shoppingCartList;
        private double netPrice;
        private double taxPrice;
        private double totalPrice;
        private double discountedTotalPrice;
        private Coupon coupon;
 

        public event PropertyChangedEventHandler PropertyChanged;

        internal ObservableCollection<Item> ShoppingCartList
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
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged("DiscountedTotalPrice");
            }
        }

        public double NetPrice
        {
            get
            {
                return CalculateNetPrice();
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
                return this.NetPrice + this.TaxPrice-this.DiscountedTotalPrice;
            }

            set
            {
                totalPrice = value;

            }
        }

        public double DiscountedTotalPrice
        {
            get
            {
                return CalculateDiscountedTotalPrice();
            }

            set
            {
                discountedTotalPrice = value;
            }
        }

        public string OrderNumber
        {
            get
            {
                return  DateTime.Now.Date.ToString("ddMMyyyy") + "0000000"+orderNumber;
            }

        }

        public Coupon Coupon
        {
            get
            {
                return coupon;
            }

            set
            {
                coupon = value;
            }
        }






        public void IncrementQuantityByOne(Item item)
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

        public void DecrementQuantityByOne(Item item)
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

        public void RemoveItem(Item item)
        {
            if (item != null)
            {
                item.Quantity = 0;
                shoppingCartList.Remove(item);
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
            sb.AppendLine("Order No.: "+OrderNumber);
            sb.AppendLine();
            foreach (Item item in ShoppingCartList)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine();
            sb.AppendLine("Netprice: " + NetPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")));
            sb.AppendLine("Tax: " + TaxPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")));
            sb.AppendLine("Discount: -" + DiscountedTotalPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")));
            sb.AppendLine("TotalPrice: " + TotalPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")));

            return sb.ToString();
        }



        public Order()
        {
            shoppingCartList = new ObservableCollection<Item>();
            orderNumber++;
        }



        public double CalculateNetPrice()
        {
            return this.ShoppingCartList.Sum(item => item.TotalPrice);
        }

        public double CalculateTaxPrice()
        {
            return this.shoppingCartList.Sum(item => item.TaxPrice);
        }

        public double CalculateDiscountedTotalPrice()
        {
            return this.shoppingCartList.Sum(item => item.DiscountedTotalPrice);
        }

        public void SaveOrderToFile(string filePath)
        {
            XDocument xdoc = null;
            XElement itemsElement = new XElement("Items");
            foreach (var item in this.ShoppingCartList)
            {
                XElement itemElement = new XElement("Item",
                                                new XElement("ID", item.Id),
                                                new XElement("Name", item.Name),
                                                new XElement("UnitPrice", item.UnitPrice),
                                                new XElement("TaxPercentage", item.TaxPercentage),
                                                new XElement("Category", item.Category),
                                                new XElement("DiscountPercentage", item.DiscountPercentage),
                                                new XElement("Quantity", item.Quantity),
                                                new XElement("TotalPrice", item.TotalPrice)
                                                );
                itemsElement.Add(itemElement);
            }
            XElement orderElement = new XElement("Order",
                                   new XElement("OrderNumber", OrderNumber),
                                   new XElement("NetPrice", NetPrice),
                                   new XElement("TaxPrice", TaxPrice),
                                   new XElement("DiscountedTotalPrice", DiscountedTotalPrice),
                                   new XElement("TotalPrice", TotalPrice),
                                   itemsElement
                                   );

            // Save to an existing xml file
            if (File.Exists(filePath + "\\ordersData.xml"))
            {
                // Load the xml file
                xdoc = XDocument.Load(filePath + "\\ordersData.xml");

                // Check if xml file's format is right 
                if (xdoc.Root.Name.LocalName.Equals("Orders")) { 

                // Add element to xml file
                xdoc.Descendants("Orders").FirstOrDefault().AddFirst(orderElement);
            }
                else
                {
                    // Write XML file
                    xdoc = new XDocument(new XElement("Orders"));
                    //  xdoc.Add(itemElement);
                    xdoc.Root.AddFirst(orderElement);
                }
            }
            else
            {
                // Write XML file
                    xdoc = new XDocument(new XElement("Orders"));
                //  xdoc.Add(itemElement);
                    xdoc.Root.AddFirst(orderElement);

            }
            try
            {
                xdoc.Save (filePath + "\\ordersData.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }



        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }
    }
}
