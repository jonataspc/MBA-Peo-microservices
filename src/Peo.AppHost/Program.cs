var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("messaging")
            .WithDataVolume(isReadOnly: false)
            .WithManagementPlugin()
            .WithLifetime(ContainerLifetime.Persistent);

var password = builder.AddParameter("sqlpassword", secret: true);

// Separate SQL Server instances for each microservice
var identitySqlServer = builder.AddSqlServer("identity-sqlserver", password)
            .WithDataVolume(isReadOnly: false)
            .WithLifetime(ContainerLifetime.Persistent);

var faturamentoSqlServer = builder.AddSqlServer("faturamento-sqlserver", password)
            .WithDataVolume(isReadOnly: false)
            .WithLifetime(ContainerLifetime.Persistent);

var gestaoAlunosSqlServer = builder.AddSqlServer("gestao-alunos-sqlserver", password)
            .WithDataVolume(isReadOnly: false)
            .WithLifetime(ContainerLifetime.Persistent);

var gestaoConteudoSqlServer = builder.AddSqlServer("gestao-conteudo-sqlserver", password)
            .WithDataVolume(isReadOnly: false)
            .WithLifetime(ContainerLifetime.Persistent);

// Each microservice gets its own database on its dedicated instance
var identityDb = identitySqlServer.AddDatabase("identity-db");
var faturamentoDb = faturamentoSqlServer.AddDatabase("faturamento-db");
var gestaoAlunosDb = gestaoAlunosSqlServer.AddDatabase("gestao-alunos-db");
var gestaoConteudoDb = gestaoConteudoSqlServer.AddDatabase("gestao-conteudo-db");

var faturamentoSvc = builder.AddProject<Projects.Peo_Faturamento_WebApi>("peo-faturamento-webapi")
           .WithReference(rabbitmq)
           .WaitFor(rabbitmq)
           .WithReference(faturamentoDb)
           .WaitFor(faturamentoDb);

var gestaoAlunosSvc = builder.AddProject<Projects.Peo_GestaoAlunos_WebApi>("peo-gestao-alunos-webapi")
           .WithReference(rabbitmq)
           .WaitFor(rabbitmq)
           .WithReference(gestaoAlunosDb)
           .WaitFor(gestaoAlunosDb);

var gestaoConteudoSvc = builder.AddProject<Projects.Peo_GestaoConteudo_WebApi>("peo-gestao-conteudo-webapi")
           .WithReference(rabbitmq)
           .WaitFor(rabbitmq)
           .WithReference(gestaoConteudoDb)
           .WaitFor(gestaoConteudoDb);

var identitySvc = builder.AddProject<Projects.Peo_Identity_WebApi>("peo-identity-webapi")
           .WithReference(rabbitmq)
           .WaitFor(rabbitmq)
           .WithReference(identityDb)
           .WaitFor(identityDb);

var bff = builder.AddProject<Projects.Peo_Web_Bff>("peo-bff")
        .WithReference(faturamentoSvc)
        .WaitFor(faturamentoSvc)
        .WithReference(gestaoAlunosSvc)
        .WaitFor(gestaoAlunosSvc)
        .WithReference(gestaoConteudoSvc)
        .WaitFor(gestaoConteudoSvc)
        .WithReference(identitySvc)
        .WaitFor(identitySvc)
        .WithExternalHttpEndpoints();

builder.AddProject<Projects.Peo_Web_Spa>("peo-frontend")       
       .WithExternalHttpEndpoints()
       .WithReference(bff)
       .WaitFor(bff);

builder.Build().Run();