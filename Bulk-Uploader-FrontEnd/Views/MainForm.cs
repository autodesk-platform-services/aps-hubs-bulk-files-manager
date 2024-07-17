using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using Log = Serilog.Log;

namespace Bulk_Uploader_Electron
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            AdditionalInitialization();
        }

        private async Task AdditionalInitialization()
        {
            WindowState = FormWindowState.Normal;
            await this.webView21.EnsureCoreWebView2Async(null);
            
            this.webView21.CoreWebView2.WebMessageReceived += WebMessageReceived;
        }

        private void WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                // Handle the incoming message from the browser's JavaScript code.
                // The message data is available in the 'e' parameter.
                string message = e.TryGetWebMessageAsString();
                // Process the message data as needed.
                Console.WriteLine(message);

                switch (message)
                {
                    case "getFolderPath":
                        try
                        {
                            // https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/threading-model#reentrancy
                            System.Threading.SynchronizationContext.Current.Post((_) =>
                            {
                                using (FolderBrowserDialog openDialog = new FolderBrowserDialog())
                                {
                                    if (openDialog.ShowDialog() == DialogResult.OK)
                                    {
                                        string selectedPath = openDialog.SelectedPath;
                                        this.webView21.CoreWebView2.PostWebMessageAsJson(
                                            JsonConvert.SerializeObject(
                                                new
                                                {
                                                    type = message,
                                                    data = selectedPath
                                                })
                                        );
                                    }
                                }
                            }, null);
                        }
                        catch (Exception exception)
                        {
                            Log.Error(exception.Message);
                        }

                        break;
                    case "getFilePath":
                        try
                        {
                            // https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/threading-model#reentrancy
                            System.Threading.SynchronizationContext.Current.Post((_) =>
                            {
                                using (OpenFileDialog openDialog = new OpenFileDialog() { FileName = "test.txt" })
                                {
                                    if (openDialog.ShowDialog() == DialogResult.OK)
                                    {
                                        string selectedPath = openDialog.FileName;
                                        this.webView21.CoreWebView2.PostWebMessageAsJson(
                                            JsonConvert.SerializeObject(
                                                new
                                                {
                                                    type = message,
                                                    data = selectedPath
                                                })
                                        );
                                    }
                                }
                            }, null);
                        }
                        catch (Exception exception)
                        {
                            Log.Error(exception.Message);
                        }

                        break;
                }

                // using (OpenFileDialog openFileDialog = new OpenFileDialog())
                // {
                //     if (openFileDialog.ShowDialog() == DialogResult.OK)
                //     {
                //         // The user selected a file, and its path will be in openFileDialog.FileName
                //         string selectedFilePath = openFileDialog.FileName;
                //
                //         // Handle the selected file path as needed (e.g., display it in a label)
                //         Console.WriteLine(selectedFilePath);
                //         
                //         this.webView21.CoreWebView2.PostWebMessageAsString(selectedFilePath);
                //     }
                // }
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message);
            }
        }
    }
}