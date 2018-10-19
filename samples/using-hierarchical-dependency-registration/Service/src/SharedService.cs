namespace Service
{
    public class SharedService : ISharedService
    {
        public string GetSharedValue()
        {
            return $"Hash: {this.GetHashCode()}";
        }
    }
}