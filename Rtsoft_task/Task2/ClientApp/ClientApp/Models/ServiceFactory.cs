namespace ClientApp.Models
{
    public static class ServiceFactory
    {
        public static AbstractItem Create(string serviceName)
        {
            switch (serviceName)
            {
                case "NewRemoteProcess": return  new Process();
                case "NewRemoteDbus":return  new Service();
            }

            return null;
        }
    }
}