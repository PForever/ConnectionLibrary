namespace ConnectionLibrary.Abstract.Modules.MessageManager.Clients.Protocoles.Containers
{
    public struct Addresses
    {
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public Addresses(string macAddress, string ipAddress)
        {
            MacAddress = macAddress;
            IpAddress = ipAddress;
        }
    }
}