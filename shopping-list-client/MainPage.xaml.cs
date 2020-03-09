using shopping_list_client.Model;
using System;
using System.ComponentModel;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;
using System.Text;
using System.Globalization;
using System.Threading;

namespace shopping_list_client
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private static readonly Uri url = new Uri(@"https://shopping-list-server-1.herokuapp.com/");
        private static readonly HttpClient client = new HttpClient() { BaseAddress = url };

        private ObservableCollection<Item> items = new ObservableCollection<Item>();
        private List<Item> serverItems = new List<Item>();
        private bool threadStarted = false;

        public MainPage()
        {
            InitializeComponent();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            GetItems();
            itemsListView.ItemsSource = items;

            foreach (Item item in items)
            {
                Item i = new Item(item);
                serverItems.Add(i);
            }
        }

        private void GetItems()
        {
            string response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;

            items = JsonConvert.DeserializeObject<ObservableCollection<Item>>(response);
            //var itemsList = JsonConvert.DeserializeObject<List<Item>>(response);
            //itemsList.Sort();
            //foreach (Item item in itemsList)
            //{
            //    items.Add(item);
            //}
        }

        private void AddItem(string item)
        {
            var values = new Dictionary<string, string>
                {
                    {"name", item }
                };
            var content = new FormUrlEncodedContent(values);
            var response = client.PostAsync(url, content).Result.Content.ReadAsStringAsync().Result;
            content.Dispose();

            items.Add(JsonConvert.DeserializeObject<Item>(response));
        }

        private void DeleteItem(Item item)
        {
            var values = new Dictionary<string, string>
                {
                    { "id", item.Id.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) },
                    {"name", item.Name },
                    { "bought", item.Bought.ToString(CultureInfo.CreateSpecificCulture("en-US")) }
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

        private void UpdateItems()
        {
            Thread.Sleep(5000);

            foreach(Item item in items)
            {
                foreach (Item serverItem in serverItems)
                {
                    if(item.Id == serverItem.Id)
                    {
                        if(item.Bought != serverItem.Bought)
                        {
                            var values = new Dictionary<string, string>
                                {
                                    { "id", item.Id.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) },
                                    {"name", item.Name },
                                    { "bought", item.Bought.ToString(CultureInfo.CreateSpecificCulture("en-US")) }
                                };
                            var content = JsonConvert.SerializeObject(values);

                            HttpRequestMessage requestMessage = new HttpRequestMessage
                            {
                                Content = new StringContent(content, Encoding.UTF8, "application/json"),
                                Method = HttpMethod.Put,
                                RequestUri = url
                            };
                            client.SendAsync(requestMessage).Wait();
                            requestMessage.Dispose();
                        }
                        break;
                    }
                }
            }
            threadStarted = false;
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            string item = itemName.Text;
            if (item != null && item.Length > 0)
            {
                AddItem(item);
            }
            itemName.Text = "";
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

            if(!threadStarted)
            {
                threadStarted = true;
                Thread updatethread = new Thread(new ThreadStart(UpdateItems));
                updatethread.Start();
            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            for(int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if(item.Bought)
                {
                    //DeleteItem(item);
                    items.Remove(item);
                }
            }
        }
    }
}
