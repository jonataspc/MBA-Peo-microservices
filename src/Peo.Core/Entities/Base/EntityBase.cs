namespace Peo.Core.Entities.Base
{
    public abstract class EntityBase
    {
        public Guid Id { get; init; } = Guid.CreateVersion7();

        public DateTime CreatedAt { get; private set; } = DateTime.Now;

        public DateTime? ModifiedAt { get; set; }

        protected EntityBase()
        {
        }
    }
}