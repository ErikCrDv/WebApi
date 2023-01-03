namespace WebApi.Services
{
    public class WriteFile : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nameFile = "file1.txt";
        private Timer timer;

        public WriteFile(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Write("START PROCESS");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Write("END PROCESS");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Write("RUNNING PROCESS: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        private void Write(string msg)
        {
            var route = $@"{env.ContentRootPath}\wwwroot\{nameFile}";
            using (StreamWriter writer = new StreamWriter(route, append: true))
            {
                writer.WriteLine(msg);
            }
        }
    }
}
