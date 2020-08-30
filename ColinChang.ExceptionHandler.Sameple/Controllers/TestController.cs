using System;
using System.Threading.Tasks;
using ColinChang.ExceptionHandler;
using Microsoft.AspNetCore.Mvc;

namespace WebDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        public Task<IOperationResult> GetAsync()
        {
            throw new OperationException("test exception middleware");
        }

        [HttpGet("{id}")]
        [OperationExceptionFilter]
        public Task<IOperationResult> GetAsync(int id)
            {
            if (id < 0)
                throw new OperationException("expected exception");

            if (id > 0)
                return Task.FromResult<IOperationResult>(new OperationResult<string>("test exception filter attribute"));

            throw new Exception("unexpected exception");
        }
    }
}