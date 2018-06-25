namespace Service.Common
{
    public class ManagementService : IManagementService
    {
        public string GetImportantValue()
        {
            return $"Hash: {this.GetHashCode()}";
        }
    }
}