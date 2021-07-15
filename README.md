# BlockChain
a Simple c# blockchain

# Api
<pre>
GET   nodes/selfid                Gets current node id
GET   nodes/registered            Gets all registered nodes address
GET   nodes/resolve               Searches in all registered nodes and set current chain to longest chain found
POST  nodes/register              Post an array of string as nodes address  input => {"addresses":[]}
          
GET   blockchain/mine             Solves a simple algorithm and add a block to blockchain
GET   blockchain/chain            Gets full blockchain
GET   blockchain/chain/{index}    Gets a block by its index
POST  blockchain/transaction/new  Adds a transaction to blockchain mempool  input => {"sender":"","recipient":"",amount:double number}
</pre>
