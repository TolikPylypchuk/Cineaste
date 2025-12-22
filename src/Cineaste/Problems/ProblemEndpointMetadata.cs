namespace Cineaste.Problems;

public class ProblemEndpointMetadata(Func<Exception> exceptionFunc)
{
    public Exception Exception => exceptionFunc();
}
