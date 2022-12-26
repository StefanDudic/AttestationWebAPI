namespace AttestationProject.Exceptions
{
    public class GuidNotValidException : Exception
    {
        public GuidNotValidException() { }
        public GuidNotValidException(string guid) : base(string.Format("Following entry is not a valid Guid: {0}", guid.ToString()))
        {

        }
    }
}
