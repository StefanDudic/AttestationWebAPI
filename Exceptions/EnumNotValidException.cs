namespace AttestationProject.Exceptions
{
    public class EnumNotValidException : Exception
    {
        public EnumNotValidException() { }
        public EnumNotValidException(string enumerator) : base(string.Format("Following entry is not a valid Guid: {0}", enumerator.ToString()))
        {

        }
    }
}
