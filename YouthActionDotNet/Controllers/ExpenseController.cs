using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouthActionDotNet.DAL;
using YouthActionDotNet.Data;

namespace YouthActionDotNet.Controllers{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase, IUserInterfaceCRUD<Expense>
    {
        private GenericRepository<Expense> ExpenseRepository;
        public ExpenseController(DBContext context)
        {
            ExpenseRepository = new GenericRepository<Expense>(context);
        }
        public Task<ActionResult<string>> All()
        {
            throw new System.NotImplementedException();
        }

        public Task<ActionResult<string>> Create(Expense template)
        {
            throw new System.NotImplementedException();
        }

        public Task<ActionResult<string>> Delete(string id)
        {
            throw new System.NotImplementedException();
        }

        public bool Exists(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ActionResult<string>> Get(string id)
        {
            throw new System.NotImplementedException();
        }

        public string Settings()
        {
            throw new System.NotImplementedException();
        }

        public Task<ActionResult<string>> Update(string id, Expense template)
        {
            throw new System.NotImplementedException();
        }

        public Task<ActionResult<string>> UpdateAndFetchAll(string id, Expense template)
        {
            throw new System.NotImplementedException();
        }
    }
}