public class NotificationCreateViewModel
{
    public string Title { get; set; }
    public string Content { get; set; }
    public int? SendRole { get; set; }
    public List<UserSelection> Users { get; set; }
}

public class UserSelection
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public bool IsSelected { get; set; }
}
