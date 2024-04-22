using System.Data;
using apbd0.Models;
using apbd0.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace apbd0.Controllers;

[ApiController]
// [Route("api/animals")]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult GetAnimals(string orderBy = "name")
    {
        string[] validColumns = { "name", "description", "category", "area" };
        if (!validColumns.Contains(orderBy.ToLower()))
        {
            return BadRequest("Invalid orderBy parameter. Valid values are: name, description, category, area.");
        }
        
        // Uruchamiamy połączenie do bazy
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        //Definiujemy command
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM Animal ORDER BY " + orderBy;
        
        // Uruchomienie zapytania
        var reader = command.ExecuteReader();

        List<Animal> animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");
        int descriptionOrdinal = reader.GetOrdinal("Description");
        int categoryOrdinal = reader.GetOrdinal("Category");
        int areaOrdinal = reader.GetOrdinal("Area");
        
        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                // IdAnimal = reader["IdAnimal"].ToString(),
                // Name = reader["Name"].ToString()
                
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal),
                Description = reader.GetString(descriptionOrdinal),
                Category = reader.GetString(categoryOrdinal),
                Area = reader.GetString(areaOrdinal)
            });
        }
        
        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimal addAnimal)
    {
        // Uruchamiamy połączenie do bazy
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        //Definiujemy command
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES(@animalName, @description, @Category, @Area)";
        command.Parameters.AddWithValue("@animalName", addAnimal.Name);
        command.Parameters.AddWithValue("@description", addAnimal.Description);
        command.Parameters.AddWithValue("@category", addAnimal.Category);
        command.Parameters.AddWithValue("@area", addAnimal.Area);
        
        // Wykonanie commanda
        command.ExecuteNonQuery();
        
        return Created("", null);
    }
    
    [HttpPut("{idAnimal}")]
    public IActionResult UpdateAnimal(int idAnimal, AddAnimal updateAnimal)
    { 
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "UPDATE Animal SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal";
        command.Parameters.AddWithValue("@Name", updateAnimal.Name);
        command.Parameters.AddWithValue("@Description", updateAnimal.Description ?? "");
        command.Parameters.AddWithValue("@Category", updateAnimal.Category);
        command.Parameters.AddWithValue("@Area", updateAnimal.Area);
        command.Parameters.AddWithValue("@IdAnimal", idAnimal);

        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected == 0)
        {
            return NotFound("Animal not found.");
        }
    

        return Ok("Animal updated successfully.");
        }
    
    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "DELETE FROM Animal WHERE IdAnimal = @IdAnimal";
        command.Parameters.AddWithValue("@IdAnimal", idAnimal);

        int rowsAffected = command.ExecuteNonQuery();

        if (rowsAffected == 0)
        {
            return NotFound("Animal not found.");
        }
        

        return Ok("Animal deleted successfully.");
        
    }
    
}