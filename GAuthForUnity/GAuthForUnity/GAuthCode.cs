namespace GAuthForUnity
{
    public class GAuthCode
    {
        public string Code { get; private set; }

        public GAuthCode(string code)
        {
            Code = code;
        }
    }
}