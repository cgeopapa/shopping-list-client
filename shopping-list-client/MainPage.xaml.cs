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
        private int gotItems = 0;

        public ICommand RefreshCommand { get; }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            restfulController = new RestfulController();

            restfulController.GetItems();
            itemsListView.ItemsSource = restfulController.items;
            gotItems = restfulController.items.Count;

            RefreshCommand = new Command(Refresh);
            itemsListView.RefreshCommand = RefreshCommand;
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
            if (gotItems == 0)
            {
                Item t = (Item)((CheckBox)sender).BindingContext;
                if (t != null)
                {
                    restfulController.ItemBought(t);
                }
            }
            else
            {
                gotItems--;
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
            gotItems = restfulController.items.Count;

            IsRefreshing = false;
        }
    }
}
