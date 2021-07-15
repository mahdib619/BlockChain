using System;
using System.Net;
using System.Collections.Generic;
using System.Web.Http;
using Flurl;
using BlockChainSample.Dto;
using BlockChainSample.Model;
using Flurl.Http;
using static BlockChainSample.Context;

namespace BlockChainSample.Api
{
    [RoutePrefix("nodes")]
    public class NodesController : ApiController
    {
        [HttpGet]
        [Route("selfid")]
        public IHttpActionResult GetNodeId()
        {
            var id = new IdDto
            {
                Id = CurrentNode.Id
            };
            return Ok(id);
        }

        [HttpGet]
        [Route("registered")]
        public IHttpActionResult GetRegisteredNodes()
        {
            return Ok(CurrentNode.RegisteredNodes);
        }

        [HttpGet]
        [Route("resolve")]
        public IHttpActionResult Consensus()
        {
            var bc = CurrentNode.BlockChain;

            var errorMessage = "";//if any errors occures this variable will add error message to response message

            var BlockChains = new List<BlockChain>();
            foreach (var node in CurrentNode.RegisteredNodes)
            {
                try
                {
                    //getting node BlockChain by calling its api
                    BlockChains.Add($"{node}/BlockChain/chain".GetJsonAsync<BlockChain>().Result);
                }
                catch (Exception)
                {
                    if (string.IsNullOrWhiteSpace(errorMessage))
                    {
                        errorMessage = $"{Environment.NewLine}One or more nodes didn't responde properly!";
                    }
                }
            }

            var chainReaplaced = bc.ResolveConflicts(BlockChains);

            var response = new
            {
                message = (chainReaplaced ? "Our chain was repalced." : "We already have the most valid chain.") + errorMessage,
                chain = bc.Chain
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult RegisterNodes(NodesDto input)
        {
            var validAddresses = new List<string>();
            var hasInvalid = false;
            try
            {
                foreach (var address in input.Nodes)
                {
                    try
                    {
                        var uri = new Url(address);

                        if (uri.IsRelative)
                            throw new Exception();

                        var url = uri.ToString();
                        if (url.EndsWith("/"))
                        {
                            url = url.Remove(url.Length - 1);
                        }

                        validAddresses.Add(url);
                    }
                    catch when (!hasInvalid)
                    {
                        hasInvalid = true;//set this flag to true if it's not
                    }
                    catch
                    {
                        //No need to do anything flag has been set already
                    }
                }

                validAddresses.ForEach(CurrentNode.RegisteredNodes.Add);

                var output = new
                {
                    message = hasInvalid ? $"{input.Nodes.Count - validAddresses.Count} node(s) had invalid address!" : "All nodes registered.",
                    validAddresses
                };

                return Content(HttpStatusCode.OK, output);
            }
            catch
            {
                return Content(HttpStatusCode.InternalServerError, "Faild to register nodes!");
            }
        }
    }
}
