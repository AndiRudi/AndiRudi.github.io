using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class EFTransactionInterceptor : DbTransactionInterceptor
{
    public override void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData)
    {
        base.TransactionCommitted(transaction, eventData);
        Console.WriteLine("Committed");
    }
}