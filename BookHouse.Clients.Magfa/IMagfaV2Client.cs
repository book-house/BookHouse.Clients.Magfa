using BookHouse.Clients.Magfa.Models;
using System.Threading.Tasks;

namespace BookHouse.Clients.Magfa
{
    public interface IMagfaV2Client
    {
        Task<long> BalanceAsync();
        Task<MagfaMessage> SendAsync(string message, string recipients);
        Task<MagfaMessage[]> SendAsync(string[] messages, string[] recipients, long[] uids = null, string[] senders = null);
    }
}