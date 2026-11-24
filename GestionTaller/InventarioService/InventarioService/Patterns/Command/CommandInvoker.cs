namespace InventarioService.Patterns.Command
{
    public class CommandInvoker
    {
        public async Task<TResult> EjecutarAsync<TResult>(ICommand<TResult> command)
        {
            return await command.ExecuteAsync();
        }
    }
}
