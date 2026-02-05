using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresUsername = builder.AddParameter("PostgresUsername");
var postgresPassword = builder.AddParameter("PostgresPassword", secret: true);

var postgres = builder.AddPostgres(
        name: "db",
        userName: postgresUsername,
        password: postgresPassword)
    .WithPgAdmin()
    .WithDataVolume();

var coreDb = postgres.AddDatabase("database");

var api = builder.AddProject<WolverineIssue>("api")
    .WaitFor(postgres)
    .WithReference(coreDb)
    .WithExternalHttpEndpoints();

var worker = builder.AddProject<WolverineIssue_Worker>("worker")
    .WaitFor(postgres)
    .WithReference(coreDb)
    .WithExternalHttpEndpoints();

builder.Build().Run();
