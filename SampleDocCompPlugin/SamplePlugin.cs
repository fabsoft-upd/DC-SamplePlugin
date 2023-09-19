using Document_Companion.Api;
using Document_Companion.CommonDialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SampleDocCompPlugin
{
    public class SamplePlugin : IPlugin
    {
        private const char NonBreakingSpace = (char)0x00A0;

        private static string _AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        private IDocumentCompanion _DocumentCompanion = null;
        private string _SerializedSettings = null;
        private SamplePluginsSettings _Settings = null;

        public void Initialize(IDocumentCompanion docComp)
        {
            _DocumentCompanion = docComp;
            _DocumentCompanion.TryGetSetting("SamplePluginsSettings", out string serializedSettings);
            _SerializedSettings = serializedSettings;
            _Settings = SamplePluginsSettings.Load(serializedSettings);
            if (string.IsNullOrWhiteSpace(serializedSettings))
            {
                _SerializedSettings = _Settings.Serialize();
            }
        }

        private static Image LoadImage(string imageUri)
        {
            Image image = new Image() { Height = 16, Width = 16 };

            // Create and set the ImageSource
            BitmapImage bitmapImage = new BitmapImage(new Uri(imageUri)) { CacheOption = BitmapCacheOption.OnLoad };
            image.Source = bitmapImage;
            return image;
        }

        public List<MenuItem> GetSingleFileMenu()
        {
            var menuEntry = new MenuItem() { Header = "Sample Single File Menu Entry", Icon = LoadImage($"pack://application:,,,/{_AssemblyName};component/images/arrow.png") };
            menuEntry.Click += new System.Windows.RoutedEventHandler((object sender, RoutedEventArgs args) => {
                MessageBoxDialogWpf.Show(_DocumentCompanion.MainWindow, $"File Selected: {string.Join("\r\n", _DocumentCompanion.GetSelectedPageNumbers())}", _DocumentCompanion.MainWindow.Title, MessageBoxDialogWpf.DialogButtons.Ok, MessageBoxDialogWpf.DialogIcons.Information);
            });
            return new List<MenuItem>() { menuEntry };
        }

        public List<MenuItem> GetMultiFileMenu()
        {
            var menuEntry = new MenuItem() { Header = "Sample Multi File Menu Entry", Icon = LoadImage($"pack://application:,,,/{_AssemblyName};component/images/arrow.png") };
            menuEntry.Click += new System.Windows.RoutedEventHandler((object sender, RoutedEventArgs args) => {
                MessageBoxDialogWpf.Show(_DocumentCompanion.MainWindow, $"Files Selected: {string.Join("\r\n", _DocumentCompanion.GetSelectedPageNumbers())}", _DocumentCompanion.MainWindow.Title, MessageBoxDialogWpf.DialogButtons.Ok, MessageBoxDialogWpf.DialogIcons.Information);
            });
            return new List<MenuItem>() { menuEntry };
        }

        public List<MenuItem> GetSinglePageMenu()
        {
            var menuEntry = new MenuItem() { Header = "Sample Single Page Menu Entry", Icon = LoadImage($"pack://application:,,,/{_AssemblyName};component/images/arrow.png") };
            menuEntry.Click += new System.Windows.RoutedEventHandler((object sender, RoutedEventArgs args) => {
                MessageBoxDialogWpf.Show(_DocumentCompanion.MainWindow, $"Page Selected: {string.Join("\r\n", _DocumentCompanion.GetSelectedPageNumbers())}", _DocumentCompanion.MainWindow.Title, MessageBoxDialogWpf.DialogButtons.Ok, MessageBoxDialogWpf.DialogIcons.Information);
            });
            return new List<MenuItem>() { menuEntry };
        }

        public List<MenuItem> GetMultiPageMenu()
        {
            var menuEntry = new MenuItem() { Header = "Sample Multi Page Menu Entry", Icon = LoadImage($"pack://application:,,,/{_AssemblyName};component/images/arrow.png") };
            menuEntry.Click += new System.Windows.RoutedEventHandler((object sender, RoutedEventArgs args) => {
                MessageBoxDialogWpf.Show(_DocumentCompanion.MainWindow, $"Pages Selected: {string.Join("\r\n", _DocumentCompanion.GetSelectedPageNumbers())}", _DocumentCompanion.MainWindow.Title, MessageBoxDialogWpf.DialogButtons.Ok, MessageBoxDialogWpf.DialogIcons.Information);
            });
            return new List<MenuItem>() { menuEntry };
        }

        public List<SettingsPanel> GetSettingsMenuScreens()
        {

            var panel = new SettingsPanel() { Title = "Sample Settings", Control = new SampleSettingsPanel(_Settings) };

            panel.Save = new SaveAction((out bool applicationRestartRequired) =>
            {
                applicationRestartRequired = false;

                string settings = _Settings.Serialize();
                _DocumentCompanion.PutUserSetting("SamplePluginsSettings", settings);
                _SerializedSettings = settings;
                panel.HasUnsavedChanges = false;
            });

            _Settings.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
                string settings = _Settings.Serialize();
                panel.HasUnsavedChanges = settings != _SerializedSettings;
            });

            return new List<SettingsPanel>() { panel };
        }

        public List<ActionOptions> GetActionButtons()
        {
            return new List<ActionOptions>()
            {
                new SingleActionButton()
                {
                    ActionButton = new ActionButton()
                    {
                        Text = "Sample Action".Replace(' ', NonBreakingSpace),
                        RequiresOpenDocument = false,
                        Tooltip = "Sample Plugin Action Logic",
                        Icon = $"pack://application:,,,/{_AssemblyName};component/images/up.png",
                        ClickAction = new Action(() =>
                        {
                            var dlg = new Microsoft.Win32.OpenFileDialog();
                            if (dlg.ShowDialog().Value)
                            {
                                List<string> imported = _DocumentCompanion.ImportDocuments(new List<string>(){ dlg.FileName }, false);
                                if (imported != null && imported.Count >0)
                                {
                                    _DocumentCompanion.OpenDocument(imported[0]);
                                }
                            }
                        })
                    }
                }
            };
        }

        public List<ActionOptions> GetSubmitButtons()
        {
            return new List<ActionOptions>() 
            { 
                new SingleActionButton() 
                {
                    ActionButton = new ActionButton() 
                    { 
                        Text = "Sample Submit - PDF".Replace(' ', NonBreakingSpace), 
                        RequiresOpenDocument = true, 
                        Tooltip = "Sample Plugin Submit PDF Logic",
                        Icon = $"pack://application:,,,/{_AssemblyName};component/images/down.png",
                        ClickAction = new Action(() => 
                        {
                            using(FileStream fs = new FileStream($@"C:\temp\{Guid.NewGuid()}.pdf", FileMode.Create, FileAccess.Write))
                            {
                                _DocumentCompanion.SaveOpenedDocumentAsPdf(fs, false);
                            }
                        }) 
                    } 
                },
                new SingleActionButton()
                {
                    ActionButton = new ActionButton()
                    {
                        Text = "Sample Submit - TIFF".Replace(' ', NonBreakingSpace),
                        RequiresOpenDocument = true,
                        Tooltip = "Sample Plugin Submit TIFF Logic",
                        Icon = $"pack://application:,,,/{_AssemblyName};component/images/down.png",
                        ClickAction = new Action(() =>
                        {
                            using(FileStream fs = new FileStream($@"C:\temp\{Guid.NewGuid()}.tiff", FileMode.Create, FileAccess.Write))
                            {
                                _DocumentCompanion.SaveOpenedDocumentAsTiff(fs);
                            }
                        })
                    }
                }
            };
        }

    }
}
