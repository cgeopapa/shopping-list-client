using shopping_list_client.Model;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using shopping_list_client.Controller;
using System.Windows.Input;

namespace shopping_list_client
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly RestfulController restfulController;

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        public ICommand RefreshCommand { get; }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            itemsListView.RefreshCommand = RefreshCommand;
            itemsListView.IsRefreshing = IsRefreshing;

            restfulController = new RestfulController();
            RefreshCommand = new Command(Refresh);

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

        private void Refresh()
        {
            IsRefreshing = true;

            itemsListView.ItemsSource = null;
            restfulController.GetItems();
            itemsListView.ItemsSource = restfulController.items;

            IsRefreshing = false;
        }
    }
}
