var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.CanadaElects_ApiService>("apiservice");

builder.AddProject<Projects.CanadaElects_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
