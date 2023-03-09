using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace DapperCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        public IConfiguration _config { get; }

        public SuperHeroController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuperHero> heroes = await SelectAllHeroes(connection);

            return Ok(heroes);

        }


        [HttpGet("{id}")]
        public async Task<ActionResult<SuperHero>> GetSuperHero(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string sql = "SELECT * FROM SuperHeroes where id = @id";
            var param = new { id = id };
            var hero = await connection.QueryFirstAsync<SuperHero>(sql, param);

            return Ok(hero);

        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> AddSuperHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync(@"INSERT INTO [dbo].[superHeroes]
                                           ([name]
                                           ,[first_name]
                                           ,[last_name]
                                           ,[place]
                                           ,[age])
                                     VALUES (@name
                                             ,@first_name
                                             ,@last_name
                                             ,@place
                                             ,@age
                                            )", hero);

            return Ok(await SelectAllHeroes(connection));

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteSuperHero(int id)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            await connection.ExecuteAsync("DELETE SuperHeroes WHERE id = @id", new {id = id});

            return Ok(await SelectAllHeroes(connection));

        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateSuperHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string sql = "UPDATE SuperHeroes SET name = @name ,first_name = @first_name, last_name = @last_name, place = @place, age = @age WHERE id = @id";

            await connection.ExecuteAsync(sql, hero);

            return Ok(await SelectAllHeroes(connection));

        }


        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            string sql = "SELECT * FROM SuperHeroes";
            return await connection.QueryAsync<SuperHero>(sql);
        }


    }
}
