using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;
using System; // Required for BitConverter

namespace Backend.Services {
    public class BlockchainService {
        private readonly Web3 _web3;
        // NOTE: ABI and Contract Address should be set correctly here or read from config
        private readonly string _contractAddress = "YOUR ABI";
        private readonly string _abi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"string\",\"name\":\"productId\",\"type\":\"string\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"admin\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"timestamp\",\"type\":\"uint256\"}],\"name\":\"ProductMarkedFake\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"string\",\"name\":\"productId\",\"type\":\"string\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"manufacturer\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bytes32\",\"name\":\"productHash\",\"type\":\"bytes32\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"timestamp\",\"type\":\"uint256\"}],\"name\":\"ProductRegistered\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"admin\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"productId\",\"type\":\"string\"}],\"name\":\"getProduct\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"exists\",\"type\":\"bool\"},{\"internalType\":\"bytes32\",\"name\":\"productHash\",\"type\":\"bytes32\"},{\"internalType\":\"address\",\"name\":\"manufacturer\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"timestamp\",\"type\":\"uint256\"},{\"internalType\":\"bool\",\"name\":\"isFake\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"productId\",\"type\":\"string\"}],\"name\":\"markProductFake\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"string\",\"name\":\"productId\",\"type\":\"string\"},{\"internalType\":\"bytes32\",\"name\":\"productHash\",\"type\":\"bytes32\"}],\"name\":\"registerProduct\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

        public BlockchainService(IConfiguration config) {
            var rpc = config.GetValue<string>("Blockchain:RpcUrl");
            var pk = config.GetValue<string>("Blockchain:PrivateKey");
            
            if (string.IsNullOrEmpty(pk)) throw new ArgumentNullException("Blockchain:PrivateKey is missing or null in configuration.");

            var account = new Account(pk);
            _web3 = new Web3(account, rpc);
        }

        // FIX #1: Change parameter type from string hashHex to byte[] hashBytes
        public async Task<string> RegisterProductAsync(string productId, byte[] hashBytes) {
            var contract = _web3.Eth.GetContract(_abi, _contractAddress);
            var func = contract.GetFunction("registerProduct");
            
            // FIX #2: Use hashBytes in the SendTransactionAsync call
            var tx = await func.SendTransactionAsync(_web3.TransactionManager.Account.Address, 
                                                     new Nethereum.Hex.HexTypes.HexBigInteger(300000), 
                                                     null, 
                                                     productId, 
                                                     hashBytes);
            return tx;
        }

        public async Task<(bool exists, string productHash, string manufacturer, BigInteger timestamp, bool isFake)> GetProductAsync(string productId) {
            var contract = _web3.Eth.GetContract(_abi, _contractAddress);
            var func = contract.GetFunction("getProduct");
            var ret = await func.CallDeserializingToObjectAsync<GetProductOutputDTO>(productId);
            return (ret.Exists, ret.ProductHash, ret.Manufacturer, ret.Timestamp, ret.IsFake);
        }

        [Nethereum.ABI.FunctionEncoding.Attributes.FunctionOutput]
        public class GetProductOutputDTO {
            [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("bool","exists",1)]
            public bool Exists { get; set; }
            [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("bytes32","productHash",2)]
            public byte[] ProductHashBytes { get; set; }
            public string ProductHash => ProductHashBytes != null ? "0x" + BitConverter.ToString(ProductHashBytes).Replace("-", "").ToLower() : null;
            [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("address","manufacturer",3)]
            public string Manufacturer { get; set; }
            [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("uint256","timestamp",4)]
            public BigInteger Timestamp { get; set; }
            [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("bool","isFake",5)]
            public bool IsFake { get; set; }
        }
    }
}