namespace Blockchain.Model
{
    public class Transaction
    {

        public Transaction() { }
        public Transaction(string from, string to, int amount)
        {
            this.From = from;
            this.To = to;
            this.Amount = amount;
        }
        public string From { get; set; }
        public string To { get; set; }
        public int Amount { get; set; }
    }
}
