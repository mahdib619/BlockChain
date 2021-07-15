using System.Collections.Generic;

namespace BlockChainSample.Model
{
    public class Block
    {
        public int Index { get; set; }
        public long TimeStamp { get; set; }
        public int Proof { get; set; }
        public string PreviousHash { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}