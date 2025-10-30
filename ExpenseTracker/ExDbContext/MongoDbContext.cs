// using ExpenseTracker.Entities;
// using MongoDB.Driver;

// namespace ExpenseTracker.ExDbContext
// {
//     public class MongoDbContext
//     {
//         private readonly IMongoDatabase _database;

//         public MongoDbContext(IConfiguration configuration)
//         {
//             var connectionString = configuration.GetConnectionString("MongoDb");
//             var mongoUrl = new MongoUrl(connectionString);
//             var client = new MongoClient(mongoUrl);
//             _database = client.GetDatabase(mongoUrl.DatabaseName);
//         }

//         public IMongoCollection<ActivityLog> ActivityLogs => _database.GetCollection<ActivityLog>("ActivityLogs");
//     }
// }