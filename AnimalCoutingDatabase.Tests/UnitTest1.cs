using AnimalCoutingDatabase.Api;
using AnimalCoutingDatabase.Api.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AnimalCoutingDatabase.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.True(1 == 1);
        }

        [Fact]
        public async Task CustomerIntegrationTest()
        {
            // Create DB context
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionBuilder = new DbContextOptionsBuilder<CustomerContext>();
            optionBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            var context = new CustomerContext(optionBuilder.Options);

            // Just to make sure: Delete all existing customers in the DB
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // Create a controller
            var controller = new CustomersController(context);

            // Add customer
            await controller.PostCustomer(new Customer() { CustomerName = "FooBar" });

            // Check: Does GetAll return the added customer?
            var result = (await controller.GetCustomers()).ToArray();

            Assert.Single(result);
            Assert.Equal("FooBar", result[0].CustomerName);

        }
    }
}
