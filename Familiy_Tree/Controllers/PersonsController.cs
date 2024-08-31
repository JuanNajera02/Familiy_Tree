using Familiy_Tree.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Family_Tree.DTOs;
using Entities.Entities;
using Familiy_Tree.DTOs;

namespace Familiy_Tree.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly PersonsBusiness _personsBusiness;

        public PersonsController(PersonsBusiness personsBusiness)
        {
            _personsBusiness = personsBusiness;
        }

        //post person
        [HttpPost("AddPersons")]
        public async Task<ActionResult<Persons>> PostPerson(AddPersonDTO addPersonDTO)
        {
            var response = await _personsBusiness.Add(addPersonDTO);
            return Ok(response);

        }

        //delete person
        [HttpDelete("DeletePersons/{id}")]
        public async Task<ActionResult<Persons>> DeletePersons(int id)
        {
            await _personsBusiness.Delete(id);
            return Ok();
        }

        //update person
        [HttpPut("UpdatePersons")]
        public async Task<ActionResult<Persons>> UpdatePersons([FromBody] UpdatePersonDTO updatePersonDTO)
        {
            var response = await _personsBusiness.Update(updatePersonDTO);
            return Ok(response);
        }

        //get persons with partenrs = null
        [HttpGet("GetPersonsWNotPartner")]
        public async Task<ActionResult<List<Persons>>> GetPersons()
        {
            var response = await _personsBusiness.GetAllWNotPartner();
            return Ok(response);
        }

        // GetAllWNotChildren

        [HttpGet("GetAllWNotChildren")]
        public async Task<ActionResult<List<Persons>>> GetAllWNotChildren()
        {
            var response = await _personsBusiness.GetAllWNotChildren();
            return Ok(response);
        }

        //get all persons
        [HttpGet("GetAllPersons")]
        public async Task<ActionResult<List<Persons>>> GetAllPersons()
        {
            var response = await _personsBusiness.GetAll();
            return Ok(response);
        }

        //GetAllWNotChildrenAndChildren param id
        [HttpGet("GetAllWNotChildrenAndChildren/{id}")]
        public async Task<ActionResult<List<Persons>>> GetAllWNotChildrenAndChildren(int id)
        {
            var response = await _personsBusiness.GetAllWNotChildrenAndChildren(id);
            return Ok(response);
        }

        [HttpGet("GetFamilyTree/{personId}")]
        public ActionResult<Persons> GetFamilyTree(int personId)
        {

            //declara un hashSET HashSet<int> seen
            var seen = new HashSet<int>();

            var familyTree = _personsBusiness.GetFamilyTree(personId, seen);
                return Ok(familyTree);
        }




    }
}
