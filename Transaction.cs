using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace BlockChain
{
    class Transaction
    {
        public PublicKey FromAddress { get; set; }
        public PublicKey ToAddress { get; set; }
        public decimal Amount { get; set; }
        public Signature Signature { get; set; }


        //constructor
        public Transaction(PublicKey fromAddress, PublicKey toAddress, decimal amount)
        {
            this.FromAddress = fromAddress;
            this.ToAddress = toAddress;
            this.Amount = amount;
        }

        //method to sign transaction
        public void SignTransaction(PrivateKey signingKey)
        {
            string fromAddressDER = BitConverter.ToString(FromAddress.toDer()).Replace("-", "");
            string signingDER = BitConverter.ToString(signingKey.publicKey().toDer()).Replace("-", "");

            if (fromAddressDER != signingDER)
            {
                throw new Exception("You cannot sign transactions for other wallets!");
            }

            string txHash = this.CalculateHash();
            this.Signature = Ecdsa.sign(txHash, signingKey);
        }

        //method to get the hash of the transaction
        public string CalculateHash()
        {
            string fromAddressDER = BitConverter.ToString(FromAddress.toDer()).Replace("-", "");
            string toAddressDER = BitConverter.ToString(ToAddress.toDer()).Replace("-", "");
            string transactionData = fromAddressDER + toAddressDER + Amount;

            byte[] tdBytes = Encoding.ASCII.GetBytes(transactionData);
            return BitConverter.ToString(SHA256.Create().ComputeHash(tdBytes)).Replace("-", "");

        }

        //method to evaluate if it is a valid transaction
        public bool IsValid()
        {
            //mining rewards, no from address listed
            if (this.FromAddress is null) return true;

            //check for signature
            if (this.Signature is null)
            {
                throw new Exception("No Signature in this transaction");
            }
            return Ecdsa.verify(this.CalculateHash(), this.Signature, this.FromAddress);
        }
    }
}
