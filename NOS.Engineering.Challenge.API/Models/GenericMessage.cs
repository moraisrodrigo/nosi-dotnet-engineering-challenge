
namespace NOS.Engineering.Challenge.API.Models;

public class GenericMessage
{
    public GenericMessage()
    {
        Message = string.Empty;
    }

    public GenericMessage(string message)
    {
        Message = message;
    }

    public string Message { get; set; }

}

