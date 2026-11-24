namespace InventarioService.Patterns.Command
{
    public interface ICommand<TResult>
    {
        Task<TResult> ExecuteAsync();
    }
}
