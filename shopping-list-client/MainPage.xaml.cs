using shopping_list_client.Model;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using System.Collections.Generic;
using shopping_list_client.Controller;

namespace shopping_list_client
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private RestfulController restfulController;
        private List<Item> serverItems = new List<Item>();
        
        public MainPage()
        {
            InitializeComponent();

            restfulController = new RestfulController();

            restfulController.items = restfulController.GetItems();
            itemsListView.ItemsSource = restfulController.items;
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            string item = itemName.Text;
            if (item != null && item.Length > 0)
            {
                restfulController.AddItem(item);
            }
            itemName.Text = "";
        }

        private void Bought_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Item t = (Item)((CheckBox)sender).BindingContext;
            if (t != null)
            {
                restfulController.ItemBought(t);
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            restfulController.DeleteBought();
        }
    }
}
