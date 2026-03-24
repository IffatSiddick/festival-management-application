using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalApp
{
    internal class FestivalManager
    {
        private SQLRepository2 repository;

        public FestivalManager(SQLRepository2 repository)
        {
            this.repository = repository;
        }

        // Add a new performer
        public string AddPerformer(string name, string telephone, string email, int fee, List<string> genres)
        {
            // Business validation
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("ERROR: Name cannot be empty");
            }
            else if (string.IsNullOrWhiteSpace(telephone))
            {
                errors.Add("ERROR: Telephone number cannot be empty");
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("ERROR: Email cannot be empty");
            }
            else if (fee <= 0)
            {
                errors.Add("ERROR: Fee cannot be zero");
            }
            else if (fee > 10000)
            {
                errors.Add("ERROR: Fee is too high.");
            }
            
            if (genres == null)
            {
                errors.Add("ERROR: You must enter at least one genre the performer performs in.");
            }
            else {
                foreach (string genre in genres)
                {
                    if (string.IsNullOrWhiteSpace(genre))
                    {
                        errors.Add("ERROR: genre cannot be empty");
                    }
                }
            }

            if (errors.Count > 0)
            {
                return string.Join(", ", errors);
            }
            else
            {
                // Create and add performer
                Performer performer = new Performer(1, name, telephone, email, fee, genres);
                repository.Add(performer);

                return "SUCCESS: Performer '" + name + "' added successfully";
            }
        }

        // Add a new crew
        public string AddCrew(string name, string telephone, string email, int hourly_rate, string employment, int weekly_hours)
        {
            // Business validation
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("ERROR: Name cannot be empty");
            }
            else if (string.IsNullOrWhiteSpace(telephone))
            {
                errors.Add("ERROR: Telephone number cannot be empty");
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("ERROR: Email cannot be empty");
            }
            else if (hourly_rate <= 0)
            {
                errors.Add("ERROR: Hourly rate cannot be zero or a negative amount");
            }
            else if (hourly_rate > 100)
            {
                errors.Add("ERROR: Hourly rate is too high.");
            }
            else if (employment != "FULL TIME" && employment != "PART TIME")
            {
                errors.Add("ERROR: Employment type invalid. ");
            }
            else if (employment == "FULL TIME" && (weekly_hours < 25 || weekly_hours > 40))
            {
                errors.Add("ERROR: Weekly hours are outside the limit for this employment type. A full time employee's hours must be between 25 - 40 hours per week.");
            }
            else if (employment == "PART TIME" && (weekly_hours >= 25 || weekly_hours < 1))
            {
                errors.Add("ERROR: Weekly hours are outside the limit for this employment type. A part time employee's hours must be between 1 - 24 hours per week");
            }

            if (errors.Count > 0)
            {
                return string.Join(", ", errors);
            }
            else
            {
                // Create and add crew
                Crew crew = new Crew(1, name, telephone, email, hourly_rate, employment, weekly_hours);
                repository.Add(crew);

                return "SUCCESS: Crew '" + name + "' added successfully";
            }
        }

        // Add a new vendor
        public string AddVendor(string name, string telephone, string email, List<string> product_categories)
        {
            // Business validation
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("ERROR: Name cannot be empty");
            }
            else if (string.IsNullOrWhiteSpace(telephone))
            {
                errors.Add("ERROR: Telephone number cannot be empty");
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("ERROR: Email cannot be empty");
            }
            
            if (product_categories == null)
            {
                errors.Add("ERROR: You must enter at least one product categories the vendor sells in.");
            }
            else {
                foreach (string product in product_categories)
                {
                    if (string.IsNullOrWhiteSpace(product))
                    {
                        errors.Add("ERROR: product name cannot be empty");
                    }
                }
            }

            if (errors.Count > 0)
            {
                return string.Join(", ", errors);
            }
            else
            {
                // Create and add vendor
                Vendor vendor = new Vendor(1, name, telephone, email, product_categories);
                repository.Add(vendor);

                return "SUCCESS: Vendor '" + name + "' added successfully";
            }
        }

        // Get all people
        public List<Person> GetAllPeople()
        {
            return repository.GetAll();
        }
       
        // Get person by ID
        public Person GetPersonByID(int personId)
        {
            return repository.FindByID(personId);
        }

        // Delete person from database
        public string DeletePerson(int personID)
        {
            Person person = repository.FindByID(personID);

            if (person == null)
            {
                return "ERROR: Person was not found!";
            }

            repository.Delete(personID);
            return "SUCCESS: Person '" + person.Name + "' was deleted successfully.";
        }

        // Edit person from database
        public string EditPerson(int personID, string column_name, 
            string updated_value, string genre_or_category = "")
        {
            Person person = repository.FindByID(personID);

            if (person == null)
            {
                return "ERROR: Person was not found!";
            }

            string[] allowed = { "name", "telephone", "email", "fee", "genre",
                "hourly_rate", "employment", "weekly_hours", "category" };

            int rowsAffected = 0;

            if (!allowed.Contains(column_name))
            {
                return "Invalid column name.";
            }
            else if (column_name == "genre" || column_name == "category")
            {
                rowsAffected = repository.EditByID(personID, column_name, updated_value, genre_or_category);
            }
            else
            {
                rowsAffected = repository.EditByID(personID, column_name, updated_value);
            }

            if (rowsAffected == 0)
            {
                return "ERROR: The edit could not be performed.";
            }
            return "SUCCESS: Person '" + person.Name + "' was edited successfully.";
        }

        public string GetByRole(string role, out List<Person> people)
        {
            people = new List<Person>();

            if (role != "PERFORMER" && role != "CREW" && role != "VENDOR")
            {
                return "ERROR: Role entered invalid.";
            }
            else
            {
                people = repository.GetByRole(role);
                return "SUCCESS";
            }
        }

        public string FindPersonByName(string name, out List<Person> people)
        {
            people = repository.SearchByName(name);

            if (people.Count == 0)
            {
                return "ERROR: No person with that name found.";
            }
            else
            {
                return "SUCCESS";
            }
        }
    }
}
