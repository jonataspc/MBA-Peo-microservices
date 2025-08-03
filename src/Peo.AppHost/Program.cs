var builder = DistributedApplication.CreateBuilder(args);


var faturamentoSvc = builder.AddProject<Projects.Peo_Faturamento_WebApi>("peo-faturamento-webapi");

var gestaoAlunosSvc = builder.AddProject<Projects.Peo_GestaoAlunos_WebApi>("peo-gestaoalunos-webapi");

var gestaoConteudoSvc = builder.AddProject<Projects.Peo_GestaoConteudo_WebApi>("peo-gestaoconteudo-webapi");

var identitySvc = builder.AddProject<Projects.Peo_Identity_WebApi>("peo-identity-webapi");



builder.AddProject<Projects.Peo_Web_Spa>("peo-frontend");

builder.Build().Run();
