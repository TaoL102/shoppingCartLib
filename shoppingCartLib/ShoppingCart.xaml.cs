using ShoppingCartLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace shoppingCartLib
{
    /// <summary>
    /// Interaction logic for ShoppingCart.xaml
    /// </summary>
    public partial class ShoppingCart : UserControl
    {
        // List for Search data
        ObservableCollection<ShoppingCartItem> searchResultData;

        // List for shopping cart item
        ObservableCollection<ShoppingCartItem> shoppingCartData;
        Order order;



        // Fields
        // List for source data
        List<Item> inputDataBase;

        // Database for this UI
        ObservableCollection<ShoppingCartItem> dataBase;


        ICollectionView view;


        // test
        ObservableCollection<ShoppingCartItem> test;

        public ShoppingCart()
        {
            InitializeComponent();


            // Read data from a file
            // Step 1. 
  


            inputDataBase = new List<Item> {
                new Item("0000000001", "Fresh Produce Onions Brown Pickling Mesh Bag 750g", 2.99, 15, "Fruits & Vegetables", "0000000001.jpg"),
                new Item("0000000002", "Fresh Produce Apples Royal Gala Loose Per Kg", 3.99, 15, "Fruits & Vegetables", "0000000002.jpg"),
                new Item("0000000003", "Fresh Produce Potatoes White Washed Loose Per Kg", 2.99, 15, "Fruits & Vegetables", "0000000003.jpg"),
                new Item("0000000004", "Arnotts Tim Tam Chocolate Biscuits Original 200g", 3.75, 15, "Biscuits & Crackers", "0000000004.jpg"),
                new Item("0000000005", "Coca Cola Soft Drink 250ml Cans 6pk", 6.99, 15, "Drinks", "0000000005.jpg"),
                new Item("0000000006", "Bundaberg Ginger Beer Diet 375ml bottles 4pk", 6.29, 15, "Drinks", "0000000006.jpg"),
                new Item("0000000007", "Carlsberg Beer 440ml Cans 6pk", 15.99, 15, "Liquor", "0000000007.jpg"),
                new Item("0000000008", "Steinlager Classic Lager 330ml Btls 15pk", 27.99, 15, "Liquor", "0000000008.jpg"),
                new Item("0000000009", "Gillette Shave Gel Moisturising 195g", 5.49, 15, "Personal care", "0000000009.jpg"),
                new Item("0000000010", "L'oreal Paris Men Expert Facial Cleanser Hydra Sensitive 150ml", 9.99, 15, "Personal care", "0000000010.jpg"),
                new Item("0000000011", "Seafood Bar Atlantic Fresh Salmon Boneless Fillets (thawed) 1kg", 25, 15, "Meat & Seafood", "0000000011.jpg"),
                new Item("0000000012", "Seafood Bar Prawns Cooked & Peeled 800g", 17, 15, "Meat & Seafood", "0000000012.jpg"),
                new Item("0000000013", "Tip Top Ice Cream Cookies & Cream 2l", 5, 15, "Frozen Foods", "0000000013.jpg"),
                new Item("0000000014", "Sara Lee Danish Blueberry Frozen pkt 400g", 6.49, 15, "Frozen Foods", "0000000014.jpg"),
                new Item("0000000015", "Panadol Paracetamol Caplet Rapid 20pk", 6.49, 15, "Health & Wellness", "0000000015.jpg"),
                new Item("0000000016", "Dettol Healthy Touch Hand Sanitiser Pump - Refresh With Aloe Vera 200ml", 8, 15, "Health & Wellness", "0000000016.jpg"),
                new Item("0000000017", "BBand Aid Plasters 6cm 1m", 4.99, 15, "Health & Wellness", "0000000017.jpg")
            };

            // Initialize fields
            dataBase = new ObservableCollection<ShoppingCartItem>();

            if (inputDataBase != null)
            {

                foreach (Item item in inputDataBase)
                {
                    dataBase.Add(new ShoppingCartItem(item));
                }


                // Item list box database
                searchResultData = new ObservableCollection<ShoppingCartItem>(dataBase);
                ItemListBox.ItemsSource = searchResultData;

                // Category list box database
                this.CategoryListBox.ItemsSource = dataBase.GroupBy(item => item.Category).Select(group => group.First()).OrderBy(item => item.Category);

                // ShoppingCart
                order = new Order();
                shoppingCartData = order.ShoppingCartList;

                listboxShoppingCart.ItemsSource = shoppingCartData;


                GridCalculation.DataContext = order;










            }

        }








        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                string txt = ((TextBox)sender).Text;
                if (!String.IsNullOrEmpty(txt) && (txt != "Search"))
                {
                    var linqResut = from item in dataBase where item.Name.ToLower().Contains(txt.ToLower()) || item.Id.Equals(txt) select item;
                    searchResultData.Clear();
                    linqResut.ToList().ForEach(item => searchResultData.Add(item));

                }

            }
        }


        private void ItemListBox_ItemSelected(object sender, MouseButtonEventArgs e)
        {

            ShoppingCartItem item = (ShoppingCartItem)(sender as ListBoxItem).DataContext;
            //if (this.shoppingCartData.Contains(item))
            //{
            this.listboxShoppingCart.SelectedItem = item;
            this.listboxShoppingCart.ScrollIntoView(item);
            this.ItemListBox.ScrollIntoView(item);
            //}
            this.ItemListBox.SelectedItem = item;

            // else
            // {
            //     shoppingCart.IncrementQuantityByOne(item);
            //  GridCalculation.DataContext = null;
            // GridCalculation.DataContext = shoppingCart;
            // }


            //listboxShoppingCart.ItemsSource = null;
            //listboxShoppingCart.ItemsSource = shoppingCart.ShoppingCartList;

        }

        private void ShoppingCartItemSelected(object sender, MouseButtonEventArgs e)
        {
            // ShoppingCartItem item = (ShoppingCartItem)(sender as ListBoxItem).DataContext;
            // ListBoxItem listboxitem = (ListBoxItem)sender;
            //listboxitem.FindName()
            // grid.Visibility = Visibility.Visible;



        }




        private void BtnIncrementByOne_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            ShoppingCartItem item = (ShoppingCartItem)(sender as Border).DataContext;

            if (order != null)
            {
                order.IncrementQuantityByOne(item);
                GridCalculation.DataContext = null;
                GridCalculation.DataContext = order;
                // listboxShoppingCart.ItemsSource = null;
                // listboxShoppingCart.ItemsSource = shoppingCart.ShoppingCartList;
            }
        }

        private void BtnDecrementByOne_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShoppingCartItem item = (ShoppingCartItem)(sender as Border).DataContext;
            if (order != null)
            {
                order.DecrementQuantityByOne(item);
                GridCalculation.DataContext = null;
                GridCalculation.DataContext = order;
                // listboxShoppingCart.ItemsSource = null;
                // listboxShoppingCart.ItemsSource = shoppingCart.ShoppingCartList;
            }
        }

        private void BtnDel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ShoppingCartItem shoppingCartItem = (this.listboxShoppingCart.SelectedItem as ShoppingCartItem);
            if (order != null)
            {

                order.RemoveItem(shoppingCartItem);
                GridCalculation.DataContext = null;
                GridCalculation.DataContext = order;
                // listboxShoppingCart.ItemsSource = null;
                // listboxShoppingCart.ItemsSource = shoppingCart.ShoppingCartList;
            }
        }


        private void CategoryListBox_ItemSelected(object sender, MouseButtonEventArgs e)
        {


            ShoppingCartItem itemSelected = (ShoppingCartItem)(sender as ListBoxItem).DataContext;
            if (itemSelected != null)
            {
                var linqResut = from item in dataBase where item.Category.Equals(itemSelected.Category) select item;
                searchResultData.Clear();

                linqResut.ToList().ForEach(item => searchResultData.Add(item));

            }



        }

        private void Btn_Pay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            txtBlock_MsgBox_Title.Text = "PAY";
            txtBlock_MsgBox_Content.Text = order.ToString();
            Grid_MsgBox.Visibility = Visibility.Visible;

        }

        private void Btn_Cancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.order.RemoveAllItem();
        }

        private void Btn_MsgBox_Yes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid_MsgBox.Visibility = Visibility.Collapsed;
        }

        private void Btn_MsgBox_No_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid_MsgBox.Visibility = Visibility.Collapsed;

        }

        private void listboxShoppingCart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() != typeof(ListBoxItem))
                listboxShoppingCart.UnselectAll();
        }

        private void Btn_Clear_Search_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            searchResultData = dataBase;
        }
    }
}
