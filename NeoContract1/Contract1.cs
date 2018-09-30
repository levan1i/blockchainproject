using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Numerics;


namespace NeoContract1
{
    public class Contract1 : SmartContract
    {
        public static readonly byte[] Owner = "AK2nJJpJr6o664CWJKi1QRXjqeic2zRp8y".ToScriptHash();
        public static readonly byte[] Price = "AdprYnf2onVDxdFSt7kiFJiekWFw7L13cW".ToScriptHash();
        public static readonly byte[] Energy = "AZdrfBwgxWm4gLkLDRSjgebpr6f9q1oqsP".ToScriptHash();
               [DisplayName("transfer")]
        public static event Action<byte[], byte[], BigInteger> Transferred;

        public static object Main(string method, params object[] args)
        {
            if (Runtime.Trigger == TriggerType.Application)
            {


                if (method == "name")
                    return "EnToken";
                if (method == "symbol")
                    return "ET";
                if (method == "decimals")
                    return 2;
                if (method == "totalSupply")
                    return 100000_00;
                if (method == "initialize")
                    Initialize();
                if (method == "balanceOf")
                    return BalanceOf((byte[])args[0]);
                if (method == "transfer")
                    return Transfer((byte[])args[0], (byte[])args[1], (BigInteger)args[2]);
                if (method == "addPrice")
                    return AddPrice((string)args[0]);
                if (method == "getPrice")
                    return GetPrice();
                if (method == "addEnergy")
                    return AddEnergy((string)args[0]);
                if (method == "getEnergy")
                    return GetEnergy();
                return false;
            }
            else
            {
                Runtime.Notify("Unsupported trigger type");
                return false;
            }
        }
    
        public static bool Transfer(byte[] from, byte[] to, BigInteger amount)
        {
           
            if (from.Length< 20)
                throw new Exception("Invalid From Address");
            if (to.Length <20)
                throw new Exception("Invalid To Address");
            if (amount < 0)
                throw new Exception("Invalid Amount");

            if (!Runtime.CheckWitness(from))
            {
                Runtime.Notify("Address check failed", from , to, amount);
                return false;
            }

            if (amount == 0)
            {
                Transfer(from, to,  amount);
                return true;
            }

            if (from == to)
            {
                Transfer(from, to, amount);
                return true;
            }

            var fromBalance = Storage.Get(Storage.CurrentContext, from).AsBigInteger();
            var toBalance = Storage.Get(Storage.CurrentContext, to).AsBigInteger();

            if (fromBalance - amount < 0)
                return false;

            Storage.Put(Storage.CurrentContext, from, fromBalance - amount);
            Storage.Put(Storage.CurrentContext, to, toBalance + amount);

            Transferred(from, to, amount);

            return true;
        }

        public static object BalanceOf(byte[] address)
        {
          
            if (address.Length <20)
                throw new Exception("Invalid Address");

            return Storage.Get(Storage.CurrentContext, address);
        }

        public static void Initialize()
        {
            if (Runtime.CheckWitness(Owner))
            {
                var initialized = Storage.Get(Storage.CurrentContext, "initialized");
                if (initialized == null)
                {
                    Storage.Put(Storage.CurrentContext, Owner, 1000_00);
                    Storage.Put(Storage.CurrentContext, "initialized", "true");
                    Transferred(null, Owner, 100000_00);
                }
            }
        }

        public static object AddPrice(string indicate)
        {
            Storage.Put(Storage.CurrentContext, Price,indicate);
            return Storage.Get(Storage.CurrentContext, Price);

        }

        public static object GetPrice()
        {
            return Storage.Get(Storage.CurrentContext, Price);
        }

        public static object AddEnergy(string energypr)
        {
            Storage.Put(Storage.CurrentContext, Energy, energypr);
            return Storage.Get(Storage.CurrentContext, Energy);

        }

        public static object GetEnergy()
        {
            return Storage.Get(Storage.CurrentContext, Energy);
        }
    }
}
