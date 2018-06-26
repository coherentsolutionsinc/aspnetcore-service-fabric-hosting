namespace Service.Common
{
    public class SharedService : ISharedService
    {
        public string GetSharedValue()
        {
            return $"Hash: {this.GetHashCode()}";
        }
    }
}