# Peo Platform Kubernetes Manifests

Este diretório contém todos os manifestos Kubernetes necessários para executar a plataforma Peo.

## 📁 Estrutura

```
devops/
├── k8s/
│   ├── base/                    # Configurações base
│   │   ├── namespace.yaml
│   │   ├── configmap.yaml
│   │   └── secrets.yaml
│   ├── infrastructure/          # Infraestrutura (DB, MQ)
│   │   ├── sqlserver.yaml
│   │   └── rabbitmq.yaml
│   ├── microservices/          # APIs
│   │   ├── identity-api.yaml
│   │   ├── gestaoconteudo-api.yaml
│   │   ├── gestaoalunos-api.yaml
│   │   └── faturamento-api.yaml
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

## 🔧 Configurações

### Variáveis de Ambiente
Configure as variáveis no arquivo `devops/k8s/base/secrets.yaml` (base64 encoded):

```bash
echo -n "MyStr0ngP@ssw0rd123" | base64  # SA_PASSWORD
echo -n "guest" | base64               # RABBITMQ_USER
echo -n "your-jwt-key" | base64        # JWT_KEY
```

### URLs dos Serviços
- Frontend: http://localhost:30000
- BFF: http://localhost:30001
- RabbitMQ UI: http://localhost:30002
- APIs: Internas ao cluster

## 📝 Comandos Úteis

```bash
# Ver status dos pods
kubectl get pods -n peo-platform

# Ver logs
kubectl logs -f deployment/peo-identity-api -n peo-platform

# Port-forward para acesso local
kubectl port-forward svc/peo-frontend 3000:80 -n peo-platform
```