namespace Hospital_Management_System.Authentication.Filter;

public class HasPermissionAttribute(string permission):AuthorizeAttribute(permission)
{

}
