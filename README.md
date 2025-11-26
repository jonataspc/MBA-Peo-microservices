[![.NET](https://github.com/jonataspc/MBA-Peo-microservices/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jonataspc/MBA-Peo-microservices/actions/workflows/dotnet.yml)

# **PEO - Plataforma de Educação Online**

## **Apresentação**

Bem-vindo ao repositório do projeto **Peo**. Este projeto é uma entrega do MBA DevXpert Full Stack .NET e é referente ao quinto módulo do MBA Desenvolvedor.IO.

O objetivo principal é desenvolver uma plataforma educacional online com múltiplos bounded contexts (BC), aplicando DDD, TDD, CQRS, microsserviços e padrões arquiteturais para gestão eficiente de conteúdos educacionais, alunos e processos financeiros. 

Além disso são aplicados conceitos de devops, como Git/GitHub, Docker, GitHub Actions, Kubernetes e Cultura DevOps.



### **Autor**
- **Jonatas Cruz**

## **Proposta do Projeto**

O projeto consiste em:

- **API RESTful:** Exposição dos endpoints necessários para os casos de uso.
- **Autenticação e Autorização:** Implementação de controle de acesso, diferenciando administradores e alunos.
- **Acesso a Dados:** Implementação de acesso ao banco de dados através de ORM.

## **Tecnologias Utilizadas**

- **Linguagem de Programação:** C#
- **Frameworks:**
  - ASP.NET Blazor
  - ASP.NET Core Web API
  - ASP.NET Aspire
  - Entity Framework Core
- **Componentes/Bibliotecas:**
  - MudBlazor
  - NSwag
  - MassTransit
  - MediatR
- **Banco de Dados:** 
  - SQL Server / SQLite
- **Mensageria:** 
  - RabbitMQ
- **Autenticação e Autorização:**
  - ASP.NET Core Identity
  - JWT (JSON Web Token) para autenticação na API
- **Documentação da API:** 
  - Swagger
- **Devops:** 
  - Aspir8 (geração automatizada dos manifestos a partir do Aspire)
  - kubernetes
  - SonarQube
  - GitHub Actions
  - Imagens ASPNET e SDK Alpine
	 
	 
## **Estrutura do Projeto**

A estrutura do projeto é organizada da seguinte forma:

- src: códigos-fonte da solução  
- tests: testes de integração e de unidade.
- docs: [documentação do projeto](./docs/README.md) e requisitos
- devops: scripts e manifestos para deploy (kubernetes/Docker)
	
- README.md: Arquivo de Documentação do Projeto
- FEEDBACK.md: Arquivo para Consolidação dos Feedbacks
- DEVELOPMENT.md: Notas de apoio para o desenvolvimento
- .gitignore: Arquivo de Ignoração do Git
- .gitattributes: Atributos do Git
- .editorconfig: Preferências de Estilo de Código

## **Como Executar o Projeto**

### **Pré-requisitos**

- .NET SDK 9.0 ou superior
- SQL Server ou SQLite
- Docker (ou outra solução de container)
- Visual Studio 2022 ou superior (ou qualquer IDE de sua preferência)
- Git

### **Passos para Execução**

1. **Clone o Repositório:**
   - `git clone https://github.com/jonataspc/MBA-Peo-microservices.git`
   - `cd MBA-Peo`

2. **Configuração do Banco de Dados:**
   - Por padrão, em ambiente de desenvolvimento, o projeto está configurado para utilizar SQLite.
   - Caso necessário configure a string de conexão nas aplicações Web-API (`\src\Peo.XXX.WebApi\appsettings.XXX.json`).
   - Rode o projeto para que a configuração do Seed crie o banco e popule com os dados básicos


3. **Executar o Aspire AppHost (garantir que o Docker esteja em execução):**
   - `cd .\src\Peo.AppHost\`
   - `dotnet run --launch-profile "https"`
   - O dashboard do Aspire estará disponível em: https://localhost:17005 (utilizar o link disponível no console após o comando `dotnet run`))
   - Acesse a documentação da API do BFF (*backend for frontend*) em: https://localhost:7276/
   - O frontend Blazor estará disponível em: https://localhost:7031/ . Utilizar os dados de login (admin) fornecidos abaixo ou registre um novo usuário (aluno).

4. **Credenciais de teste para usuário administrativo:**

Usuário: admin@admin.com
 
Senha: @dmin!

## **Instruções de Configuração**

- **JWT para API:** As chaves de configuração do JWT estão nos arquivos `\src\Peo.XXX.Api\appsettings.json`.
- **Migrações do Banco de Dados:** As migrações são gerenciadas pelo Entity Framework Core. Não é necessário aplicar manualmente devido a configuração do seed de dados. 

## **Documentação da API**

A documentação da API BFF está disponível através do Swagger. Após iniciar a API, acesse a documentação em https://localhost:7276/

## **Documentação do projeto**
Uma documentação extensiva pode ser obtida [aqui](./docs/README.md).


## **Code coverage e CI**
A cobertura de código pode ser gerada manualmente/localmente através do script `\scripts\run-tests-with-coverage.ps1` (que utiliza dotCover) e pode ser visualizada no caminho `\scripts\report.html`. 

No repositório GitHub a action de compilação executa a compilação e os testes, além de gerar o relatório de cobertura de código e armazena-lo como artefato, que pode ser obtido: 
- Acesse a aba "Actions" do repositório.
- Escolha o último workflow executado.
- Na seção Artifacts, baixar o arquivo ZIP, contendo o relatório em HTML.
 

## **Devops**
- Para geração dos manifestos do Kubernetes e deploy, utilize Aspir8 (https://github.com/prom3theu5/aspirational-manifests) à partir do path `\src\Peo.AppHost`

```bash
dotnet tool install -g aspirate --prerelease

# k8s
aspirate generate --output-path ..\..\devops\k8s-manifests

# docker-compose
aspirate generate --output-path ..\..\devops\docker-compose-manifests --output-format compose

```

Aplicar os manifestos no cluster Kubernetes:
```bash
kubectl apply -k .\devops\k8s-manifests
```

## **Avaliação**

- Este projeto é parte de um curso acadêmico e não aceita contribuições externas. 
- Para feedbacks ou dúvidas utilize o recurso de Issues
- O arquivo `FEEDBACK.md` é um resumo das avaliações do instrutor e deverá ser modificado apenas por ele.
