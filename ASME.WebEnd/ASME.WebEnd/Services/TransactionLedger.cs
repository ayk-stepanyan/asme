using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using ASME.WebEnd.Entities;

namespace ASME.WebEnd.Services
{
    public interface ITransactionLedger
    {
        void OrderExecuted(Transaction transaction);

        Transaction[] FindTransactions(DateTimeOffset from, DateTimeOffset till);
    }

    public class TransactionLedger : ITransactionLedger
    {
        private readonly BlockingCollection<Transaction> _processingQueue = new BlockingCollection<Transaction>();
        private readonly ConcurrentBag<Transaction> _transactions = new ConcurrentBag<Transaction>();

        public TransactionLedger()
        {
            Task.Run(() =>
            {
                foreach (var transactions in _processingQueue.GetConsumingEnumerable())
                {
                    _transactions.Add(transactions);
                }
            });
        }

        public void OrderExecuted(Transaction transaction)
        {
            _processingQueue.Add(transaction);
        }

        public Transaction[] FindTransactions(DateTimeOffset @from, DateTimeOffset till)
        {
            return _transactions.ToArray().Where(t => t.Timestamp >= @from).Where(t => t.Timestamp <= till).ToArray();
        }
    }
}