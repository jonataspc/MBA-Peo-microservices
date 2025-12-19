# Peo Platform - Docker Compose

Este guia explica como executar toda a plataforma Peo usando Docker Compose.

## Pré-requisitos

- **Docker**: versão 20.10 ou superior
- **Docker Compose**: versão 2.0 ou superior
- **Memória RAM**: mínimo 8GB recomendado
- **Espaço em disco**: ~5GB para imagens e volumes

## Início Rápido

### Desenvolvimento
```bash
# Clone o repositório
git clone https://github.com/jonataspc/MBA-Peo-microservices.git
cd MBA-Peo-microservices

# Configure o ambiente (IMPORTANTE!)
cp .env.example .env

# Edite o .env se necessário (senhas, JWT key, etc.)
# O arquivo .env será automaticamente lido pelo Docker Compose

# Inicie em modo desenvolvimento
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d --build
```

### Produção
```bash
# Configure o ambiente
cp .env.example .env

# IMPORTANTE: Edite o .env com credenciais seguras para produção!
# - Mude SA_PASSWORD
# - Gere novo JWT_KEY
# - Configure RABBITMQ_USER/PASSWORD

# Inicie em modo produção
docker compose up -d --build
```

##  Arquitetura dos Serviços

### Infraestrutura
- **SQL Server** (porta 1433) - Banco de dados principal
- **RabbitMQ** (porta 5672, UI: 15672) - Message broker

### Microserviços
- **Identity API** (porta 5001) - Autenticação e autorização
- **Gestão Conteúdo API** (porta 5002) - Gestão de cursos e conteúdo
- **Gestão Alunos API** (porta 5003) - Gestão de alunos e matrículas
- **Faturamento API** (porta 5004) - Processamento de pagamentos

### Frontend
- **BFF** (porta 5000) - Backend for Frontend / API Gateway
- **Blazor SPA** (porta 3000) - Interface web principal

## URLs da Aplicação

| Serviço | URL | Descrição |
|---------|-----|-----------|
| Frontend | http://localhost:3000 | Interface principal (Blazor) |
| BFF API | http://localhost:5000 | API Gateway / Backend for Frontend |
| Identity API | http://localhost:5001 | Serviço de autenticação |
| Gestão Conteúdo | http://localhost:5002 | API de gestão de conteúdo |
| Gestão Alunos | http://localhost:5003 | API de gestão de alunos |
| Faturamento | http://localhost:5004 | API de faturamento |
| RabbitMQ UI | http://localhost:15672 | Interface de gerenciamento |
| SQL Server | localhost:1433 | Banco de dados |

### Credenciais Padrão

**RabbitMQ Management:**
- Usuário: `guest` (configurável via .env)
- Senha: `guest` (configurável via .env)

**SQL Server:**
- Usuário: `sa`
- Senha: `MyStr0ngP@ssw0rd123` (configurável via .env)

**Aplicação (usuário admin):**
- Email: `admin@admin.com`
- Senha: `@dmin!`

### 📝 Configuração Important para Produção

**URL da API:** O frontend Blazor está configurado automaticamente para:
- **Desenvolvimento (Aspire):** `https://localhost:7276/`
- **Produção (Docker):** `http://localhost:5000/` (BFF)

A configuração é alterada automaticamente através do arquivo `appsettings.Production.json` no container.

## Comandos Docker Compose

### Comandos Básicos
```bash
# Construir imagens
docker compose build

# Iniciar todos os serviços (produção)
docker compose up -d

# Iniciar todos os serviços (desenvolvimento)
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d

# Iniciar com logs visíveis
docker compose up

# Parar serviços
docker compose down

# Ver logs de todos os serviços
docker compose logs -f

# Ver logs de um serviço específico
docker compose logs -f peo-identity-api

# Ver status dos serviços
docker compose ps

# Reiniciar serviços
docker compose restart
```

## Monitoramento e Logs

### Ver logs específicos
```bash
# Logs de um serviço específico
docker compose logs -f peo-identity-api

# Logs das APIs
docker compose logs -f peo-identity-api peo-gestaoconteudo-api peo-gestaoalunos-api peo-faturamento-api

# Logs do frontend e BFF
docker compose logs -f peo-bff peo-frontend

# Logs do banco de dados
docker compose logs -f sqlserver

# Logs do RabbitMQ
docker compose logs -f rabbitmq
```

### Health Checks
```bash
# Status de todos os serviços
docker compose ps

# Verificar se serviços estão respondendo
curl http://localhost:5000/health  # BFF
curl http://localhost:5001/health  # Identity API
```

##  Persistência de Dados

Os dados são persistidos em volumes Docker:
- `peo_sqlserver_data` - Dados do SQL Server
- `peo_rabbitmq_data` - Dados do RabbitMQ

### Backup e Restauração
```bash
# Backup dos volumes
docker run --rm -v peo_sqlserver_data:/data -v $(pwd):/backup alpine tar czf /backup/sqlserver-backup.tar.gz -C /data .

# Restauração
docker run --rm -v peo_sqlserver_data:/data -v $(pwd):/backup alpine tar xzf /backup/sqlserver-backup.tar.gz -C /data
```

## Solução de Problemas

### Comandos de Limpeza
```bash
# Parar e remover containers, redes e volumes
docker compose down -v --remove-orphans

# Limpar sistema Docker (cuidado!)
docker system prune -f

# Remover tudo incluindo imagens (muito cuidado!)
docker compose down -v --remove-orphans --rmi all
docker system prune -af
```

## Ambiente de Desenvolvimento vs Produção

### Diferenças

**Desenvolvimento (`docker-compose.override.yml`):**
- Logs mais verbosos
- Configurações de CORS mais permissivas
- User Secrets montados
- Certificados HTTPS de desenvolvimento

**Produção (`docker-compose.yml`):**
- Configurações otimizadas para performance
- Logs estruturados
- Políticas de restart
- Health checks configurados

### Alternando entre ambientes
```bash
# Desenvolvimento
docker compose -f docker-compose.yml -f docker-compose.override.yml up -d

# Produção
docker compose up -d
```

## Monitoramento de Performance

### Métricas básicas
```bash
# Uso de CPU e memória
docker stats

# Logs de performance
docker compose logs | grep -E "(Performance|Timing|Duration)"
```

### Endpoints de Health Check
- BFF: http://localhost:5000/health
- Identity: http://localhost:5001/health
- Gestão Conteúdo: http://localhost:5002/health
- Gestão Alunos: http://localhost:5003/health
- Faturamento: http://localhost:5004/health

##  Segurança

### Recomendações para Produção

1. **Alterar senhas padrão** no arquivo `.env`
2. **Usar volumes externos** para dados críticos
3. **Configurar rede isolada** se necessário
4. **Implementar backup automatizado**
5. **Monitorar logs de segurança**

### Variáveis de ambiente sensíveis
Todas as configurações sensíveis estão no arquivo `.env`. **Nunca commite este arquivo em produção!**

## Suporte

Para problemas relacionados ao Docker Compose:
1. Verifique os logs: `make logs`
2. Verifique o status dos serviços: `make health`
3. Tente um reset completo: `make reset`
4. Abra uma issue no repositório do projeto

## Updates e Manutenção

### Atualizar imagens
```bash
# Pull das imagens mais recentes
docker compose pull

# Rebuild e restart
docker compose down
docker compose build
docker compose up -d
```

### Limpeza periódica
```bash
# Limpar recursos não utilizados
docker system prune -f

# Limpar tudo (cuidado em produção!)
docker compose down -v --remove-orphans --rmi all
docker system prune -af
```