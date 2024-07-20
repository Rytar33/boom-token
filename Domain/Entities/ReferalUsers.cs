namespace Domain.Entities;

public class ReferalUsers : BaseEntity
{
    public ReferalUsers(Guid idUser, Guid idUserInvited)
    {
        IdUser = idUser;
        IdUserInvited = idUserInvited;
        CountTakeFromClick = 0;
    }

    public long CountTakeFromClick { get; private set; }

    public Guid IdUser { get; private set; }

    public Guid IdUserInvited { get; private set; }

    public User? User { get; private set; }
    
    public User? UserInvited { get; private set; }

    public void Update(long? countTakeFromClick = null)
    {
        if (countTakeFromClick != null)
            CountTakeFromClick = countTakeFromClick.Value;
    }
}
