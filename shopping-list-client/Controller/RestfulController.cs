using Newtonsoft.Json;
using shopping_list_client.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Timers;

namespace shopping_list_client.Controller
{
    class RestfulController
    {
        private static readonly Uri url = new Uri(@"https://shopping-list-server-1.herokuapp.com/");
        private static readonly HttpClient client = new HttpClient() { BaseAddress = url };

        private const int INTERVAL = 2000;
        private System.Timers.Timer timer;

        public ObservableCollection<Item> items = new ObservableCollection<Item>();

        public RestfulController()
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            timer = new System.Timers.Timer(INTERVAL);
            timer.Elapsed += CountdownToPut;

            App.onAppClosing += UpdateItems;
        }

        private void CountdownToPut(Object source, ElapsedEventArgs e)
        {
            timer.Stop();
            UpdateItems();
        }

        public void GetItems()
        {
            string response = client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;

            var toSort = JsonConvert.DeserializeObject<List<Item>>(response);
            toSort.Sort();
            items = new ObservableCollection<Item>(toSort);
        }

        public void AddItem(string item)
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

        public void DeleteBought()
        {
            var values = new List<Dictionary<string, string>>();
            var toRemove = new List<Item>();
            foreach (Item item in items)
            {
                if (item.Bought)
                {
                    toRemove.Add(item);
                    values.Add(new Dictionary<string, string>{
                        { "id", item.Id.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) },
                        { "name", item.Name },
                        { "bought", item.Bought.ToString(CultureInfo.CreateSpecificCulture("en-US")) }
                    });
                }
            }
            var content = JsonConvert.SerializeObject(values);
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = url
            };
            client.SendAsync(requestMessage).Wait();
            requestMessage.Dispose();

            foreach (Item item in toRemove)
            {
                items.Remove(item);
            }
        }

        public void UpdateItems()
        {
            var values = new List<Dictionary<string, string>>();
            foreach (Item item in items)
            {
                values.Add(new Dictionary<string, string>
                {
                    { "id", item.Id.ToString("G", CultureInfo.CreateSpecificCulture("en-US")) },
                    { "name", item.Name },
                    { "bought", item.Bought.ToString(CultureInfo.CreateSpecificCulture("en-US")) }
                });
            }
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

        public void ItemBought(Item t)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (t.Bought == item.Bought && t.Id != item.Id)
                {
                    items.Move(items.IndexOf(t), i);
                    return;
                }
            }
            if (t.Bought)
            {
                items.Move(items.IndexOf(t), items.Count - 1);
            }
            else
            {
                items.Move(items.IndexOf(t), 0);
            }

            timer.Stop();
            timer.Start();
        }
    }
}
