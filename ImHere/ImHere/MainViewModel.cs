using System.Threading.Tasks;
using System.Windows.Input;
using System.Text;
using Xamarin.Essentials;
using ImHere.Data;
using Newtonsoft.Json;
using System.Net.Http;
using Xamarin.Forms;

namespace ImHere
{
    public class MainViewModel : BaseViewModel
    {
        string message = "";
        public string Message
        {
            get => message;
            set => Set(ref message, value);
        }

        string phoneNumbers = "";
        public string PhoneNumbers
        {
            get => phoneNumbers;
            set => Set(ref phoneNumbers, value);
        }

        public MainViewModel()
        {
            SendLocationCommand = new Command(async () => await SendLocation());
        }

        public ICommand SendLocationCommand { get; }

        HttpClient client = new HttpClient();
        const string baseUrl = "http://localhost:7071";

        async Task SendLocation()
        {
            Location location = await Geolocation.GetLastKnownLocationAsync();

            if (location != null)
            {
                Message = $"Location found: {location.Latitude}, {location.Longitude}.";

                PostData postData = new PostData
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    ToNumbers = PhoneNumbers.Split('\n')
                };

                string data = JsonConvert.SerializeObject(postData);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.PostAsync($"{baseUrl}/api/SendLocation",
                                                                    content);

                if (result.IsSuccessStatusCode)
                    Message = "Location sent successfully";
                else
                    Message = $"Error - {result.ReasonPhrase}";
            }
        }
    }
}