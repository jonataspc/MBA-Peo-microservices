var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("messaging")
            .WithDataVolume(isReadOnly: false)
            .WithManagementPlugin();

var faturamentoSvc = builder.AddProject<Projects.Peo_Faturamento_WebApi>("peo-faturamento-webapi")
           .WithReference(rabbitmq)
           .WaitFor(rabbitmq);

var gestaoAlunosSvc = builder.AddProject<Projects.Peo_GestaoAlunos_WebApi>("peo-gestaoalunos-webapi")
           .WithReference(rabbitmq)
           .WaitFor(rabbitmq);

var gestaoConteudoSvc = builder.AddProject<Projects.Peo_GestaoConteudo_WebApi>("peo-gestaoconteudo-webapi")
           .WithReference(rabbitmq)
           .WaitFor(rabbitmq);

var identitySvc = builder.AddProject<Projects.Peo_Identity_WebApi>("peo-identity-webapi")
           .WithReference(rabbitmq)
           .WaitFor(rabbitmq);

var bff = builder.AddProject<Projects.Peo_Web_Bff>("peo-bff")
        .WithReference(faturamentoSvc)
        .WaitFor(faturamentoSvc)
        .WithReference(gestaoAlunosSvc)
        .WaitFor(gestaoAlunosSvc)
        .WithReference(gestaoConteudoSvc)
        .WaitFor(gestaoConteudoSvc)
        .WithReference(identitySvc)
        .WaitFor(identitySvc);

builder.AddProject<Projects.Peo_Web_Spa>("peo-frontend")
       .WithExternalHttpEndpoints()
       .WithReference(bff)
       .WaitFor(bff);

builder.Build().Run();