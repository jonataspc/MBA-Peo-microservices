# Peo Platform Kubernetes Manifests

Este diretório contém todos os manifestos Kubernetes necessários para executar a plataforma Peo usando o padrão **"Database per Service"** com instâncias SQL Server dedicadas para cada microserviço.

## 🏗️ **Arquitetura Implementada**

### **Database per Service Pattern:**
- **4 Instâncias SQL Server Separadas**: Uma para cada microserviço
- **Isolamento Completo**: Dados completamente isolados entre serviços  
- **Escalabilidade Independente**: Cada banco pode ser escalado conforme necessidade
- **Isolamento de Falhas**: Problemas em um banco não afetam outros

## 📁 Estrutura

```
devops/
├── k8s/
│   ├── base/                    # Configurações base
│   │   ├── namespace.yaml
│   │   ├── configmap.yaml      # Inclui configurações de DB separadas
│   │   └── secrets.yaml
│   ├── infrastructure/          # Infraestrutura (4 DBs + MQ)
│   │   ├── sqlserver.yaml      # 4 instâncias SQL Server dedicadas
│   │   └── rabbitmq.yaml
│   ├── microservices/          # APIs com conexões específicas
│   │   ├── identity-api.yaml         # → peo-identity-sqlserver
│   │   ├── gestaoconteudo-api.yaml   # → peo-gestaoconteudo-sqlserver
│   │   ├── gestaoalunos-api.yaml     # → peo-gestaoalunos-sqlserver
│   │   └── faturamento-api.yaml      # → peo-faturamento-sqlserver
│   ├── frontend/               # BFF e SPA
│   │   ├── bff.yaml
│   │   └── spa.yaml
│   └── kustomization.yaml      # Kustomize config 
```

## 🚀 Deploy Rápido

```bash
# Deploy completo
kubectl apply -k devops/k8s/
```

## 🗄️ **Instâncias de Banco de Dados**

| Serviço | Host Interno | Port Externo | Database | Recursos |
|---------|-------------|--------------|----------|----------|
| **Identity** | peo-identity-sqlserver | 31433 | identity-db | 2-4Gi RAM |
| **Faturamento** | peo-faturamento-sqlserver | 31434 | faturamento-db | 2-4Gi RAM |
| **Gestão Alunos** | peo-gestaoalunos-sqlserver | 31435 | gestao-alunos-db | 2-4Gi RAM |
| **Gestão Conteúdo** | peo-gestaoconteudo-sqlserver | 31436 | gestao-conteudo-db | 2-4Gi RAM |

## 🔧 Configurações

### Variáveis de Ambiente
Configure as variáveis no arquivo `devops/k8s/base/secrets.yaml` (base64 encoded):

```bash
echo -n "MyStr0ngP@ssw0rd123" | base64  # SA_PASSWORD (mesma para todas as instâncias)
echo -n "guest" | base64               # RABBITMQ_USER
echo -n "your-jwt-key" | base64        # JWT_KEY
```

### URLs dos Serviços
- **Frontend:** http://localhost:30000
- **BFF:** http://localhost:30001  
- **RabbitMQ UI:** http://localhost:30002
- **Identity DB:** localhost:31433 (via NodePort)
- **Faturamento DB:** localhost:31434 (via NodePort)
- **Gestão Alunos DB:** localhost:31435 (via NodePort)
- **Gestão Conteúdo DB:** localhost:31436 (via NodePort)

## 📝 Comandos Úteis

```bash
# Ver status dos pods (incluindo 4 instâncias SQL Server)
kubectl get pods -n peo-platform

# Ver logs das APIs
kubectl logs -f deployment/peo-identity-api -n peo-platform
kubectl logs -f deployment/peo-faturamento-api -n peo-platform
kubectl logs -f deployment/peo-gestaoalunos-api -n peo-platform
kubectl logs -f deployment/peo-gestaoconteudo-api -n peo-platform

# Ver logs das instâncias de banco
kubectl logs -f deployment/peo-identity-sqlserver -n peo-platform
kubectl logs -f deployment/peo-faturamento-sqlserver -n peo-platform
kubectl logs -f deployment/peo-gestaoalunos-sqlserver -n peo-platform
kubectl logs -f deployment/peo-gestaoconteudo-sqlserver -n peo-platform

# Port-forward para acesso externo às bases de dados
kubectl port-forward svc/peo-identity-sqlserver-external 1433:1433 -n peo-platform
kubectl port-forward svc/peo-faturamento-sqlserver-external 1434:1433 -n peo-platform
kubectl port-forward svc/peo-gestaoalunos-sqlserver-external 1435:1433 -n peo-platform
kubectl port-forward svc/peo-gestaoconteudo-sqlserver-external 1436:1433 -n peo-platform
```

## ⚠️ **Requisitos Importantes**

### **Recursos Mínimos do Cluster:**
- **CPU:** ~12 cores (4 instâncias SQL Server × 2 cores cada + APIs)
- **Memória:** ~24GB RAM (4 instâncias × 4Gi cada + APIs)
- **Storage:** ~40GB (4 PVCs × 10GB cada)

### **Strings de Conexão para Ferramentas Externas:**
```bash
# Identity Database
Server=localhost,31433;Database=identity-db;User Id=sa;Password=MyStr0ngP@ssw0rd123;TrustServerCertificate=true;

# Faturamento Database  
Server=localhost,31434;Database=faturamento-db;User Id=sa;Password=MyStr0ngP@ssw0rd123;TrustServerCertificate=true;

# Gestão Alunos Database
Server=localhost,31435;Database=gestao-alunos-db;User Id=sa;Password=MyStr0ngP@ssw0rd123;TrustServerCertificate=true;

# Gestão Conteúdo Database
Server=localhost,31436;Database=gestao-conteudo-db;User Id=sa;Password=MyStr0ngP@ssw0rd123;TrustServerCertificate=true;
```