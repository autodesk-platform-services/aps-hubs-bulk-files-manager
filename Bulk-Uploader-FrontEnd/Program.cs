namespace Bulk_Uploader_Electron
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().RunAsync();
            
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            
            //Handle random errors without crashing the application
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                if ((e == null) || (e.Exception == null))
                {
                    return;
                }
                
                using (var sw = File.AppendText(@".\exceptions.txt")) 
                {
                    sw.WriteLine(e.Exception);
                }                
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if ((e == null) || (e.ExceptionObject == null))
                {
                    return;
                }
                
                using (var sw = File.AppendText(@".\exceptions.txt")) 
                {
                    sw.WriteLine(e.ExceptionObject);
                }                
            };
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    // webBuilder.UseElectron(args);
                    webBuilder.UseStartup<Startup>();
                    
                    webBuilder.UseUrls("http://localhost:8083");
                });
    }
}
