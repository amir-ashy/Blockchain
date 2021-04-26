namespace Blockchain.Model
{
    public class Transaction
    {

        public Transaction() { }
        public Transaction(string from, string to, int amount)
        {
            From = from;
            To = to;
            Amount = amount;
        }
        public string From { get; set; }
        public string To { get; set; }
        public int Amount { get; set; }
    }
}
