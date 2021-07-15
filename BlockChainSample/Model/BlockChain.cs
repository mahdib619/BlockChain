using System;
using System.Collections.Generic;
using System.Linq;
using BlockChainSample.Utilities;
using Newtonsoft.Json;
using static BlockChainSample.Context;

namespace BlockChainSample.Model
{
    public class BlockChain
    {
        public BlockChain()
        {
            //in some cases we don't want to create genesis block for examples when getting block chain from api so we use this constructor
            Chain = new List<Block>();
            CurrentTransactions = new List<Transaction>();
        }
        public BlockChain(Block genesisBlock):this()
        {
            //This constructor creates a block to start BlockChain
            NewBlock(genesisBlock.Proof, genesisBlock.PreviousHash);
        }
        public List<Block> Chain { get; set; }
        public List<Transaction> CurrentTransactions { get; set; }
        public int Length => Chain.Count;
        public Block LastBlock => Chain.LastOrDefault();

        public int NewTransaction(Transaction trx)
        {
            //Adds new transaction to mempool
            CurrentTransactions.Add(trx);

            //This method returns the index of next block that this transaction will be created on
            return LastBlock.Index + 1;
        }

        public Block NewBlock(int proof, string previousHash)
        {
            var block = new Block
            {
                Index = Chain.Count + 1,
                TimeStamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds(),
                Transactions = CurrentTransactions,
                Proof = proof,
                PreviousHash = previousHash
            };

            Chain.Add(block);
            CurrentTransactions = new List<Transaction>();
            return block;
        }

        public int ProofOfWork()
        {
            //to create a block we need to pass a simple algorithm
            var lastProof = LastBlock.Proof;
            var lastHash = Hash(LastBlock);

            var proof = 0;
            while (!ValidateProof(lastProof, proof, lastHash))
            {
                proof++;
            }

            return proof;
        }

        public bool ValidateProof(int lastProof, int proof, string lastBlockHash)
        {
            var guess = $"{lastProof}{proof}{lastBlockHash}";
            var guessHash = Tools.Hash(guess);
            return guessHash.EndsWith("0000");
        }

        public bool ResolveConflicts(IEnumerable<BlockChain> BlockChains)
        {
            List<Block> newChain = null;

            var maxLength = CurrentNode.BlockChain.Length;
            foreach (var BlockChain in BlockChains)
            {
                if (BlockChain == null)
                {
                    continue;
                }

                if (BlockChain.Length > maxLength && ValidateChain(BlockChain))
                {
                    newChain = BlockChain.Chain;
                }
            }

            if (newChain == null)
            {
                return false;
            }

            Chain = newChain;
            return true;
        }

        public bool ValidateChain(BlockChain BlockChain)
        {
            var lastBlock = BlockChain.Chain[0];
            for (var i = 1; i < BlockChain.Length; i++)
            {
                var block = BlockChain.Chain[i];

                var lastBlockHash = Hash(lastBlock);
                if (!(block.PreviousHash == lastBlockHash && ValidateProof(lastBlock.Proof, block.Proof, lastBlockHash)))
                {
                    return false;
                }

                lastBlock = block;
            }

            return true;
        }

        //an indexer to get a block with its index eg => BlockChain[1] will return the block with index 1
        public Block this[int index] => Chain[index - 1];

        public static string Hash(Block block)
        {
            var blockJson = JsonConvert.SerializeObject(block);
            return Tools.Hash(blockJson);
        }
    }
}
