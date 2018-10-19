namespace Service
{
    public class PersonalService : IPersonalService
    {
        public string GetPersonalValue()
        {
            return $"Hash: {this.GetHashCode()}";
        }
    }
}