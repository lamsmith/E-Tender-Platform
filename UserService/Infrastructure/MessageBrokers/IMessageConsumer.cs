namespace UserService.Infrastructure.MessageBrokers
{
    public interface IMessageConsumer<in T>
    {
        Task ConsumeAsync(T message);
    }
}