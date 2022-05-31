### Without a doubt the most user-friendly way to connect to an existing postgresql database

Today we're going to use a database-first pattern to connect to an existing, hosted postgresql database. 

Based off of this video [here](https://www.youtube.com/watch?v=Mpl4IWFob4I)

And a repo for this very basic skeleton of a project can be found [here](https://github.com/harrisonBarnett/EZPOstgresApi.git)

**First off** you need to have a postgresql database hosted *somewhere*. This can be local, on docker, on an azure server. Doesn't matter, just make sure you know the server name (i.e. localhost, xyz.azure.com, etc), port number, password. All that. Make yourself a table. In this example, we're going to have a boring old table full of `user` in a database named `my_user_db`.

Make the database and run this query:

```
CREATE TABLE users(
	userId int,
	userName varchar(255),
	age int
);

INSERT INTO users
VALUES (1, 'Harrison', 31);
INSERT INTO users
VALUES (2, 'Collin', 32);

SELECT * FROM users;
```
So what we're going to do is make a very simple web api using ASP.NET Core Web Api framework. So go ahead and make that.

Before we do anything too crazy, let's take a look at the `appsettings.json` file. In here we will establish our connection string so that we can plug into our database. So add

```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=my_user_db;User Id=postgres;Password=asterisks"
	},
```
to your appsettings file.

No we need a big, beefy package to do most of the heavy lifting for us. Install the nuget package **Npgsql** and say thank you, past Harrison, for unfucking your day.

Some small details would be to fuck around with cors settings in `Program.cs`, so go ahead and plop down this code in that file:

```
// cors configuration bc it will piss you off later
builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
...
...
...
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
```
This will make it so that our API doesn't lose its fucking mind trying to understand how it's basically talking to itself on a local connection. Computers confuse very easily.

Alright, so go ahead and make a `UserController.cs` class so that we can control our users. Optionally, make yourself a `User.cs` model and stick it in a `Models` file if you wanna try and do some fancy EntityFramework shit, but right now all we're really concerned about it talking to the database. 

*NB: make sure you're making an API controller instead of a boring as fuck MVC controller. You will get mad at this later.*

**UserController.cs**
```
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using System.Data;

namespace EZPostgresApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        NpgsqlConnection myConn;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            string connString = _configuration.GetConnectionString("DefaultConnection");
            myConn = new NpgsqlConnection(connString);
        }

        [HttpGet]
        public string Get()
        {
            string query = "SELECT * FROM users";

            DataTable table = new DataTable();
            NpgsqlDataReader reader;
            myConn.Open();
            NpgsqlCommand cmd = new NpgsqlCommand(query, myConn);
            reader = cmd.ExecuteReader();
            table.Load(reader);

            reader.Close();
            myConn.Close();

            return JsonConvert.SerializeObject(table);
        }
    }
}
```
Now obviously all this thing is doing is returning some dumb fuckin string. That's fine, you can do whatever the fuck you want with the data as long as you're using the `NpgsqlDataReader` to query your database for stuff. 

That's it. That's the setup. Why the fuck does MySQL have to make it so fucking hard?
