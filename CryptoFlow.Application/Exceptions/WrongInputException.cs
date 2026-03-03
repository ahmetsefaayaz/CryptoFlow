namespace CryptoFlow.Application.Exceptions;

public class WrongInputException: Exception
{
    public WrongInputException(string message) : base(message) {}
}