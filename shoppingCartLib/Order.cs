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
    /// <summary>
    /// This is the Order Model for the control;
    /// Author: Tao Liu
    /// Date: 21/08/2016
    /// </summary>
    internal class Order : INotifyPropertyChanged
    {
        #region Fields

        private static int _orderNumber;
        private ObservableCollection<Item> _cartItemList;

        // Reference: How to: Implement Property Change Notification, https://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public double NetPrice => CalculateNetPrice();
        public double TaxPrice => CalculateTaxPrice();
        public double TotalPrice => NetPrice + TaxPrice - DiscountedTotalPrice;
        public double DiscountedTotalPrice => CalculateDiscountedTotalPrice();
        public string OrderNumber => DateTime.Now.Date.ToString("ddMMyyyy") + "0000000" + _orderNumber;
        public Coupon Coupon { get; set; }
        internal ObservableCollection<Item> CartItemList
        {
            get
            {
                return _cartItemList;
            }
            set
            {
                _cartItemList = value;

                // Call OnPropertyChanged whenever the property is updated
                // Reference: How to: Implement Property Change Notification, https://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx
                OnPropertyChanged("CartItemList");
                OnPropertyChanged("NetPrice");
                OnPropertyChanged("TaxPrice");
                OnPropertyChanged("TotalPrice");
                OnPropertyChanged("DiscountedTotalPrice");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public Order()
        {
            // Initialize a list for store items in cart
            _cartItemList = new ObservableCollection<Item>();
            // Increment the static field _orderNumber by one to generate order number
            _orderNumber++;
        }


        /// <summary>
        /// Overide the ToString() method;
        /// To generate the details(Receipt) of this order;
        /// </summary>
        /// <returns>A string description for this order</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // Order Number
            sb.AppendLine("Order No.: " + OrderNumber);
            sb.AppendLine();

            // Order items
            foreach (Item item in CartItemList)
            {
                sb.AppendLine(item.ToString());
            }
            sb.AppendLine();

            // Price, tax and discount for the order
            sb.AppendLine("Netprice: " + NetPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")));
            sb.AppendLine("Tax: " + TaxPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")));
            sb.AppendLine("Discount: -" + DiscountedTotalPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")));
            sb.AppendLine("TotalPrice: " + TotalPrice.ToString("C2", CultureInfo.CreateSpecificCulture("en-NZ")));

            return sb.ToString();
        }

        /// <summary>
        /// Increment the quantity of an item in the order by one
        /// </summary>
        /// <param name="item">Item</param>
        public void IncrementQuantityByOne(Item item)
        {
            if (item == null) return;

            // If quantity is greater than 0, increment by one
            if (item.Quantity > 0)
            {
                item.Quantity++;
            }

            // If not, increment by one and add the item to the item list
            else
            {
                item.Quantity++;
                CartItemList.Add(item);
            }
        }

        /// <summary>
        /// Decrement the quantity of an item in the order by one
        /// </summary>
        /// <param name="item">Item</param>
        public void DecrementQuantityByOne(Item item)
        {
            if (item == null) return;

            // If quantity is greate than 1, decrement by one
            if (item.Quantity > 1)
            {
                item.Quantity--;
            }

            // If not, remove it from item list
            else 
            {
                this.RemoveItem(item);
            }
        }

        /// <summary>
        /// Remove an item from the item list of cart
        /// </summary>
        /// <param name="item">Item</param>
        public void RemoveItem(Item item)
        {
            if (item == null) return;

            // Set the quantity to 0, remove it from the list
            item.Quantity = 0;
            _cartItemList.Remove(item);
        }

        /// <summary>
        /// Remove all items from the item list of cart
        /// </summary>
        public void RemoveAllItem()
        {
            CartItemList.All(item =>
            {
                item.Quantity = 0;
                return true;
            });
            CartItemList.Clear();
        }

        /// <summary>
        /// Calcute the net price for this order
        /// </summary>
        /// <returns>Net price of the order</returns>
        public double CalculateNetPrice()
        {
            return CartItemList.Sum(item => item.TotalPrice);
        }

        /// <summary>
        /// Calcute the tax for this order
        /// </summary>
        /// <returns>Tax of the order</returns>
        public double CalculateTaxPrice()
        {
            return _cartItemList.Sum(item => item.TaxPrice);
        }

        /// <summary>
        /// Calcute the discounted total price for this order
        /// </summary>
        /// <returns>Discounted total price of the order</returns>
        public double CalculateDiscountedTotalPrice()
        {
            return _cartItemList.Sum(item => item.DiscountedTotalPrice);
        }

        /// <summary>
        /// Save order to external XML file
        /// Reference: XElement Class, https://msdn.microsoft.com/en-us/library/system.xml.linq.xelement(v=vs.110).aspx
        /// </summary>
        /// <param name="filePath">File path</param>
        public void SaveOrderToFile(string filePath)
        {
            XDocument xdoc = null;
            XElement itemsElement = new XElement("Items");
            foreach (var item in this.CartItemList)
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
                if (xdoc.Root.Name.LocalName.Equals("Orders"))
                {

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
                xdoc.Save(filePath + "\\ordersData.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Create the OnPropertyChanged method to raise the event
        /// Reference: How to: Implement Property Change Notification, https://msdn.microsoft.com/en-us/library/ms743695(v=vs.110).aspx
        /// </summary>
        /// <param name="name">Property name</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
