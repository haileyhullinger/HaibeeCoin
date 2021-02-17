using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace BlockChain
{
    class Block
    {
        public int Index { get; set; }
        public string PreviousHash { get; set; }
        public string Timestamp { get; set; }
        //public string Data { get; set; }
        public string Hash { get; set; }
        
        //information used in mining
        public int Nonce { get; set; }

        //replaces Data attribute
        public List<Transaction> Transactions { get; set; }


        //constructor
        public Block(int index, string timestamp, List<Transaction> transactions, string previousHash = "")
        {
            //set instances passed in
            this.Index = index;
            this.Timestamp = timestamp;
            this.Transactions = transactions;
            this.PreviousHash = previousHash;
            //calculate hash by calling method
            this.Hash = CalculateHash();
            this.Nonce = 0;
        }

        public string CalculateHash()
        {
            //concatenate all transactions from the block
            string blockData = this.Index + this.PreviousHash + this.Timestamp + this.Transactions.ToString() + this.Nonce;

            //convert to bytes to prepare to hash
            byte[] blockBytes = Encoding.ASCII.GetBytes(blockData);

            //hashing algorithm
            byte[] hashBytes = SHA256.Create().ComputeHash(blockBytes);

            //return hashed block data in hexidecimal format
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        //mining method, vary nonce until the overall hash of the block meets the desired difficulty level
        //the more 0 we requrie in the hash, the higher the difficulty level
        public void Mine(int difficulty)
        {
            //loop to recalculate the nonce if the desired difficulty level is not met
            while (this.Hash.Substring(0,difficulty) != new String('0', difficulty))
            {
                this.Nonce++;
                this.Hash = this.CalculateHash();
                Console.WriteLine("Mining: " + this.Hash);
            }

            //mined block
            Console.WriteLine("Block has been minded: " + this.Hash);
        }
    }
}
