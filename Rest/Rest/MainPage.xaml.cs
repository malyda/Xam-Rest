using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using Rest.Model;
using Xamarin.Forms;

namespace Rest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            ShowPosts();
        }

        public async void ShowPosts()
        {
            string json = await GetPostsJsonTask();
            PostsListView.ItemsSource = await ParsePostJsonTask(json);
        }

        /// <summary>
        /// Parse Json string via task - no lag on UI
        /// </summary>
        /// <param name="json">Json string of Posts</param>
        /// <returns>Collection of parsed objects</returns>
        /// <exception cref="HttpRequestException">If device could not connect ie. Internet access denied or Status code is not Success</exception>
        public async Task<ObservableCollection<Post>> ParsePostJsonTask(string json)
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<ObservableCollection<Post>>(json));
        }

        /// <summary>
        /// Download data from API via background Task
        /// </summary>
        /// <returns>Json string from API</returns>
        public async Task<string> GetPostsJsonTask()
        {
            var client = new HttpClient();
            var uri = new Uri("http://jsonplaceholder.typicode.com/posts/");

            string content = await Task.Run(async () =>
            {
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();
                throw new HttpRequestException("Communication with server failed");
            });
            return content;
        }

        /// <summary>
        /// GETs data from API in synchronous way, lags UI thread
        /// Just for education purpose
        /// </summary>
        public void GetData()
        {
            HttpClient client = new HttpClient(new NativeMessageHandler());
            var uri = new Uri("https://jsonplaceholder.typicode.com/posts/");

            var response = client.GetAsync(uri).Result;
            string content = response.Content.ReadAsStringAsync().Result;
        }
    }
}
