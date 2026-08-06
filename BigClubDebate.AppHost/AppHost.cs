var builder = DistributedApplication.CreateBuilder(args);

if (builder.ExecutionContext.IsPublishMode)
{
    builder.AddAzureContainerAppEnvironment("env");
}

var web = builder.AddProject<Projects.BigClubDebate_Web>("web")
    .WithExternalHttpEndpoints();

builder.Build().Run();
