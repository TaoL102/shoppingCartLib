using ShoppingCartLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace shoppingCartLib
{
    /// <summary>
    /// Interaction logic for ShoppingCart.xaml
    /// </summary>
    public partial class ShoppingCart : UserControl
    {
        // Observable Collection for the items in the display area
        ObservableCollection<Item> collectionDisplayItems;

        // Observable Collection for the items in the order
        //ObservableCollection<Item> collectionItemsInOrder;

        // Order
        Order order;

        // List for the items from inputdata
        List<Item> listInputItems;

        // Temp files path
        public static string filePath = "C:\\Temp";



        public ShoppingCart()
        {
            InitializeComponent();


            Init();

        }


        private void Init()
        {
            // Define temp files path
            filePath = "C:\\Temp";

            // Initialize data source, Get data from external file.
            listInputItems = ReadDataFromXMLFile();

            // By default, show all items in the items display area
            collectionDisplayItems = new ObservableCollection<Item>(listInputItems);

            // Initialize order
            order = new Order();

            // Data Binding
            ListBox_DisplayItems.ItemsSource = collectionDisplayItems;
            ListBox_DisplayCategory.ItemsSource = listInputItems.GroupBy(item => item.Category).Select(group => group.First()).OrderBy(item => item.Category);
            ListBox_ItemsInOrder.ItemsSource = order.ShoppingCartList;
            gridOrderDetails.DataContext = order;
        }

        private List<Item> ReadDataFromXMLFile()
        {
            List<Item> list = new List<Item>();

            string itemsDataFilePath = filePath + "\\itemsData.xml";

            // Check if file exsits
            if (File.Exists(itemsDataFilePath))
            {
                var items = from item in XElement.Load(itemsDataFilePath).Elements() select item;



                foreach (var item in items)
                {
                    Item i = new Item(item.Element("ID").Value,
                        item.Element("Name").Value,
                        Convert.ToDouble(item.Element("UnitPrice").Value),
                        Convert.ToInt32(item.Element("TaxPercentage").Value),
                        item.Element("Category").Value,
                        filePath + "\\" + item.Element("PicUrl").Value
                        );
                    list.Add(i);


                }
            }

            return list;

        }





        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                string txt = ((TextBox)sender).Text;
                if (string.IsNullOrWhiteSpace(txt))
                {
                    collectionDisplayItems.Clear();
                    listInputItems.ForEach(item => collectionDisplayItems.Add(item));
                }
                else if (txt != "Search")
                {
                    var linqResut = from item in listInputItems where item.Name.ToLower().Contains(txt.ToLower()) || item.Id.Equals(txt) select item;
                    collectionDisplayItems.Clear();
                    linqResut.ToList().ForEach(item => collectionDisplayItems.Add(item));
                }
            }



        }



        private void ItemListBox_ItemSelected(object sender, MouseButtonEventArgs e)
        {

            Item item = (Item)(sender as ListBoxItem).DataContext;
            //if (this.collectionItemsInOrder.Contains(item))
            //{
            this.ListBox_ItemsInOrder.SelectedItem = item;
            this.ListBox_ItemsInOrder.ScrollIntoView(item);
            this.ListBox_DisplayItems.ScrollIntoView(item);
            //}
            this.ListBox_DisplayItems.SelectedItem = item;

            // else
            // {
            //     shoppingCart.IncrementQuantityByOne(item);
            //  gridOrderDetails.DataContext = null;
            // GridCalculation.DataContext = shoppingCart;
            // }


            //ListBox_ItemsInOrder.ItemsSource = null;
            //ListBox_ItemsInOrder.ItemsSource = shoppingCart.ShoppingCartList;

        }


        private void BtnIncrementByOne_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            Item item = (Item)(sender as Border).DataContext;

            if (order != null)
            {
                order.IncrementQuantityByOne(item);
                gridOrderDetails.DataContext = null;
                gridOrderDetails.DataContext = order;
                // ListBox_ItemsInOrder.ItemsSource = null;
                // ListBox_ItemsInOrder.ItemsSource = shoppingCart.ShoppingCartList;
            }
        }

        private void BtnDecrementByOne_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Item item = (Item)(sender as Border).DataContext;
            if (order != null)
            {
                order.DecrementQuantityByOne(item);
                gridOrderDetails.DataContext = null;
                gridOrderDetails.DataContext = order;
                // ListBox_ItemsInOrder.ItemsSource = null;
                // ListBox_ItemsInOrder.ItemsSource = shoppingCart.ShoppingCartList;
            }
        }

        private void BtnDel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Item shoppingCartItem = (this.ListBox_ItemsInOrder.SelectedItem as Item);
            if (order != null)
            {

                order.RemoveItem(shoppingCartItem);
                gridOrderDetails.DataContext = null;
                gridOrderDetails.DataContext = order;
                // ListBox_ItemsInOrder.ItemsSource = null;
                // ListBox_ItemsInOrder.ItemsSource = shoppingCart.ShoppingCartList;
            }
        }


        private void CategoryListBox_ItemSelected(object sender, MouseButtonEventArgs e)
        {


            Item itemSelected = (Item)(sender as ListBoxItem).DataContext;
            if (itemSelected != null)
            {
                var linqResut = from item in listInputItems where item.Category.Equals(itemSelected.Category) select item;
                collectionDisplayItems.Clear();

                linqResut.ToList().ForEach(item => collectionDisplayItems.Add(item));

            }



        }

        private void Btn_Pay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            txtBlock_MsgBox_Title.Text = "PAY";
            Grid_MsgBox.Tag = "PAY";
            txtBlock_MsgBox_Content.Text = order.ToString();
            Grid_MsgBox.Visibility = Visibility.Visible;
        }


        private void Btn_Cancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Open msg box
            txtBlock_MsgBox_Title.Text = "CANCEL";
            Grid_MsgBox.Tag = "CANCEL";
            txtBlock_MsgBox_Content.Text = "Confirm to cancel this order?";
            Grid_MsgBox.Visibility = Visibility.Visible;
        }


        private void Btn_MsgBox_Yes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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

                        // update the price grid
                        gridOrderDetails.DataContext = null;
                        gridOrderDetails.DataContext = order;

                    };
                    break;
                default:
                    break;
            }

            Grid_MsgBox.Visibility = Visibility.Collapsed;
        }

        private void Btn_MsgBox_No_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid_MsgBox.Visibility = Visibility.Collapsed;
        }

        private void ListBox_ItemsInOrder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() != typeof(ListBoxItem))
                ListBox_ItemsInOrder.UnselectAll();
        }

        private void Btn_Clear_Search_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // collectionDisplayItems.Clear();
            //  listInputItems.ForEach(item => collectionDisplayItems.Add(item));
            SearchBox.Focus();
            SearchBox.Clear();
        }

        private void Btn_Coupon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get txt from Txt_Coupon
            string txt = TxtBox_Coupon.Text;

            order.Coupon = SearchCouponById(txt);

            // Open msg box
            txtBlock_MsgBox_Title.Text = "COUPON";
            Grid_MsgBox.Tag = "COUPON";
            txtBlock_MsgBox_Content.Text =(order.Coupon != null?order.Coupon.ToString() :"No coupon found.");  
              Grid_MsgBox.Visibility = Visibility.Visible;

        }

        private void ApplyCouponToOrder()
        {
            if (order.Coupon != null) { 
            if (!string.IsNullOrEmpty(order.Coupon.ApplicableCategory))
            {
                List<Item> items = SearchItemByCategory(order.Coupon.ApplicableCategory);
                items.ForEach(item => { item.DiscountPercentage = order.Coupon.DiscountPercentage; item.IsDiscountApplied = true; });
            }
            else if (!string.IsNullOrEmpty(order.Coupon.ApplicableItemCode))
            {
                Item item = SearchItemById(order.Coupon.ApplicableItemCode);
                item.DiscountPercentage = order.Coupon.DiscountPercentage;
                item.IsDiscountApplied = true;
            }
        }
        }

        public Item SearchItemById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {


                var result = from ShoppingCartItem in collectionDisplayItems where ShoppingCartItem.Id.ToLower().Equals(id.ToLower()) select ShoppingCartItem;
                return result.FirstOrDefault();
            }
            else return null;
        }

        public Coupon SearchCouponById(string couponCode)
        {
            Coupon coupon = null;
            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                // Get coupon from file
                string couponFilePath = filePath+"\\couponsData.xml";

                // Query the coupon data from XML file
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
            }
            return coupon;
        }

        public List<Item> SearchItemByCategory(string category)
        {
            List<Item> items = new List<Item>();
            if (!string.IsNullOrEmpty(category))
            {

                var result = from ShoppingCartItem in collectionDisplayItems where ShoppingCartItem.Category.ToLower().Equals(category.ToLower()) select ShoppingCartItem;
                items = result.ToList();
            }

            return items;
        }



        private void ListBox_ItemsInOrder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
