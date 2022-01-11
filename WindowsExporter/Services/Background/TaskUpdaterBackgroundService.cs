namespace WindowsExporter.Services.Background
{
    public class TaskUpdaterBackgroundService : BackgroundService
    {
        private readonly IExporterTask[] _tasks;

        public TaskUpdaterBackgroundService(IExporterTask[] tasks)
        {
            this._tasks = tasks;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    
                    foreach(var task in _tasks)
                    {
                        try
                        {
                            task.Update();
                        }
                        catch(Exception e)
                        {
                            //Log if needed
                        }
                    }
                    await Task.Delay(5 * 60 * 1000);
                }
            });

            return Task.CompletedTask;
        }
    }
}
