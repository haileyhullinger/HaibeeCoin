using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;



namespace BlockChain
{
    class Program
    {
        static void Main(string[] args)
        {
            //create instances of new wallets with private and public keys
            PrivateKey key1 = new PrivateKey();
            PublicKey wallet1 = key1.publicKey();

            PrivateKey key2 = new PrivateKey();
            PublicKey wallet2 = key2.publicKey();

            //create the chain
            Blockchain haibeecoin = new Blockchain(2, 100);

            Console.WriteLine("Start the Miner");

            //add 100 to wallet1 to begin transactions 
            haibeecoin.MinePendingTransactions(wallet1);
            Console.WriteLine("\n Balance of wallet 1 is $" + haibeecoin.GetBalanceOfWallet(wallet1).ToString());

            //create transaction object
            Transaction tx1 = new Transaction(wallet1, wallet2, 10);
            //sign transaction
            tx1.SignTransaction(key1);
            //add to pending transactions to be mined
            haibeecoin.addPendingTransaction(tx1);

            //start miner again for the transaction
            Console.WriteLine("Start the Miner");
            haibeecoin.MinePendingTransactions(wallet2);
            //check balance of both walets again to see transaction
            Console.WriteLine("\n Balance of wallet 1 is $" + haibeecoin.GetBalanceOfWallet(wallet1).ToString());
            Console.WriteLine("\n Balance of wallet 2 is $" + haibeecoin.GetBalanceOfWallet(wallet2).ToString());

            //new instances of a blocks on the chain
            //haibeecoin.AddBlock(new Block(1, DateTime.Now.ToString("yyyyMMddHHmssffff"), "amount: 50"));
            //haibeecoin.AddBlock(new Block(2, DateTime.Now.ToString("yyyyMMddHHmssffff"), "amount: 200"));

            //serialize entire blockchain
            string blockJSON = JsonConvert.SerializeObject(haibeecoin, Formatting.Indented);

            Console.WriteLine(blockJSON);

            //call isChainValid Method
            if (haibeecoin.IsChainValid())
            {
                Console.WriteLine("Blockchain is Valid!");
            }
            else
            {
                Console.WriteLine("Blockchain is NOT valid!");
            }
        }
    }

}
