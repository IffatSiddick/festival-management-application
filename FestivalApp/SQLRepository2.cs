namespace FestivalApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using MySql.Data.MySqlClient;

    internal class SQLRepository2
    {
        // this sets the database this application is connecting to while running
        private readonly string _cs = "server=localhost;database=festival;uid=root;pwd='';";
        // this sets up the connection to the database
        private MySqlConnection GetConnection() => new MySqlConnection(_cs); 

        public SQLRepository2()
        {

        }

        // The SELECT + LEFT JOIN SQL covers all fields of the
        // database tables person, performer, crew and vendor
        // it does not cover the genres performers have
        // and product categories vendors have
        private const string BASE_SELECT_SQL =
            @"SELECT
                    person.person_id,
                    person.name,
                    person.telephone,
                    person.email,
                    person.role,
                    performer.fee,
                    crew.hourly_rate,
                    crew.employment,
                    crew.weekly_hours
                FROM person
                LEFT JOIN performer ON performer.person_id = person.person_id
                LEFT JOIN crew ON crew.person_id = person.person_id
                LEFT JOIN vendor ON vendor.person_id = person.person_id";

        // this method returns a list of all people in the database and their information
        public List<Person>? GetAll()
        {   
            //these are the fields all people have in the database
            int personID;
            string name;
            string telephone;
            string email;
            string role;

            List<Person> people = new List<Person>();

            using (var conn = GetConnection())
            {
                conn.Open();

                string person_sql = "SELECT person_id, name, telephone, email, role FROM person;";
                using (var person_cmd = new MySqlCommand(person_sql, conn))
                {
                    using (MySqlDataReader person_rdr = person_cmd.ExecuteReader())
                    {
                        while (person_rdr.Read())
                        {
                            // the person variables are set to each record in the table person field values
                            personID = person_rdr.GetInt32("person_id");
                            name = person_rdr.GetString("name");
                            telephone = person_rdr.GetString("telephone");
                            email = person_rdr.GetString("email");
                            role = person_rdr.GetString("role");

                            // this gets the performer specific information, their fee and genres
                            if (role == "PERFORMER")
                            {
                                // another connection is needed as the connection for person is still open
                                using (var conn2 = GetConnection())
                                {
                                    conn2.Open();
                                    int fee = 0;

                                    string performer_sql = "SELECT fee FROM performer WHERE person_id = @id;";
                                    using (var performer_cmd = new MySqlCommand(performer_sql, conn2))
                                    {
                                        performer_cmd.Parameters.AddWithValue("@id", personID);
                                        using (MySqlDataReader performer_rdr = performer_cmd.ExecuteReader())
                                        {
                                            while (performer_rdr.Read())
                                            {
                                                // fee is set for each record in performer
                                                fee = performer_rdr.GetInt32("fee");
                                            }
                                        }
                                    }
                                    List<string> genres = new List<string>();

                                    string genres_sql = "SELECT genre FROM genre_performer WHERE performer = @id;";
                                    using (var genres_cmd = new MySqlCommand(genres_sql, conn2))
                                    {
                                        genres_cmd.Parameters.AddWithValue("@id", personID);

                                        using (MySqlDataReader genres_rdr = genres_cmd.ExecuteReader())
                                        {
                                            while (genres_rdr.Read())
                                            {
                                                // every performer in the table has a list of genres that is added to
                                                genres.Add(genres_rdr.GetString("genre"));
                                            }
                                        }
                                    }
                                    // the performer is added to the list of people to be returned 
                                    people.Add(new Performer(personID, name, telephone, email, fee, genres));
                                }   
                            }
                            // this gets the crew specific information
                            else if (role == "CREW")
                            {
                                // another connection is needed as the connection for person is still open
                                using (var conn2 = GetConnection())
                                {
                                    conn2.Open();
                                    int rate = 0;
                                    string employment = "";
                                    int hours = 0;

                                    string crew_sql = "SELECT hourly_rate, employment, weekly_hours FROM crew WHERE person_id = @id;";
                                    using (var crew_cmd = new MySqlCommand(crew_sql, conn2))
                                    {
                                        crew_cmd.Parameters.AddWithValue("@id", personID);
                                        using (MySqlDataReader crew_rdr = crew_cmd.ExecuteReader())
                                        {
                                            while (crew_rdr.Read())
                                            {
                                                // the crew variables are set for each record in the table crew
                                                rate = crew_rdr.GetInt32("hourly_rate");
                                                employment = crew_rdr.GetString("employment");
                                                hours = crew_rdr.GetInt32("weekly_hours");
                                            }
                                        }
                                    }
                                    // the crew member is added to the list of people to be returned 
                                    people.Add(new Crew(personID, name, telephone, email, rate, employment, hours));
                                }   
                            }
                            // this gets the vendor specific information
                            else if (role == "VENDOR")
                            {
                                // another connection is needed as the connection for person is still open
                                using (var conn2 = GetConnection())
                                {
                                    conn2.Open();
                                    List<string> categories = new List<string>();

                                    string vendor_sql = "SELECT category FROM vendor_category WHERE vendor = @id;";
                                    using (var vendor_cmd = new MySqlCommand(vendor_sql, conn2))
                                    {
                                        vendor_cmd.Parameters.AddWithValue("@id", personID);
                                        using (MySqlDataReader vendor_rdr = vendor_cmd.ExecuteReader())
                                        {
                                            while (vendor_rdr.Read())
                                            {
                                                // every vendor in the table has a list of product categories that is added to
                                                categories.Add(vendor_rdr.GetString("category"));
                                            }
                                        }
                                    }
                                    // the crew member is added to the list of people to be returned
                                    people.Add(new Vendor(personID, name, telephone, email, categories));
                                }  
                            }
                        }
                        // once all the records in person and their linked records in performer, crew, and vendor are processed
                        // the list of all people in the database is returned
                        return people;
                    }
                }
            }
        }

        // this method returns a list of people from the role PERFORMER/CREW/VENDOR passed in as the parameter
        public List<Person>? GetByRole(string role)
        {
            // these variables represent information that all people in the dstabase have
            int personID;
            string name;
            string telephone;
            string email;

            // connection to the database is opened
            using (var conn = GetConnection())
            {
                conn.Open();
                // this covers the case where the user enters PERFORMER
                if (role == "PERFORMER")
                {
                    List<Person> performers = new List<Person>();

                    // fee is initialised to be set later
                    int fee;

                    // this sql query selects all information about the performer except their genre/s
                    string performer_sql = "SELECT person.person_id, person.name, person.telephone, person.email, performer.fee " +
                        "FROM person, performer " +
                        "WHERE person.person_id = performer.person_id;";
                    using (var performer_cmd = new MySqlCommand(performer_sql, conn))
                    using (MySqlDataReader performer_rdr = performer_cmd.ExecuteReader())
                    {
                        while (performer_rdr.Read())
                        {
                            // the variables are set to the data each record holds
                            personID = performer_rdr.GetInt32("person_id");
                            name = performer_rdr.GetString("name");
                            telephone = performer_rdr.GetString("telephone");
                            email = performer_rdr.GetString("email");
                            fee = performer_rdr.GetInt32("fee");

                            List<string> genres = new List<string>();

                            // another connection is opened as the previous connection is still running
                            using (var conn2 = GetConnection())
                            {
                                conn2.Open();
                                // the genres each performer performs in are selected and added to the list genres
                                string genres_sql = "SELECT genre " +
                                    "FROM genre_performer " +
                                    "WHERE genre_performer.performer = @id;";
                                using (var genres_cmd = new MySqlCommand(genres_sql, conn2))
                                {
                                    genres_cmd.Parameters.AddWithValue("@id", personID);
                                    using (MySqlDataReader genres_rdr = genres_cmd.ExecuteReader())
                                    {
                                        while (genres_rdr.Read())
                                        {
                                            genres.Add(genres_rdr.GetString("genre"));
                                        }
                                    }
                                }
                            }
                            // now all information about the performer has been gathered a
                            // new Performer object is made and added to the list of performers
                            performers.Add(new Performer(personID, name, telephone, email, fee, genres));
                        }
                    }
                    // the list of performers is returned
                    return performers;
                }
                else if (role == "CREW")
                {
                    // these are the variables unique to the crew database table fields
                    int rate;
                    string employment;
                    int hours;

                    List<Person> crew = new List<Person>();

                    // this sql query selects the information each crew member has
                    string crew_sql = "SELECT person.person_id, person.name, person.telephone, person.email, " +
                        "hourly_rate, employment, weekly_hours FROM person, crew " +
                        "WHERE person.person_id = crew.person_id;";
                    using (var crew_cmd = new MySqlCommand(crew_sql, conn))
                    using (MySqlDataReader crew_rdr = crew_cmd.ExecuteReader())
                    {
                        while (crew_rdr.Read())
                        {
                            // the variables are set to the data each record holds
                            personID = crew_rdr.GetInt32("person_id");
                            name = crew_rdr.GetString("name");
                            telephone = crew_rdr.GetString("telephone");
                            email = crew_rdr.GetString("email");
                            rate = crew_rdr.GetInt32("hourly_rate");
                            employment = crew_rdr.GetString("employment");
                            hours = crew_rdr.GetInt32("weekly_hours");

                            // now all information about the crew member has been gathered a
                            // new crew object is made and added to the list of crew members
                            crew.Add(new Crew(personID, name, telephone, email, rate, employment, hours));
                        }
                    }
                    // the list of crew members is returned
                    return crew;
                }
                else if (role == "VENDOR")
                {
                    List<Person> vendors = new List<Person>();

                    // this selects all the information a vendor has except their product categories
                    string vendor_sql = "SELECT person.person_id, person.name, person.telephone, person.email " +
                        "FROM person, vendor " +
                        "WHERE person.person_id = vendor.person_id;";
                    using (var vendor_cmd = new MySqlCommand(vendor_sql, conn))
                    using (MySqlDataReader vendor_rdr = vendor_cmd.ExecuteReader())
                    {
                        while (vendor_rdr.Read())
                        {
                            // the variables are set to the data each record holds
                            personID = vendor_rdr.GetInt32("person_id");
                            name = vendor_rdr.GetString("name");
                            telephone = vendor_rdr.GetString("telephone");
                            email = vendor_rdr.GetString("email");

                            List<string> categories = new List<string>();

                            // another connection is opened as the previous connection is still running
                            using (var conn2 = GetConnection())
                            {
                                conn2.Open();
                                // the product category each vendor sells are selected and added to the list categories
                                string categories_sql = "SELECT category " +
                                    "FROM vendor_category " +
                                    "WHERE vendor_category.vendor = @id;";
                                using (var categories_cmd = new MySqlCommand(categories_sql, conn2))
                                {
                                    categories_cmd.Parameters.AddWithValue("@id", personID);
                                    using (MySqlDataReader categories_rdr = categories_cmd.ExecuteReader())
                                    {
                                        while (categories_rdr.Read())
                                        {
                                            categories.Add(categories_rdr.GetString("category"));
                                        }
                                    }
                                }
                            }
                            // now all information about the vendor has been gathered a
                            // new vendor object is made and added to the list of vendors
                            vendors.Add(new Vendor(personID, name, telephone, email, categories));
                        }
                    }
                    // the list of vendors is returned
                    return vendors;
                }
            }
            // if the role entered does not match PERFORMER, CREW OR VENDOR null will be returned
            // this indicates that the user gave incorrect input
            return null;
        }

        // this method is part of the function of adding a new PERFORMER/CREW/VENDOR by first adding a person record
        // to the database and then calling the appropriate function for the role
        public bool Add(Person person)
        {
            // this immediately checks the person passed as a parameter can be acted on
            // if it null than a record cannot be added to the database
            if (person == null) return false;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (MySqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        // the ID is initialised here as it will be needed for 
                        // adding the performer/crew/vendor record
                        int personID = 0;

                        // Base insert
                        using (MySqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            // the sql command adds the person record which is needed before
                            // the performer/crew/vendor record can be added
                            cmd.CommandText =
                                @"INSERT INTO person
                                      (name, telephone, email, role)
                                      VALUES
                                      (@name, @telephone, @email, @role);";

                            cmd.Parameters.AddWithValue("@name", person.Name);
                            cmd.Parameters.AddWithValue("@telephone", person.Telephone);
                            cmd.Parameters.AddWithValue("@email", person.Email);

                            // the if else if else statement ensures only the valid roles will be added to the record
                            if (person is Performer) cmd.Parameters.AddWithValue("@role", "PERFORMER");

                            else if (person is Crew) cmd.Parameters.AddWithValue("@role", "CREW");

                            else if (person is Vendor) cmd.Parameters.AddWithValue("@role", "VENDOR");

                            else
                            {
                                return false;
                            }

                            cmd.ExecuteNonQuery();

                            // personID is set to the last generated ID 
                            // this is the ID that was used to insert into person
                            personID = (int)cmd.LastInsertedId;
                        }

                        // Derived insert for performer, crew and vendor
                        if (person is Performer peformer)
                        {
                            AddPerformer(conn, tx, peformer, personID);
                        }
                        else if (person is Crew crew)
                        {
                            AddCrew(conn, tx, crew, personID);
                        }
                        else if (person is Vendor vendor)
                        {
                            AddVendor(conn, tx, vendor, personID);
                        }
                        // confirms the sql command is sent to the database
                        tx.Commit();
                        return true;
                    }
                    // if there is any exception in the try block the catch block will stop the command from
                    // executing and return false to indicate the command could not be completed
                    catch
                    {
                        tx.Rollback();
                        return false; ;
                    }
                }
            }
        }

        // this method add a record to the performer table and returns nothing
        public void AddPerformer(MySqlConnection conn, MySqlTransaction tx, Performer performer, int personID)
        {
            List<string> genres = performer.GetGenres();
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                // this inserts the id and fee as a record into the performer table
                // and connects the performer record to its person record
                cmd.CommandText =
                    @"INSERT INTO performer
                            (person_id, fee)
                            VALUES
                            (@person_id, @fee);";

                cmd.Parameters.AddWithValue("@person_id", personID);
                cmd.Parameters.AddWithValue("@fee", performer.Fee);

                cmd.ExecuteNonQuery();

                foreach (string genre in genres)
                {
                    cmd.Parameters.Clear();
                    cmd.Transaction = tx;

                    // insert into genre_performers the new genre if it doesn't already exist
                    cmd.CommandText =
                        @"INSERT INTO genre (genre_name)
                        SELECT @genre
                        WHERE NOT EXISTS (
                           SELECT 1 FROM genre WHERE genre_name = @genre
                        );";
                    cmd.Parameters.AddWithValue("@genre", genre);
                    cmd.ExecuteNonQuery();

                    // adds record into genre_performer for each genre the performer performs in
                    cmd.CommandText =
                        @"INSERT INTO genre_performer
                            (genre, performer)
                            VALUES
                            (@genre, @person_id);";

                    // the same ID is used to link all three records to each other
                    cmd.Parameters.AddWithValue("@person_id", personID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // this method add a record to the performer table and returns nothing
        public void AddCrew(MySqlConnection conn, MySqlTransaction tx, Crew crew, int personID)
        {
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                // inserts the crew unique information as a crew record 
                // uses the id to connect the person and crew records to each other
                cmd.CommandText =
                    @"INSERT INTO crew
                            (person_id, hourly_rate, employment, weekly_hours)
                            VALUES
                            (@person_id, @hourly_rate, @employment, @weekly_hours);";

                cmd.Parameters.AddWithValue("@person_id", personID);
                cmd.Parameters.AddWithValue("@hourly_rate", crew.Rate);
                cmd.Parameters.AddWithValue("@employment", crew.Employment);
                cmd.Parameters.AddWithValue("@weekly_hours", crew.Hours);

                cmd.ExecuteNonQuery();
            }
        }

        // this method add a record to the performer table and returns nothing
        public void AddVendor(MySqlConnection conn, MySqlTransaction tx, Vendor vendor, int personID)
        {
            List<string> categories = vendor.ProductCategories;

            using (MySqlCommand cmd = conn.CreateCommand())
            {
                // this inserts the id as a record into the vendor table
                // and connects the vendor record to its person record
                cmd.Transaction = tx;
                cmd.CommandText =
                    @"INSERT INTO vendor
                            (person_id)
                            VALUES
                            (@person_id);";

                cmd.Parameters.AddWithValue("@person_id", personID);

                cmd.ExecuteNonQuery();

                foreach (string category in categories)
                {
                    cmd.Parameters.Clear();
                    cmd.Transaction = tx;

                    // insert into product_categories the product if it doesn't already exist
                    cmd.CommandText =
                        @"INSERT INTO product_categories (product_category)
                        SELECT @category
                        WHERE NOT EXISTS (
                           SELECT 1 FROM product_categories WHERE product_category = @category
                        );";
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.ExecuteNonQuery();

                    // adds record into vendor_category for each product the vendor sells
                    cmd.CommandText =
                        @"INSERT INTO vendor_category
                                (vendor, category)
                                VALUES
                                (@person_id, @category);";

                    // the same ID is used to link all three records to each other
                    cmd.Parameters.AddWithValue("@person_id", personID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ON DELETE CASCADE in the DB removes the performer/crew/vendor as well
        public bool Delete(int id)
        {
            // checks for the invalid ID 0 which no record in the database can have
            if (id == 0)
            {
                return false;
            }

            using (var conn = GetConnection())
            {
                conn.Open();
                using (MySqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        using (MySqlCommand cmd = conn.CreateCommand())
                        {
                            // by the foreign key relations once the person record is deleted
                            // all records with the same id will be deleted as well
                            cmd.Transaction = tx;
                            cmd.CommandText =
                                @"DELETE FROM `person` 
                                    WHERE person_id = @person_id;";

                            cmd.Parameters.AddWithValue("@person_id", id);
                            cmd.ExecuteNonQuery();
                        }
                        tx.Commit();
                        return true;
                    }
                    // catches any exceptions ad returns false to indicate the record could not be deleted
                    catch
                    {
                        tx.Rollback();
                        return false;
                    }
                }
            }
        }

        // this emthod edits a record in the database
        public int EditByID(int id, string column, string user_value, string genre_or_category = "")
        {

            // checks for the invalid ID 0 which no record in the database can have
            if (id == 0) return 0;

            // check if ID exixts in database
            Person person = FindByID(id);

            // if ID does not exist return null
            if (person == null)
            {
                return 0;
            }

            int rowsAffected = 0;

            using (var conn = GetConnection())
            {
                conn.Open();
                using (MySqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        using (MySqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;

                            // checks if column name user entered is a field name in person table
                            if (column == "name" || column == "telephone" || column == "email")
                            {
                                // updates record
                                cmd.CommandText =
                                    $"UPDATE `person` SET `{column}` = @user_value WHERE person_id = @person_id;";

                                cmd.Parameters.AddWithValue("@person_id", person.PersonID);
                                cmd.Parameters.AddWithValue("@user_value", user_value);
                                rowsAffected = cmd.ExecuteNonQuery();
                            }
                            // admin may want to change performers fee or the category/categories they perform in
                            else if (person.Role == "PERFORMER")
                            {
                                string table;

                                if (column == "genre")
                                {
                                    table = "genre_performer";
                                    column = "genre";

                                    // can change genre the performer performs in
                                    cmd.CommandText =
                                    $"UPDATE `{table}` SET `{column}` = @user_value WHERE performer = @person_id AND " +
                                    $"genre = @old_value;";
                                }
                                else
                                {
                                    // can change fee of performer
                                    table = "performer";
                                    column = "fee";

                                    cmd.CommandText =
                                    $"UPDATE `{table}` SET `{column}` = @user_value WHERE person_id = @person_id;";
                                }

                                cmd.Parameters.AddWithValue("@person_id", person.PersonID);
                                cmd.Parameters.AddWithValue("@user_value", user_value);
                                cmd.Parameters.AddWithValue("@old_value", genre_or_category);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }
                            else if (person.Role == "CREW")
                            {
                                cmd.CommandText =
                                    $"UPDATE crew SET `{column}` = @user_value WHERE person_id = @person_id;";

                                cmd.Parameters.AddWithValue("@person_id", person.PersonID);
                                cmd.Parameters.AddWithValue("@user_value", user_value);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }
                            // admin wants to change product category as this is the only role specific field vendor has 
                            else if (person.Role == "VENDOR")
                            {
                                cmd.CommandText =
                                    @"UPDATE `vendor_category` 
                                        SET `category` = @user_value
                                        WHERE `vendor` = @person_id AND " +
                                        $"category = @old_value;";

                                cmd.Parameters.AddWithValue("@person_id", id);
                                cmd.Parameters.AddWithValue("@user_value", user_value);
                                cmd.Parameters.AddWithValue("@old_value", genre_or_category);

                                rowsAffected = cmd.ExecuteNonQuery();
                            }
                            tx.Commit();
                            return rowsAffected;
                        }
                    }
                    catch
                    {
                        tx.Rollback();
                        return rowsAffected;
                    }
                }
            }
        }

        public Person? FindByID(int id)
        {
            int personID = 0;
            string name = "";
            string telephone = "";
            string email = "";
            string role = "";

            using (var conn = GetConnection())
            {
                conn.Open();

                string person_sql = "SELECT person_id, name, telephone, email, role " +
                    "FROM person " +
                    "WHERE person_id = @id;";

                using (var person_cmd = new MySqlCommand(person_sql, conn))
                {
                    person_cmd.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader person_rdr = person_cmd.ExecuteReader())
                        while (person_rdr.Read())
                        {
                            personID = person_rdr.GetInt32("person_id");
                            name = person_rdr.GetString("name");
                            telephone = person_rdr.GetString("telephone");
                            email = person_rdr.GetString("email");
                            role = person_rdr.GetString("role");

                            if (role == "PERFORMER")
                            {
                                person_rdr.Close();

                                int fee = 0;

                                string performer_sql = "SELECT fee FROM performer WHERE person_id = @id;";
                                using (var performer_cmd = new MySqlCommand(performer_sql, conn))
                                {
                                    performer_cmd.Parameters.AddWithValue("@id", id);
                                    using (MySqlDataReader performer_rdr = performer_cmd.ExecuteReader())
                                        while (performer_rdr.Read())
                                        {
                                            fee = performer_rdr.GetInt32("fee");
                                        }
                                }

                                List<string> genres = new List<string>();

                                string genres_sql = "SELECT genre FROM genre_performer WHERE performer = @id;";
                                using (var genres_cmd = new MySqlCommand(genres_sql, conn))
                                {
                                    genres_cmd.Parameters.AddWithValue("@id", id);
                                    using (MySqlDataReader genres_rdr = genres_cmd.ExecuteReader())
                                        while (genres_rdr.Read())
                                        {
                                            genres.Add(genres_rdr.GetString("genre"));
                                        }
                                }
                                return new Performer(personID, name, telephone, email, fee, genres);
                            }
                            else if (role == "CREW")
                            {
                                person_rdr.Close();

                                int rate = 0;
                                string employment = "";
                                int hours = 0;

                                string crew_sql = "SELECT hourly_rate, employment, weekly_hours FROM crew WHERE person_id = @id;";
                                using (var crew_cmd = new MySqlCommand(crew_sql, conn))
                                {
                                    crew_cmd.Parameters.AddWithValue("@id", id);
                                    using (MySqlDataReader crew_rdr = crew_cmd.ExecuteReader())
                                        while (crew_rdr.Read())
                                        {
                                            rate = crew_rdr.GetInt32("hourly_rate");
                                            employment = crew_rdr.GetString("employment");
                                            hours = crew_rdr.GetInt32("weekly_hours");
                                        }
                                }
                                return new Crew(personID, name, telephone, email, rate, employment, hours);
                            }

                            else if (role == "VENDOR")
                            {
                                person_rdr.Close();

                                List<string> categories = new List<string>();

                                string vendor_sql = "SELECT category FROM vendor_category WHERE vendor = @id;";
                                using (var vendor_cmd = new MySqlCommand(vendor_sql, conn))
                                {
                                    vendor_cmd.Parameters.AddWithValue("@id", id);
                                    using (MySqlDataReader vendor_rdr = vendor_cmd.ExecuteReader())
                                        while (vendor_rdr.Read())
                                        {
                                            categories.Add(vendor_rdr.GetString("category"));
                                        }
                                }
                                return new Vendor(personID, name, telephone, email, categories);
                            }
                        }
                }
            }
            return null;
        }

        private Person? MapRowToItem(MySqlDataReader reader)
        {
            int personID = reader.GetInt32("person_id");
            string name = reader.GetString("name");
            string telephone = reader.GetString("telephone");
            string email = reader.GetString("email");
            string role = reader.GetString("role");

            Person result;   // this will be assigned in every branch below

            if (role == "PERFORMER")
            {
                int fee = reader.GetInt32("fee");

                // genres need a separate query — they're a one-to-many list
                List<string> genres = GetGenresForPerformer(personID);

                result = new Performer(personID, name, telephone, email, fee, genres);
            }
            else if (role == "CREW")
            {
                int rate = reader.GetInt32("hourly_rate");
                string employment = reader.GetString("employment");
                int hours = reader.GetInt32("weekly_hours");

                result = new Crew(personID, name, telephone, email, rate, employment, hours);
            }
            else if (role == "VENDOR")
            {
                List<string> categories = GetCategoriesForVendor(personID);

                result = new Vendor(personID, name, telephone, email, categories);
            }
            else
            {
                //this should never run as role can only be PERFORMER/CREW/VENDOR
                //but compiler needs an else case
                //role was something unexpected
                throw new InvalidOperationException($"Unknown role: {role}");
            }

            return result;
        }

        private List<string> GetGenresForPerformer(int personID)
        {
            var genres = new List<string>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT genre FROM genre_performer WHERE performer = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", personID);
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            genres.Add(rdr.GetString("genre"));
                }
            }
            return genres;
        }

        private List<string> GetCategoriesForVendor(int personID)
        {
            var categories = new List<string>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT category FROM vendor_category WHERE vendor = @id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", personID);
                    using (var rdr = cmd.ExecuteReader())
                        while (rdr.Read())
                            categories.Add(rdr.GetString("category"));
                }
            }
            return categories;
        }

        public List<Person> SearchByName(string searchTerm)
        {
            List<Person> results = new List<Person>();

            using (var conn = GetConnection())
            {
                conn.Open();
                string sql = BASE_SELECT_SQL +
                             @" WHERE LOWER(person.name) LIKE CONCAT('%', LOWER(@term), '%')
                                ORDER BY person.name;";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@term", searchTerm);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            results.Add(MapRowToItem(reader));
                    }
                }
            }

            return results;
        }
    }
}