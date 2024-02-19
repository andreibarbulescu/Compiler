using System.Runtime.Serialization;

[Serializable]
internal class ParseException : Exception
{
    private object value;

    public ParseException()
    {
    }

    public ParseException(object value)
    {
        this.value = value;
    }

    public ParseException(string? message) : base(message)
    {
    }

    public ParseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}