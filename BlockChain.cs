using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace BlockChain
{
    class Blockchain
    {
        public List<Block> Chain { get; set; }

        public int Difficutly { get; set; }
        public List<Transaction> pendingTransactions { get; set; }
        public decimal MiningReward { get; set; }


        //constructor
        public Blockchain(int difficulty, decimal miningReward)
        {
            this.Chain = new List<Block>();
            this.Chain.Add(CreateGenesisBlock());
            this.Difficutly = difficulty;
            this.MiningReward = miningReward;
            this.pendingTransactions = new List<Transaction>();

        }

        //method to create genesis block (block 0 in the chain)
        public Block CreateGenesisBlock()
        {
            return new Block(0, DateTime.Now.ToString("yyyyMMddHHmssffff"), new List<Transaction>());

        }

        //method to retrieve the most recent block
        public Block GetLatestBlock()
        {
            return this.Chain.Last();
        }

        //method to add a block to the chain
        public void AddBlock(Block newBlock)
        {
            //pulls the hash from the previous block
            newBlock.PreviousHash = this.GetLatestBlock().Hash;
            //rehashes entire block
            newBlock.Hash = newBlock.CalculateHash();
            //add block to chain
            this.Chain.Add(newBlock);
        }

        public void addPendingTransaction(Transaction transaction)
        {
            if (transaction.FromAddress is null || transaction.ToAddress is null)
            {
                throw new Exception("Transaction must include a to and a from address");
            }

            if (transaction.Amount > this.GetBalanceOfWallet(transaction.FromAddress))
            {
                throw new Exception("There is not sufficient funds in the sender's wallet");
            }

            if (transaction.IsValid() == false)
            {
                throw new Exception("Cannot add an invalid transaction to a block");
            }

            //if all checks pass, then add pending transaction to the block

            this.pendingTransactions.Add(transaction);
        }

        //method to check balance of transaction to see if it is valid
        public decimal GetBalanceOfWallet(PublicKey address)
        {
            decimal balance = 0;

            string addressDER = BitConverter.ToString(address.toDer()).Replace("-", "");

            foreach (Block block in this.Chain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    if (!(transaction.FromAddress is null))
                    {
                        string fromDER = BitConverter.ToString(transaction.FromAddress.toDer()).Replace("-", "");

                        if (fromDER == addressDER)
                        {
                            balance -= transaction.Amount;
                        }
                    }

                    string toDER = BitConverter.ToString(transaction.ToAddress.toDer()).Replace("-", "");
                    if (toDER == addressDER)
                    {
                        balance += transaction.Amount;
                    }
                }
            }

            return balance;
        }

        //method to initate mining
        public void MinePendingTransactions(PublicKey miningRewardWallet)
        {
            Transaction rewardtx = new Transaction(null, miningRewardWallet, MiningReward);
            this.pendingTransactions.Add(rewardtx);

            Block newBlock = new Block(GetLatestBlock().Index - 1, DateTime.Now.ToString("yyyyMMddHHmssffff"), this.pendingTransactions, GetLatestBlock().Hash);
            newBlock.Mine(this.Difficutly);

            Console.WriteLine("Block Successfully mined!");

            this.Chain.Add(newBlock);

            this.pendingTransactions = new List<Transaction>();
        }

        //method to check if chain is valid 
        public bool IsChainValid()
        {
            for (int i = 1; i < this.Chain.Count; i++)
            {
                Block curretnBlock = this.Chain[i];
                Block previousBlock = this.Chain[i - 1];

                //check if the current block hash is same as calcualted hash
                if (curretnBlock.Hash != curretnBlock.CalculateHash())
                {
                    return false;
                }

                //check that the chain has not been tampered with
                if (curretnBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            //if passes all check, blockchain is valid
            return true;
        }

    }

}
