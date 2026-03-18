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
        private readonly string _cs = "server=localhost;database=festival;uid=root;pwd='';";
        private MySqlConnection GetConnection() => new MySqlConnection(_cs); 

        public SQLRepository2()
        {

        }

        // The shared SELECT + LEFT JOIN SQL used by all read methods
        // An optional WHERE clause is appended by each calling method
        private const string BASE_SELECT_SQL =
            @"SELECT
                    person.person_id,
                    person.name,
                    person.telephone,
                    person.email,
                    performer.fee,
                    crew.hourly_rate,
                    crew.employment,
                    crew.weekly_hours,
                FROM person
                LEFT JOIN performer ON peformer.person_id = person.person_id
                LEFT JOIN crew ON crew.person_id = person.person_id
                LEFT JOIN vendor ON vendor.person_id = vendor.person_id";

        public List<Person> GetAll()
        {
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
                            personID = person_rdr.GetInt32("person_id");
                            name = person_rdr.GetString("name");
                            telephone = person_rdr.GetString("telephone");
                            email = person_rdr.GetString("email");
                            role = person_rdr.GetString("role");

                            if (role == "PERFORMER")
                            {
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
                                                genres.Add(genres_rdr.GetString("genre"));
                                            }
                                        }
                                    }
                                    people.Add(new Performer(personID, name, telephone, email, fee, genres));
                                }   
                            }
                            else if (role == "CREW")
                            {
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
                                                rate = crew_rdr.GetInt32("hourly_rate");
                                                employment = crew_rdr.GetString("employment");
                                                hours = crew_rdr.GetInt32("weekly_hours");
                                            }
                                        }
                                    }
                                    people.Add(new Crew(personID, name, telephone, email, rate, employment, hours));
                                }   
                            }
                            else if (role == "VENDOR")
                            {
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
                                                categories.Add(vendor_rdr.GetString("category"));
                                            }
                                        }
                                    }
                                    people.Add(new Vendor(personID, name, telephone, email, categories));
                                }  
                            }
                        }
                        return people;
                    }
                }
            }
        }

        public List<Person>? GetByRole(string role)
        {
            int personID;
            string name;
            string telephone;
            string email;

            using (var conn = GetConnection())
            {
                conn.Open();
                if (role == "PERFORMER")
                {
                    List<Person> performers = new List<Person>();

                    int fee;

                    string performer_sql = "SELECT person.person_id, person.name, person.telephone, person.email, performer.fee " +
                        "FROM person, performer " +
                        "WHERE person.person_id = performer.person_id;";
                    using (var performer_cmd = new MySqlCommand(performer_sql, conn))
                    using (MySqlDataReader performer_rdr = performer_cmd.ExecuteReader())
                    {
                        while (performer_rdr.Read())
                        {
                            personID = performer_rdr.GetInt32("person_id");
                            name = performer_rdr.GetString("name");
                            telephone = performer_rdr.GetString("telephone");
                            email = performer_rdr.GetString("email");
                            fee = performer_rdr.GetInt32("fee");

                            List<string> genres = new List<string>();

                            using (var conn2 = GetConnection())
                            {
                                conn2.Open();
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
                            performers.Add(new Performer(personID, name, telephone, email, fee, genres));
                        }
                    }
                    return performers;
                }
                else if (role == "CREW")
                {
                    int rate;
                    string employment;
                    int hours;

                    List<Person> crew = new List<Person>();

                    string crew_sql = "SELECT person.person_id, person.name, person.telephone, person.email, " +
                        "hourly_rate, employment, weekly_hours FROM person, crew " +
                        "WHERE person.person_id = crew.person_id;";
                    using (var crew_cmd = new MySqlCommand(crew_sql, conn))
                    using (MySqlDataReader crew_rdr = crew_cmd.ExecuteReader())
                    {
                        while (crew_rdr.Read())
                        {
                            personID = crew_rdr.GetInt32("person_id");
                            name = crew_rdr.GetString("name");
                            telephone = crew_rdr.GetString("telephone");
                            email = crew_rdr.GetString("email");
                            rate = crew_rdr.GetInt32("hourly_rate");
                            employment = crew_rdr.GetString("employment");
                            hours = crew_rdr.GetInt32("weekly_hours");

                            crew.Add(new Crew(personID, name, telephone, email, rate, employment, hours));
                        }
                    }
                    return crew;
                }
                else if (role == "VENDOR")
                {
                    List<Person> vendors = new List<Person>();

                    string vendor_sql = "SELECT person.person_id, person.name, person.telephone, person.email " +
                        "FROM person, vendor " +
                        "WHERE person.person_id = vendor.person_id;";
                    using (var vendor_cmd = new MySqlCommand(vendor_sql, conn))
                    using (MySqlDataReader vendor_rdr = vendor_cmd.ExecuteReader())
                    {
                        while (vendor_rdr.Read())
                        {
                            personID = vendor_rdr.GetInt32("person_id");
                            name = vendor_rdr.GetString("name");
                            telephone = vendor_rdr.GetString("telephone");
                            email = vendor_rdr.GetString("email");

                            List<string> categories = new List<string>();

                            using (var conn2 = GetConnection())
                            {
                                conn2.Open();
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
                            vendors.Add(new Vendor(personID, name, telephone, email, categories));
                        }
                    }
                    return vendors;
                }
            }
            return null;
        }

        public void Add(Person person)
        {
            if (person == null) throw new Exception("Person cannot be a null object.");

            using (var conn = GetConnection())
            {
                conn.Open();
                using (MySqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        int personID = 0;

                        // Base insert
                        using (MySqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText =
                                @"INSERT INTO person
                                      (name, telephone, email, role)
                                      VALUES
                                      (@name, @telephone, @email, @role);";

                            cmd.Parameters.AddWithValue("@name", person.Name);
                            cmd.Parameters.AddWithValue("@telephone", person.Telephone);
                            cmd.Parameters.AddWithValue("@email", person.Email);

                            if (person is Performer) cmd.Parameters.AddWithValue("@role", "PERFORMER");

                            else if (person is Crew) cmd.Parameters.AddWithValue("@role", "CREW");

                            else if (person is Vendor) cmd.Parameters.AddWithValue("@role", "VENDOR");

                            else throw new Exception("Unsupported role for person");

                            cmd.ExecuteNonQuery();

                            personID = (int)cmd.LastInsertedId;
                        }

                        // Derived insert
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
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void AddPerformer(MySqlConnection conn, MySqlTransaction tx, Performer performer, int personID)
        {

            List<string> genres = performer.get_genres();
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
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

                    // insert into genre_performers if it doesn't already exist
                    cmd.CommandText =
                        @"INSERT INTO genre (genre_name)
                        SELECT @genre
                        WHERE NOT EXISTS (
                           SELECT 1 FROM genre WHERE genre_name = @genre
                        );";
                    cmd.Parameters.AddWithValue("@genre", genre);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText =
                        @"INSERT INTO genre_performer
                            (genre, performer)
                            VALUES
                            (@genre, @person_id);";

                    cmd.Parameters.AddWithValue("@person_id", personID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddCrew(MySqlConnection conn, MySqlTransaction tx, Crew crew, int personID)
        {
            using (MySqlCommand cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText =
                    @"INSERT INTO crew
                            (person_id, rate, employment, hours)
                            VALUES
                            (@person_id, @hourly_rate, @employment, @weekly_hours);";

                // need to check hours is in line with emloyment type
                // check spec for buisiness / logic divide

                cmd.Parameters.AddWithValue("@person_id", personID);
                cmd.Parameters.AddWithValue("@hourly_rate", crew.Rate);
                cmd.Parameters.AddWithValue("@employment", crew.Employment);
                cmd.Parameters.AddWithValue("@weekly_hours", crew.Hours);

                cmd.ExecuteNonQuery();
            }
        }

        public void AddVendor(MySqlConnection conn, MySqlTransaction tx, Vendor vendor, int personID)
        {
            List<string> categories = vendor.product_categories;

            using (MySqlCommand cmd = conn.CreateCommand())
            {
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

                    // insert into product_categories if it doesn't already exist
                    cmd.CommandText =
                        @"INSERT INTO product_categories (product_category)
                        SELECT @category
                        WHERE NOT EXISTS (
                           SELECT 1 FROM product_categories WHERE product_category = @category
                        );";
                    cmd.Parameters.AddWithValue("@category", category);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText =
                        @"INSERT INTO vendor_category
                                (vendor, category)
                                VALUES
                                (@person_id, @category);";

                    cmd.Parameters.AddWithValue("@person_id", personID);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ON DELETE CASCADE in the DB removes the performer/crew/vendor as well
        public bool Delete(int id)
        {
            if (id == 0) throw new Exception("Person cannot have an invalid ID number.");

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
                            cmd.CommandText =
                                @"DELETE FROM `person` 
                                    WHERE person_id = @person_id;";

                            cmd.Parameters.AddWithValue("@person_id", id);
                            cmd.ExecuteNonQuery();
                        }
                        tx.Commit();
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        return false;
                        throw;
                    }
                }
            }
        }

        public bool EditByID(int id, string column, string user_value)
        {
            if (id == 0) throw new Exception("ID cannot be null.");

            string[] allowed = { "name", "telephone", "email", "fee", "hourly_rate", "employment", "weekly_hours", "category" };
            if (!allowed.Contains(column))
                throw new Exception("Invalid column name.");

            Person person = FindByID(id);
            if (person == null) throw new Exception("Person not found.");

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
                            if (column == "name" || column == "telephone" || column == "email")
                            {
                                cmd.CommandText =
                                    $"UPDATE `person` SET `{column}` = @user_value WHERE person_id = @person_id;";

                                cmd.Parameters.AddWithValue("@person_id", person.PersonID);
                                cmd.Parameters.AddWithValue("@user_value", user_value);
                                cmd.ExecuteNonQuery();
                            }
                            // admin may want to change performers fee or the category/categories they perform in
                            else if (person.Role == "PERFORMER")
                            {
                                string table;

                                if (column == "genre")
                                {
                                    table = "genre_performer";
                                }
                                else
                                {
                                    table = "performer";
                                }
                                cmd.CommandText =
                                    $"UPDATE `{table}` SET `{column}` = @user_value WHERE person_id = @person_id;";

                                cmd.Parameters.AddWithValue("@person_id", person.PersonID);
                                cmd.Parameters.AddWithValue("@user_value", user_value);
                                cmd.ExecuteNonQuery();
                            }
                            else if (person.Role == "CREW")
                            {
                                cmd.CommandText =
                                    $"UPDATE `crew` SET `{column}` = @user_value WHERE person_id = @person_id;";

                                cmd.Parameters.AddWithValue("@person_id", person.PersonID);
                                cmd.Parameters.AddWithValue("@user_value", user_value);
                                cmd.ExecuteNonQuery();
                            }
                            // admin wants to change product category as this is the only role specific field vendor has 
                            else if (person.Role == "VENDOR")
                            {
                                cmd.CommandText =
                                    @"UPDATE `vendor_category` 
                                        SET `category` = @user_value
                                        WHERE person_id = @person_id;";

                                cmd.Parameters.AddWithValue("@person_id", id);
                                cmd.Parameters.AddWithValue("@user_value", user_value);
                                cmd.ExecuteNonQuery();
                            }
                            tx.Commit();
                            return true;
                        }
                    }
                    catch
                    {
                        tx.Rollback();
                        return false;
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
    }
}