using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace SmartMirrorRpPi
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    
    public sealed partial class MainPage : Page
    {
        private GpioController gpio;
        int number = 0;
        private GpioPin sensor;
        private MediaCapture _mediaCapture;
        BackgroundWorker worker = new BackgroundWorker();
        BackgroundWorker worker2 = new BackgroundWorker();
        Faces persons = new Faces();
        FaceRecognition algorithm = new FaceRecognition();
        
        public MainPage()
        {
            beginRecognize();
            this.InitializeComponent();
            //Full screen mode

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            LoadNewsFeed();
            LoadWeatherData();
            Application.Current.Resuming += Application_Resuming;
            

            InitGPIO();

        }

        private async void Application_Resuming(object sender, object o)
        {

        }


        private void InitGPIO()
        {
            gpio = GpioController.GetDefault();
            if (gpio == null)
                return;
            sensor = gpio.OpenPin(16);
            sensor.SetDriveMode(GpioPinDriveMode.Input);
            sensor.ValueChanged += Sensor_ValueChanged;

        }

        private async void Sensor_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.RisingEdge)
            {
                System.Diagnostics.Debug.WriteLine("MOTION DETECTED");

               
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     Greetings.Text = "Move!" + number;
                     number++;
                     // MakePhoto();
                 });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("NO MOTION DETECTED");
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () =>
                 {
                     
                 });
            }
        }
        private async Task beginRecognize()
        {
            await persons.DownloadFaces();
            
            algorithm.LearnAlgorithm(persons.imageList);

            InitializeCameraAsync();

        }
        private async void LoadWeatherData()
        {
            string myCity = "Wroclaw";
            string responseXML;
            WeatherDataEntry result;

            while (true)
            {
                responseXML = await WeatherApiConnection.LoadDataAsync(myCity);
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseXML)))
                {
                    result = ParseWeatherLinq.Parse(stream);
                    if (worker.IsBusy != true)
                        worker.RunWorkerAsync();
                }
               
                CityTextBox.Text = result.City.ToString();
                TemperatureTextBox.Text = "Temperature: " + (result.Temperature -273.15).ToString("0.0") + " °C";
                WindTextBox.Text = "Wind: " + (result.Wind).ToString() + " mps";
                HumidityTextBox.Text = "Humidity: " + (result.Humidity).ToString() + " %";
                PressureTextBox.Text = "Pressure: " + (result.Pressure).ToString() + " hPa";
                WeatherIcon.Source = new BitmapImage( new Uri("http://openweathermap.org/img/w/" + result.weatherIcon.ToString() + ".png", UriKind.Absolute));

                await Task.Delay(300000);
            }

        }

        private async void LoadNewsFeed()
        {

            string responseJson;
            NewsData result;

            while (true)
            {
                responseJson = await NewsApiConnection.LoadDataAsync();
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseJson)))
                {
                    result = ParseNews.Parse(stream);
                    if (worker2.IsBusy != true)
                        worker2.RunWorkerAsync();
                }
                TitleTextBlock.Text = result.title.ToString();
                DescriptionTextBlock.Text = result.description.ToString();
                UrlTextBlock.Text = result.url.ToString();

                await Task.Delay(100000);
            }

        }
        private async Task InitializeCameraAsync()
        {
            if (_mediaCapture == null)
            {
                // Get the camera devices
                var cameraDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

                // try to get the back facing device for a phone
                var backFacingDevice = cameraDevices
                    .FirstOrDefault(c => c.EnclosureLocation?.Panel == Windows.Devices.Enumeration.Panel.Back);

                // but if that doesn't exist, take the first camera device available
                var preferredDevice = backFacingDevice ?? cameraDevices.FirstOrDefault();

                // Create MediaCapture
                _mediaCapture = new MediaCapture();

                // Initialize MediaCapture and settings
                await _mediaCapture.InitializeAsync(
                    new MediaCaptureInitializationSettings
                    {
                        VideoDeviceId = preferredDevice.Id
                    });

                // Set the preview source for the CaptureElement
  

                // Start viewing through the CaptureElement 
                await _mediaCapture.StartPreviewAsync();
            }
        }

        private async void MakePhoto()
        {
            // This is where we want to save to.
            var storageFolder = KnownFolders.SavedPictures;
            
            //var file = await storageFolder.CreateFileAsync("Test" + ".png", CreationCollisionOption.ReplaceExisting);

            IReadOnlyList<StorageFile> files = await storageFolder.GetFilesAsync();
            if (files.Count == 0)
            {
                var file = await storageFolder.CreateFileAsync("Test" + ".png", CreationCollisionOption.ReplaceExisting);
                await _mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreatePng(), file);
            }
            else
            {
                var file = await storageFolder.CreateFileAsync("Test" + ".png", CreationCollisionOption.ReplaceExisting);
                // var file = await storageFolder.CreateFileAsync((Int32.Parse((files[files.Count - 1].DisplayName)) + 1).ToString() + ".png", CreationCollisionOption.ReplaceExisting);
                await _mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreatePng(), file);
                await persons.addNewFace();

                Greetings.Text = persons.getName(algorithm.RecognizeNewFace(persons.newFace));

                await file.DeleteAsync(StorageDeleteOption.Default);
            }


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MakePhoto();
        }
    }


}
