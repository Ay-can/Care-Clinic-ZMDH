using System.Collections.Generic;

public class UserRoleViewModel 
{
    public string UserId {get;set;}
    public string Email {get;set;}
    public IList<string> Roles {get;set;}
}