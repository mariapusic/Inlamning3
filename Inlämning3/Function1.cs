using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Inlämning3
{
    public static class Function1
    {
        private static List<Contact> contactList = new();

        //Den här funktionen gör så att du kan hämta alla kontakter i adressboken med hjälp av en GET 
        [FunctionName("GetAllContacts")]
        public static async Task<IActionResult> GetAllContacts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Addressbook")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Fetching all contacts");

            return new OkObjectResult(contactList);
        }

        //Den här funtionen gör så att du kan hämta en kontakt i adressboken med hjälp av id med hjälp av en GET
        [FunctionName("GetContactById")]
        public static async Task<IActionResult> GetContactById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Addressbook/{Id}")]
            HttpRequest req,
            ILogger log, string Id)
        {
            log.LogInformation("Fething one contact");
            Contact? contact = contactList.FirstOrDefault(c => c.Id == Id);

            return contact is null ? new NotFoundResult() : new OkObjectResult(contact);
        }

        //Den här funktionen gör så att du kan skapa en ny kontakt med  en POST
        [FunctionName("CreateContact")]
        public static async Task<IActionResult> CreateContact(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Addressbook")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating a contact");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Contact newContact = JsonConvert.DeserializeObject<Contact>(requestBody);
            newContact.Id = Guid.NewGuid().ToString();
            contactList.Add(newContact);

            return new OkObjectResult(newContact);
        }


        //Den här funktionen gör så att du kan uppdatera en kontakt i adressboken med hjälp av en PUT
        [FunctionName("UpdateContact")]
        public static async Task<IActionResult> UpdateContact(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "Addressbook/{Id}")]
            HttpRequest req,
            ILogger log, string Id)
        {
            log.LogInformation("Updating a contact");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Contact updateContact = JsonConvert.DeserializeObject<Contact>(requestBody);
            Contact? contact = contactList.FirstOrDefault(c => c.Id == Id);
            contact.FirstName = updateContact.FirstName;
            contact.LastName = updateContact.LastName;
            contact.Email = updateContact.Email;
            contact.PhoneNumber = updateContact.PhoneNumber;

            return new OkObjectResult(updateContact);
        }

        //Den här funktionen gör så att du kan ta bort en kontakt i adressboken med hjälp av en DELETE
        [FunctionName("DeleteContact")]
        public static async Task<IActionResult> DeleteContact(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Addressbook/{Id}")]
            HttpRequest req,
            ILogger log, string Id)
        {
            log.LogInformation("Deleting a contact");
            Contact? contact = contactList.FirstOrDefault(c => c.Id == Id);

            if (contact is not null)
            {
                contactList.Remove(contact);
                return new OkResult();
            }

            return new NotFoundResult();
        }
    }
}
