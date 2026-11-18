using Microsoft.Data.SqlClient;

namespace WebApplication14.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int UserId { get; set; }
    }

    public class ItemRepository
    {
        private readonly string _conn;

        public ItemRepository(string conn)
        {
            _conn = conn;
        }

        private Item Read(SqlDataReader r)
        {
            return new Item
            {
                Id = r.GetInt32(0),
                Title = r.GetString(1),
                Description = r.GetString(2),
                Price = r.GetInt32(3),
                UserId = r.GetInt32(4)
            };
        }

        public List<Item> GetAll()
        {
            var list = new List<Item>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT Id, Title, Description, Price, UserId FROM Items", con);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(Read(r));
                    }
                }
            }
            return list;
        }

        public Item GetById(int id)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT Id, Title, Description, Price, UserId FROM Items WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read()) return Read(r);
                }
            }
            return null;
        }
                public List<Item> GetByUser(int userId)
        {
            var list = new List<Item>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT Id, Title, Description, Price, UserId FROM Items WHERE UserId = @UserId", con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(Read(r));
                    }
                }
            }
            return list;
        }

        public int CountByUser(int userId)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Items WHERE UserId = @UserId", con);
                cmd.Parameters.AddWithValue("@UserId", userId);
                return (int)cmd.ExecuteScalar();
            }
        }

        public void Add(Item item)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("INSERT INTO Items (Title, Description, Price, UserId) VALUES (@Title, @Description, @Price, @UserId)", con);
                cmd.Parameters.AddWithValue("@Title", item.Title);
                cmd.Parameters.AddWithValue("@Description", item.Description);
                cmd.Parameters.AddWithValue("@Price", item.Price);
                cmd.Parameters.AddWithValue("@UserId", item.UserId);
                cmd.ExecuteNonQuery();
            }
        }

        public bool Update(Item item)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("UPDATE Items SET Title = @Title, Description = @Description, Price = @Price WHERE Id = @Id AND UserId = @UserId", con);
                cmd.Parameters.AddWithValue("@Title", item.Title);
                cmd.Parameters.AddWithValue("@Description", item.Description);
                cmd.Parameters.AddWithValue("@Price", item.Price);
                cmd.Parameters.AddWithValue("@Id", item.Id);
                cmd.Parameters.AddWithValue("@UserId", item.UserId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int id, int userId)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("DELETE FROM Items WHERE Id = @Id AND UserId = @UserId", con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", userId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}