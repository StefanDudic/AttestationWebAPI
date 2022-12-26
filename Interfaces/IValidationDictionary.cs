namespace AttestationProject.Interfaces
{
    public interface IValidationDictionary
    {
        void AddError(string key, string errorMessage);
        bool IsValid { get; }
        string Errors();
    }
}
