namespace DarkPatterns.OneTimePassword.Commands;

public interface ICommand<out TResult, in TContext> where TResult : Task
{
	TResult Execute(TContext context);
}
