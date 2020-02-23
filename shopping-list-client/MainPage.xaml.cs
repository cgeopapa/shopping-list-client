using shopping_list_client.Model;
using System;
using System.ComponentModel;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace shopping_list_client
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private static readonly Uri url = new Uri(@"https://shopping-list-server-1.herokuapp.com/");
        private static readonly HttpClient client = new HttpClient() { BaseAddress = url };

        private ObservableCollection<Item> items = new ObservableCollection<Item>();

        public MainPage()
        {
            InitializeComponent();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            itemsListView.ItemsSource = items;
            GetItems();
        }

        private void GetItems()
        {
            string response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;

            var itemsList = JsonConvert.DeserializeObject<List<Item>>(response);
            itemsList.Sort();
            foreach (Item item in itemsList)
            {
                items.Add(item);
            }
        }

        private void AddItem()
        {
            string item = itemName.Text;
            if (item == null || item.Length > 0)
            {
                var values = new Dictionary<string, string>
                {
                    {"name", item }
                };
                var content = new FormUrlEncodedContent(values);
                var response = client.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
                content.Dispose();

                items.Add(JsonConvert.DeserializeObject<Item>(response));

                itemName.Text = "";
            }
        }

        private void DeleteItem(Item item)
        {
            var values = new Dictionary<string, string>
                {
                    { "id", item.Id.ToString() },
                    {"name", item.Name },
                    { "bought", item.Bought.ToString() }
                };
            var content = JsonConvert.SerializeObject(values);

            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = url
            };
            client.SendAsync(requestMessage).Wait();

            requestMessage.Dispose();
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            AddItem();
        }

        private void bought_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Item t = (Item)((CheckBox)sender).BindingContext;
            List<Item> tempList = new List<Item>(items);
            foreach (Item item in tempList)
            {
                if (t.Bought == item.Bought && t.Name != item.Name)
                {
                    items.Move(items.IndexOf(t), items.IndexOf(item));
                    return;
                }
            }
            if(t.Bought)
            {
                items.Move(items.IndexOf(t), items.Count-1);
            }
            else
            {
                items.Move(items.IndexOf(t), 0);
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            List<Item> itemsList = new List<Item>(items);
            foreach (Item item in itemsList)
            {
                if(item.Bought)
                {
                    DeleteItem(item);

                    items.Remove(item);
                }
            }
        }
    }
}
