namespace Domain.Entities;

public class ImprovementAccess : BaseEntity
{
    public ImprovementAccess(Guid idUser, Guid idImprovement)
    {
        IdUser = idUser;
        IdImprovement = idImprovement;
    }

    public Guid IdUser { get; private set; }

    public Guid IdImprovement { get; private set; }

    public User? User { get; private set; }

    public Improvement? Improvement { get; private set; }
}
