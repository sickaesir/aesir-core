using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AesirLightsUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        MessageWebSocket client;
        bool connected;
        DataWriter writer;
        ColorPicker colorPicker;
        ComboBox effectCombo;
        SecondaryTile colorTile;
        private TileUpdater tileUpdater;
        public Color CurrentLedsColor;
        Color lastSetLedColor = Color.FromArgb(0, 0, 0, 0);
        private ApplicationView view;

        private T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var childType = child as T;
                if (childType == null)
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        public MainPage()
        {
            client = new MessageWebSocket();
            client.Control.MessageType = SocketMessageType.Utf8;
            client.Closed += Client_Closed;
            client.MessageReceived += Client_MessageReceived;
            this.Loaded += MainPage_Loaded;
          
            this.InitializeComponent();
            tileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile("color");
            colorTile = new SecondaryTile("colorTile", "Aesir Lights", "action=view", new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"), Windows.UI.StartScreen.TileSize.Default);
        }

        private async void Client_Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            view.Title = "Connecting...";
            connected = false;
            await Connect();
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            view = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            view.Title = "Connecting...";
            var tiles = await SecondaryTile.FindAllAsync();
            var tile = tiles.FirstOrDefault(k => k.TileId == "colorTile");
            if (tile == null)
            {
                tile = colorTile;
                await tile.RequestCreateAsync();
            }

            colorPicker = FindChild<ColorPicker>(this, "ColorPicker");
            effectCombo = FindChild<ComboBox>(this, "EffectType");

            await Connect();

        }

        private async Task UpdateTile(Color color)
        {

            var tiles = await SecondaryTile.FindAllAsync();
            var tile = tiles.FirstOrDefault(k => k.TileId == "colorTile");
            if (tile == null)
            {
                return;
            }

            tile.VisualElements.BackgroundColor = color;

            await tile.UpdateAsync();
        }

        private async void Client_MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader dataReader = args.GetDataReader())
                {
                    dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(message);

                    if(data.notify != null)
                    {
                        switch((string)data.notify)
                        {
                            case "rgb_changed":
                                {
                                    CurrentLedsColor = Color.FromArgb(255, (byte)data.ledsRgb.r, (byte)data.ledsRgb.g, (byte)data.ledsRgb.b);
                                    if (CurrentLedsColor == lastSetLedColor) return;
                                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                    {

                                        colorPicker.ColorChanged -= OnColorChange;
                                        colorPicker.Color = CurrentLedsColor;
                                        colorPicker.ColorChanged += OnColorChange;
                                        lastSetLedColor = CurrentLedsColor;


                                    });
                                   // await UpdateTile(CurrentLedsColor);
                                    //canvas.Background = ;
                                }
                                break;

                            case "effect_changed":
                                {
                                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                    {
                                        effectCombo.SelectionChanged -= OnEffectChange;
                                        if (effectCombo.SelectedIndex != (int)data.effectType)
                                            effectCombo.SelectedIndex = (int)data.effectType;
                                        effectCombo.SelectionChanged += OnEffectChange;

                                    });
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                connected = false;
                view.Title = "Connecting";
                await Connect();
            }
        }

        private async Task Connect()
        {
            try
            {
                if(writer != null)
                {
                    writer.Dispose();
                    writer = null;
                }

                await client.ConnectAsync(new Uri("ws://localhost:8000"));
                writer = new DataWriter(this.client.OutputStream);
                view.Title = "Connected";
                connected = true;
            }
            catch(Exception)
            {
                await Task.Delay(3000);
                await Connect();
                return;
            }
        }

        private async Task SendMessageUsingMessageWebSocketAsync(string message)
        {
            if (writer == null) return;
            writer.WriteString(message);
            await writer.StoreAsync();
            /*
            using (var dataWriter = new DataWriter(this.client.OutputStream))
            {
                dataWriter.WriteString(message);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
            }*/
        }

        private Task WriteMessage(dynamic data)
        {
            return SendMessageUsingMessageWebSocketAsync(JsonConvert.SerializeObject(data));
        }

        private async void OnColorChange(ColorPicker sender, ColorChangedEventArgs args)
        {
            lastSetLedColor = CurrentLedsColor = Color.FromArgb(255, args.NewColor.R, args.NewColor.G, args.NewColor.B);
            try
            {
                await WriteMessage(new { cmd = "set_color", rgb = new { r = args.NewColor.R, g = args.NewColor.G, b = args.NewColor.B } });
            }
            catch(Exception)
            {

            }
            //Debug.WriteLine("Color Changed");

        }

        private async void OnEffectChange(object sender, SelectionChangedEventArgs e)
        {
            if (!connected) return;
            int effectType = 0;
            switch(e.AddedItems[0].ToString())
            {
                case "Music": effectType = 0; break;
                case "Static": effectType = 1; break;
            }
            await WriteMessage(new { cmd = "change_effect", type = effectType } );
        }
    }
}
