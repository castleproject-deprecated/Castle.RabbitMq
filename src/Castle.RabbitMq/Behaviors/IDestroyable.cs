namespace Castle.RabbitMq
{
    using System.Threading.Tasks;

    public interface IDestroyable
    {
        void Delete();
        Task DeleteAsync();
    }
}