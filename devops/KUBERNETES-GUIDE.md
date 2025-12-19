# Peo Platform - Guia Completo Kubernetes

## 🎯 **Visão Geral**

Esta documentação contém todos os manifestos necessários para executar a plataforma Peo no Kubernetes, seguindo os mesmos padrões do Docker Compose.

## 📋 **Pré-requisitos**

- **Kubernetes cluster** (local: minikube, Docker Desktop, kind)
- **kubectl** configurado e conectado ao cluster
- **Docker** para build das imagens
- **Kustomize** (incluído no kubectl v1.14+)

## 🚀 **Deploy**

```bash
# Build imagens primeiro
docker build -t peo-identity-api:latest -f src/Peo.Identity.WebApi/Dockerfile .
docker build -t peo-gestaoconteudo-api:latest -f src/Peo.GestaoConteudo.WebApi/Dockerfile .
docker build -t peo-gestaoalunos-api:latest -f src/Peo.GestaoAlunos.WebApi/Dockerfile .
docker build -t peo-faturamento-api:latest -f src/Peo.Faturamento.WebApi/Dockerfile .
docker build -t peo-bff:latest -f src/Peo.Web.Bff/Dockerfile .
docker build -t peo-frontend:latest -f src/Peo.Web.Spa/Dockerfile .

# Deploy usando Kustomize
kubectl apply -k devops/k8s/
```

## 🌐 **URLs de Acesso**

| Serviço | URL | Tipo |
|---------|-----|------|
| **Frontend** | http://localhost:30000 | NodePort |
| **BFF API** | http://localhost:30001 | NodePort |
| **RabbitMQ UI** | http://localhost:30002 | NodePort |

## 🔐 **Credenciais**

### **Aplicação:**
- **Admin:** admin@admin.com / @dmin!

### **RabbitMQ:** 
- **Usuário:** guest / guest (configurável em secrets.yaml)

### **SQL Server:**
- **Usuário:** sa / MyStr0ngP@ssw0rd123 (configurável em secrets.yaml)

## ⚙️ **Configuração**

### **Alterando Senhas (Produção)**
```bash
# 1. Edite devops/k8s/base/secrets.yaml
# 2. Gere novos valores base64:
echo -n "MinhaNovaSenga123!" | base64
echo -n "MeuNovoJWTKey" | base64

# 3. Substitua os valores no arquivo
# 4. Redeploy:
kubectl apply -k devops/k8s/
```

### **Recursos por Pod**
| Serviço | CPU Request | CPU Limit | Memory Request | Memory Limit |
|---------|-------------|-----------|----------------|--------------|
| SQL Server | 500m | 2000m | 2Gi | 4Gi |
| RabbitMQ | 250m | 500m | 512Mi | 1Gi |
| APIs | 250m | 500m | 256Mi | 512Mi |
| BFF | 250m | 500m | 256Mi | 512Mi |
| Frontend | 100m | 200m | 128Mi | 256Mi |

## 📊 **Monitoramento**

### **Status dos Pods**
```bash
# Ver todos os pods
kubectl get pods -n peo-platform

# Ver pods com mais detalhes
kubectl get pods -n peo-platform -o wide

# Ver logs de um pod
kubectl logs -f deployment/peo-identity-api -n peo-platform
```

### **Health Checks**
```bash
# Verificar saúde dos serviços
kubectl get pods -n peo-platform | grep Running

# Port forward para testar APIs localmente
kubectl port-forward svc/peo-bff 5000:80 -n peo-platform
curl http://localhost:5000/health
```

### **Métricas de Recursos**
```bash
# Uso de recursos por pod
kubectl top pods -n peo-platform

# Uso de recursos por node
kubectl top nodes
```

## 🔄 **Operações Comuns**

### **Restart de Serviços**
```bash
# Restart de um deployment específico
kubectl rollout restart deployment/peo-identity-api -n peo-platform

# Restart de todos os deployments
kubectl rollout restart deployment -n peo-platform
```

### **Scaling**
```bash
# Scale horizontal de um serviço
kubectl scale deployment peo-identity-api --replicas=3 -n peo-platform

# Scale de múltiplos serviços
kubectl scale deployment peo-bff peo-frontend --replicas=3 -n peo-platform
```

### **Updates de Imagem**
```bash
# Update de imagem específica
kubectl set image deployment/peo-identity-api identity-api=peo-identity-api:v2.0.0 -n peo-platform

# Verificar status do rollout
kubectl rollout status deployment/peo-identity-api -n peo-platform
```

## 🐛 **Troubleshooting**

### **Pod não inicia**
```bash
# Ver eventos do pod
kubectl describe pod <pod-name> -n peo-platform

# Ver logs detalhados
kubectl logs <pod-name> -n peo-platform --previous
```

### **Problemas de conectividade**
```bash
# Testar DNS interno
kubectl exec -it <pod-name> -n peo-platform -- nslookup peo-sqlserver

# Testar conectividade entre serviços
kubectl exec -it <pod-name> -n peo-platform -- wget -O- http://peo-identity-api/health
```

### **Problemas de banco de dados**
```bash
# Conectar ao SQL Server
kubectl exec -it deployment/peo-sqlserver -n peo-platform -- /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'MyStr0ngP@ssw0rd123' -C

# Verificar logs do SQL Server
kubectl logs deployment/peo-sqlserver -n peo-platform
```

## 🧹 **Limpeza**

```bash
# Remover namespace (remove todos os recursos)
kubectl delete namespace peo-platform

# Remover apenas recursos específicos
kubectl delete -k devops/k8s/

# Limpar imagens Docker locais
docker rmi $(docker images 'peo-*' -q)
```

## 📈 **Produção**

### **Considerações para Produção**

1. **🏷️ Registry de Imagens:**
   - Publique imagens em um registry (Docker Hub, ACR, ECR)
   - Atualize `kustomization.yaml` com as URLs corretas

2. **🔒 Secrets:**
   - Use ferramentas como Sealed Secrets ou External Secrets
   - Nunca commite secrets em plain text

3. **💾 Persistent Volumes:**
   - Configure StorageClass adequada
   - Implemente backup automatizado dos PVs

4. **🌐 Ingress:**
   - Configure um Ingress Controller
   - Use certificados SSL/TLS

5. **📊 Observabilidade:**
   - Implemente logging centralizado (ELK, Loki)
   - Configure monitoramento (Prometheus + Grafana)
   - Adicione tracing distribuído (Jaeger, Zipkin)

### **Exemplo de configuração para produção:**
```yaml
# ingress.yaml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: peo-ingress
  namespace: peo-platform
spec:
  rules:
  - host: peo.yourdomain.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: peo-frontend
            port:
              number: 80
      - path: /api
        pathType: Prefix
        backend:
          service:
            name: peo-bff
            port:
              number: 80
```

## 🔄 **CI/CD Integration**

Para integrar com pipelines CI/CD, use os manifestos como base e automatize o deploy:

```yaml
# Exemplo GitHub Actions
- name: Deploy to K8s
  run: |
    kubectl apply -k devops/k8s/
    kubectl rollout status deployment -n peo-platform
```
