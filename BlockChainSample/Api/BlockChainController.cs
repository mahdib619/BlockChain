using System.Web.Http;
using BlockChainSample.Model;
using static BlockChainSample.Context;

namespace BlockChainSample.Api
{
    [RoutePrefix("BlockChain")]
    public class BlockChainController : ApiController
    {
        [HttpGet]
        [Route("mine")]
        public IHttpActionResult Mine()
        {
            var bc = CurrentNode.BlockChain;

            //Create reward transaction
            bc.NewTransaction(new Transaction { Amount = 12.5, Recipient = CurrentNode.Id, Sender = "" });

            var previousHash = BlockChain.Hash(bc.LastBlock);
            var proof = bc.ProofOfWork();
            var newBlock = bc.NewBlock(proof, previousHash);

            var response = new
            {
                message = "New block mined.",
                block = newBlock
            };

            return Created($"BlockChain/chain/{newBlock.Index}", response);
        }

        [HttpGet]
        [Route("chain")]
        public IHttpActionResult GetChain()
        {
            var response = new
            {
                chain = CurrentNode.BlockChain.Chain,
                length = CurrentNode.BlockChain.Length
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("chain/{index}")]
        public IHttpActionResult GetChain(int index)
        {
            var bc = CurrentNode.BlockChain;

            if (index < 1 || index > bc.Length)
            {
                return BadRequest("Index was outside of range!");
            }

            var response = new
            {
                chain = bc[index]
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("transactions/new")]
        public IHttpActionResult NewTransaction(Transaction trx)
        {
            var bc = CurrentNode.BlockChain;

            var blockIndex = bc.NewTransaction(trx);
            var response = new
            {
                message = $"Transaction will be added to Block {blockIndex}"
            };

            return Created($"Block {blockIndex}", response);
        }
    }
}
