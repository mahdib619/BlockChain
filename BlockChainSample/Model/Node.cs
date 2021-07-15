using System;
using System.Collections.Generic;

namespace BlockChainSample.Model
{
    public class Node
    {
        public Node()
        {
            Id = Guid.NewGuid().ToString();
            RegisteredNodes = new HashSet<string>();
            BlockChain = new BlockChain(new Block{PreviousHash = "",Proof = 0});
        }
        public string Id { get; set; }
        public ICollection<string> RegisteredNodes { get; set; }
        public BlockChain BlockChain { get; set; }
    }
}
