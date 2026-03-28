namespace Hospital_Management_System.Helpers;

public class EmailBodyBuilder
{
    public static string GenerateEmailBody(string Template, Dictionary<string, string> TemplateModel)
    {
        // Path For Template 
        var TemplatePath = $"{Directory.GetCurrentDirectory()}/Templates/{Template}.html";
        // Read Form that Path :
        var streamReader = new StreamReader(TemplatePath);
        var body = streamReader.ReadToEnd();
        streamReader.Close();

        foreach (var item in TemplateModel)
        {
            body = body.Replace(item.Key, item.Value);
        }


        return body;
    }
}
