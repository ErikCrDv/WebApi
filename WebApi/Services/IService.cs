namespace WebApi.Services
{
    public interface IService
    {
        void DoTask();
        Guid GetScoped();
        Guid GetSingleton();
        Guid GetTransient();
    }

    public class ServiceA : IService
    {
        private readonly ILogger<ServiceA> logger;
        private readonly ServiceTransiet serviceTransiet;
        private readonly ServiceScoped serviceScoped;
        private readonly ServiceSingleton serviceSingleton;

        public ServiceA(
            ILogger<ServiceA> logger,
            ServiceTransiet serviceTransiet,
            ServiceScoped serviceScoped,
            ServiceSingleton serviceSingleton
            )
        {
            this.logger = logger;
            this.serviceTransiet = serviceTransiet;
            this.serviceScoped = serviceScoped;
            this.serviceSingleton = serviceSingleton;
        }

        public Guid GetTransient() {  return serviceTransiet.Guid; }
        public Guid GetScoped() { return serviceScoped.Guid; }
        public Guid GetSingleton() { return serviceSingleton.Guid; }

        public void DoTask()
        {
            //throw new NotImplementedException();
        }
    }

    public class ServiceB : IService
    {
        public void DoTask()
        {
            //throw new NotImplementedException();
        }

        public Guid GetScoped()
        {
            throw new NotImplementedException();
        }

        public Guid GetSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid GetTransient()
        {
            throw new NotImplementedException();
        }
    }

    public class ServiceTransiet
    {
        public Guid Guid = Guid.NewGuid();
    }
    public class ServiceScoped
    {
        public Guid Guid = Guid.NewGuid();
    }
    public class ServiceSingleton
    {
        public Guid Guid = Guid.NewGuid();
    }
}
