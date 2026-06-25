var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("env");

var web = builder.AddProject<Projects.BigClubDebate_Web>("web")
    .WithExternalHttpEndpoints();

builder.Build().Run();
