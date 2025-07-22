# Visão Geral da Arquitetura

## Arquitetura do Sistema
A Plataforma de Educação Online segue uma arquitetura baseada em Domain-Driven Design (DDD) com princípios de Clean Architecture. O sistema é dividido em múltiplos contextos delimitados, cada um responsável por um domínio de negócio específico.

## Camadas Arquiteturais

### 1. Camada de Apresentação (API)
- ASP.NET Core WebAPI
- Endpoints RESTful
- Autenticação JWT
- Documentação Swagger
- DTOs de Requisição/Resposta

### 2. Camada de Aplicação
- Implementação CQRS
- Handlers de Comando e Consulta
- Serviços de Aplicação
- DTOs e mapeamentos
- Validação

### 3. Camada de Domínio
- Entidades de domínio
- Objetos de valor
- Serviços de domínio
- Eventos de domínio
- Interfaces de repositório
- Regras de negócio e invariantes

### 4. Camada de Infraestrutura
- Implementações de repositório
- Contexto de banco de dados
- Integrações com serviços externos
- Logging
- Cache

## Bounded contexts (BCs)

### Gestão de Conteúdo
- Gestão de cursos e aulas
- Organização de conteúdo
- Manipulação de materiais

### Gestão de Alunos
- Matrícula de alunos
- Acompanhamento de progresso
- Geração de certificados

### Faturamento
- Processamento de pagamentos
- Geração de faturas
- Gestão de status de pagamento

### Identidade
- Autenticação de usuários
- Autorização
- Gestão de papéis

## Preocupações Transversais
- Logging
- Tratamento de exceções
- Validação
- Segurança

## Design do Banco de Dados
- SQL Server para produção
- SQLite para desenvolvimento e testes
- Entity Framework Core para acesso a dados
- Abordagem code-first
- Migrações

## Segurança
- Autenticação baseada em JWT
- Autorização baseada em papéis
- Manipulação segura de senhas
- Melhores práticas de segurança de API

## Pontos de Integração
- Integração com PayPal simulada para processamento de pagamentos
