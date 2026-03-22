namespace Hospital_Management_System.Abstractions;

public static class ProblemExtesctions
{
    public static ObjectResult ToProblem(this Result result,int code)
    {
        if (result.IsSuccess) throw new InvalidOperationException("Can't connver failuer to succes");
        var ResultPromble = Results.Problem(statusCode: code);
        var ResultPrombleDetailes = ResultPromble.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(ResultPromble) as ProblemDetails;
        ResultPrombleDetailes!.Extensions = new Dictionary<string, object?>
        {
            {
                "error",new[]{result.Error}
            }
        };
      
        return new ObjectResult(ResultPrombleDetailes);
    }
}
