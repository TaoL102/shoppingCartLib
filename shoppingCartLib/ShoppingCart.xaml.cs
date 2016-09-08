﻿using ShoppingCartLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace shoppingCartLib
{
    /// <summary>
    /// Interaction logic for ShoppingCart.xaml
    /// This is the controller for the main view
    /// Author: Tao Liu
    /// Date: 21/08/2016
    /// </summary>
    public partial class ShoppingCart : UserControl
    {
        #region Fields
        // Order
        Order order;

        // Observable Collection for the items in the display area
        ObservableCollection<Item> collectionDisplayItems;

        // List for the items from inputdata
        List<Item> listInputItems;

        // file path for external xml files( This is for temporiry use, in actural situations, this control will connect to database directly)
        public static string filePath;
        #endregion

        #region Methods
        /// <summary>
        /// Constructor
        /// </summary>
        public ShoppingCart()
        {
            InitializeComponent();

            // Call Init() method to initialize fields, data source etc.. 
            Init();
        }

        /// <summary>
        /// To initialize fields, data source, UI
        /// </summary>
        private void Init()
        {
            // Initialize order
            order = new Order();

            // Define temp files path
            filePath = "C:\\Temp";

            // Initialize data source, Get data from external file.
            listInputItems = ReadDataFromXMLFile();

            // By default, show all items in the items display area
            collectionDisplayItems = new ObservableCollection<Item>(listInputItems);

            // Data Binding
            ListBox_DisplayItems.ItemsSource = collectionDisplayItems;
            ListBox_DisplayCategory.ItemsSource = 
                listInputItems
                .GroupBy(item => item.Category)
                .Select(group => group.First())
                .OrderBy(item => item.Category);
            ListBox_ItemsInOrder.ItemsSource = order.CartItemList;
            grid_OrderDetails.DataContext = order;
        }

        /// <summary>
        /// To read external xml file and convert it to a list of items
        /// Reference: XElement Class, https://msdn.microsoft.com/en-us/library/system.xml.linq.xelement(v=vs.110).aspx
        /// </summary>
        /// <returns></returns>
        private List<Item> ReadDataFromXMLFile()
        {
            List<Item> list = new List<Item>();

            string itemsDataFilePath = filePath + "\\itemsData.xml";

            // Check if file exsits
            if (!File.Exists(itemsDataFilePath)) return list;

            // If file exsits, iterate the items and crate a list of items
            var items = from item in XElement.Load(itemsDataFilePath).Elements() select item;
            foreach (var item in items)
            {
                Item i = new Item(item.Element("ID")?.Value,
                    item.Element("Name")?.Value,
                    Convert.ToDouble(item.Element("UnitPrice")?.Value),
                    Convert.ToInt32(item.Element("TaxPercentage")?.Value),
                    item.Element("Category")?.Value,
                    filePath + "\\" + item.Element("PicUrl")?.Value
                );
                list.Add(i);
            }
            return list;
        }

        /// <summary>
        /// To apply coupon discount to items in order
        /// </summary>
        private void ApplyCouponToOrder()
        {
            if (order.Coupon == null) return;

            // Check if the coupon applies to category 
            if (!string.IsNullOrEmpty(order.Coupon.ApplicableCategory))
            {
                // Get all items in this category
                List<Item> items = SearchItemByCategory(order.Coupon.ApplicableCategory);

                // For each item, set the discount rate
                items.ForEach(item =>
                {
                    item.DiscountPercentage = order.Coupon.DiscountPercentage;
                    item.IsDiscountApplied = true;
                });
            }

            // Otherwise, check if the coupon applies to a specific item
            else if (!string.IsNullOrEmpty(order.Coupon.ApplicableItemCode))
            {
                // Get the applicable item
                Item item = SearchItemById(order.Coupon.ApplicableItemCode);

                // Set the discount rate for this item
                item.DiscountPercentage = order.Coupon.DiscountPercentage;
                item.IsDiscountApplied = true;
            }

            // update the price grid
            grid_OrderDetails.DataContext = null;
            grid_OrderDetails.DataContext = order;
        }

        /// <summary>
        /// Search item by item ID
        /// </summary>
        /// <param name="id">Item ID</param>
        /// <returns>Item</returns>
        public Item SearchItemById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            // search the database for the item which matches the ID
            var result = collectionDisplayItems.Where(i => i.Id.ToLower().Equals(id.ToLower()));

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Search items by item category
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>Item list</returns>
        public List<Item> SearchItemByCategory(string category)
        {
            List<Item> items = new List<Item>();
            if (string.IsNullOrEmpty(category)) return items;

            // search the database for the items which matches the category
            var result = collectionDisplayItems.Where(i => i.Category.ToLower().Equals(category.ToLower()));

            items = result.ToList();
            return items;
        }

        /// <summary>
        /// Search coupon by coupon code
        /// </summary>
        /// <param name="couponCode">Coupon Code</param>
        /// <returns>Coupon</returns>
        public Coupon SearchCouponById(string couponCode)
        {
            if (string.IsNullOrWhiteSpace(couponCode)) return null;

            Coupon coupon = null;

            // Get coupon from file
            string couponFilePath = filePath + "\\couponsData.xml";

            // Query the coupon data from XML file
            // Reference: XElement Class, https://msdn.microsoft.com/en-us/library/system.xml.linq.xelement(v=vs.110).aspx
            var results = from item in XElement.Load(couponFilePath).Elements() select item;
            IEnumerable<XElement> tests =
                from el in results
                where el.Element("Code").Value.ToString().ToLower().Equals(couponCode.ToLower())
                select el;
            XElement xElement = tests.FirstOrDefault();

            if (xElement != null)
            {
                coupon = new Coupon(xElement.Element("Code").Value,
                    Convert.ToDateTime(xElement.Element("StartDate").Value),
                    Convert.ToDateTime(xElement.Element("EndDate").Value),
                    Convert.ToDouble(xElement.Element("DiscountPercentage").Value),
                    xElement.Element("ApplicableCategory").Value,
                    xElement.Element("ApplicableItemCode").Value);
            }
            return coupon;
        }

        #endregion

        #region Events

        /// <summary>
        /// Search Box Text Changed event handler
        /// To query the database and display items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtBox_Search_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // Not applicable when the control is loaded
            if (!IsLoaded) return;

            string txt = ((TextBox)sender).Text;

            // If text is null, empty or white space, show all items
            if (string.IsNullOrWhiteSpace(txt))
            {
                collectionDisplayItems.Clear();
                listInputItems.ForEach(item => collectionDisplayItems.Add(item));
            }

            // If not null, begin to search
            else if (txt != "Search")
            {
                // Query the database
                var linqResut = 
                    listInputItems
                    .Where(item=> item.Name.ToLower().Contains(txt.ToLower()) || item.Id.Equals(txt));

                // Clear the items already in the display area
                collectionDisplayItems.Clear();

                // Add the search result to the display area
                linqResut.ToList().ForEach(item => collectionDisplayItems.Add(item));
            }
        }

        /// <summary>
        /// Item selected event handler for list box in items display area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemListBox_ItemSelected(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem == null) return;

            // Get the selected item
            Item item = (Item)listBoxItem.DataContext;

            // Select the coresponding item in order area
            ListBox_ItemsInOrder.SelectedItem = item;

            // Scroll to this item 
            ListBox_ItemsInOrder.ScrollIntoView(item);
            ListBox_DisplayItems.ScrollIntoView(item);
            ListBox_DisplayItems.SelectedItem = item;
        }

        /// <summary>
        /// IncrementByOne button click event;
        /// To increase the quantity of selected item by one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnIncrementByOne_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (order == null) return;
            var border = sender as Border;
            if (border == null) return;

            // Get the selected item
            Item item = (Item)border.DataContext;

            // Increase the quantity by one and update the datacontext of the order display details
            order.IncrementQuantityByOne(item);
            grid_OrderDetails.DataContext = null;
            grid_OrderDetails.DataContext = order;
        }

        /// <summary>
        /// DecrementByOne button click event;
        /// To decrease the quantity of selected item by one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDecrementByOne_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (order == null) return;
            var border = sender as Border;
            if (border == null) return;

            // Get the selected item
            Item item = (Item)border.DataContext;

            // Decrease the quantity by one and update the datacontext of the order display area
            order.DecrementQuantityByOne(item);
            grid_OrderDetails.DataContext = null;
            grid_OrderDetails.DataContext = order;
        }

        /// <summary>
        /// Delete button click event handler;
        /// To delete the selected item in order;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (order == null) return;

            // Get the selected item in shopping cart
            Item selectedItem = ListBox_ItemsInOrder.SelectedItem as Item;

            // Remove this item from order object and update the datacontext of the order display details
            order.RemoveItem(selectedItem);
            grid_OrderDetails.DataContext = null;
            grid_OrderDetails.DataContext = order;
        }

        /// <summary>
        /// Item selected event handler for list box in category area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CategoryListBox_ItemSelected(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem == null) return;

            // Get the selected item
            Item itemSelected = (Item)listBoxItem.DataContext;

            // Search the database for items which match the category selected
            var linqResut = listInputItems.Where(i => i.Category.Equals(itemSelected.Category));

            // Clear the items already in the item display area
            collectionDisplayItems.Clear();
            linqResut.ToList().ForEach(item => collectionDisplayItems.Add(item));
        }

        /// <summary>
        /// Pay button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Pay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set the message box title and content
            txtBlock_MsgBox_Title.Text = "PAY";
            Grid_MsgBox.Tag = "PAY";
            txtBlock_MsgBox_Content.Text = order.ToString();

            // Set the message box visible
            Grid_MsgBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Cancel Order button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Cancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set the message box title and content
            txtBlock_MsgBox_Title.Text = "CANCEL";
            Grid_MsgBox.Tag = "CANCEL";
            txtBlock_MsgBox_Content.Text = "Confirm to cancel this order?";

            // Set the message box visible
            Grid_MsgBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Clear search box button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Clear_Search_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // set focus to search box and clear the text, so the text in search box would be empty, 
            // which triggers the textChanged event handler , returns all items and display
            txtBox_Search.Focus();
            txtBox_Search.Clear();
        }

        /// <summary>
        /// Apply coupon button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Coupon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get txt from Txt_Coupon text box
            string txt = TxtBox_Coupon.Text;

            // Call the SearchCouponById() method to find coupon
            order.Coupon = SearchCouponById(txt);

            // Set msg box title and content
            txtBlock_MsgBox_Title.Text = "COUPON";
            Grid_MsgBox.Tag = "COUPON";
            txtBlock_MsgBox_Content.Text = order.Coupon?.ToString() ?? "No coupon found.";

            // Set msg box visible
            Grid_MsgBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Message box YES button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_MsgBox_Yes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Check what the message box is for
            switch (Grid_MsgBox.Tag.ToString())
            {
                case "CANCEL":
                    Init();
                    break;

                case "PAY":
                    // Save order detail to local xml file
                    order.SaveOrderToFile(filePath);

                    // Clear current order and Creat a new order
                    Init();
                    break;

                case "COUPON":
                    if (order.Coupon != null)
                    {
                        // Apply coupon
                        ApplyCouponToOrder();
                    };
                    break;
            }

            // Set the message box invisible
            Grid_MsgBox.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///  Message box NO button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_MsgBox_No_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set the message box invisible
            Grid_MsgBox.Visibility = Visibility.Collapsed;
        }
        #endregion
    }
}
