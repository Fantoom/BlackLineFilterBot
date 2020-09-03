using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BlackLineFilterBot.Commands
{
    public interface ICommand
    {
        public string CommandName { get; }

        Task ExecuteAsync(Message message);
    }
}