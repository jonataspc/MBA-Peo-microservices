# Telas Necessárias para o Front-end da Plataforma Educacional

## 📋 Telas Comuns (Ambos os Perfis)

### 1. **Tela de Login**
- Campos: email e senha
- Botão de login
- Link para cadastro
- Validação de credenciais
- Redirecionamento baseado no tipo de usuário (Admin/Aluno)

### 2. **Tela de Cadastro de Usuário**
- Seleção do tipo de usuário (Aluno/Administrador)
- Campos: nome, email, senha, confirmação de senha
- Validações de formulário
- Integração com Auth API

---

## 👨‍🎓 Telas para ALUNO

### 3. **Dashboard do Aluno**
- Visão geral dos cursos matriculados
- Progresso atual dos cursos
- Certificados disponíveis
- Acesso rápido às aulas

### 4. **Catálogo de Cursos**
- Lista de cursos disponíveis
- Detalhes dos cursos (nome, conteúdo programático)
- Botão de matrícula
- Filtros e busca

### 5. **Detalhes do Curso**
- Informações completas do curso
- Conteúdo programático
- Botão de matrícula
- Preview das aulas (se aplicável)

### 6. **Tela de Matrícula**
- Confirmação da matrícula
- Informações do curso selecionado
- Redirecionamento para pagamento

### 7. **Tela de Pagamento**
- Formulário de pagamento
- Informações do curso
- Status do pagamento
- Confirmação de transação

### 8. **Meus Cursos**
- Lista de cursos matriculados
- Status de cada curso (ativo, concluído, pendente)
- Progresso visual
- Acesso às aulas

### 9. **Tela da Aula**
- Conteúdo da aula
- Controles de navegação (anterior/próxima)
- Marcação de progresso
- Material de apoio

### 10. **Histórico do Aluno**
- Cursos concluídos
- Certificados obtidos
- Histórico de pagamentos
- Progresso geral

### 11. **Meus Certificados**
- Lista de certificados disponíveis
- Download de certificados
- Visualização/preview dos certificados

### 12. **Perfil do Aluno**
- Dados pessoais
- Edição de informações
- Alteração de senha

---

## 👨‍💼 Telas para ADMINISTRADOR

### 13. **Dashboard Administrativo**
- Estatísticas gerais (total de alunos, cursos, receita)
- Gráficos de desempenho
- Resumo de atividades recentes
- Acesso rápido às principais funcões

### 14. **Gerenciamento de Cursos**
- Lista de todos os cursos
- Botões para criar, editar, excluir
- Status dos cursos
- Filtros e busca

### 15. **Cadastro/Edição de Curso**
- Formulário para criar/editar curso
- Campos: nome, descrição, conteúdo programático
- Validações
- Preview do curso

### 16. **Gerenciamento de Aulas**
- Lista de aulas por curso
- Criação de novas aulas
- Edição de aulas existentes
- Organização/ordenação das aulas

### 17. **Cadastro/Edição de Aula**
- Formulário para criar/editar aula
- Upload de conteúdo
- Definição de ordem/sequência
- Material de apoio

### 18. **Gerenciamento de Alunos**
- Lista de todos os alunos
- Informações de matrícula
- Progresso individual
- Histórico de pagamentos

### 19. **Detalhes do Aluno (Admin)**
- Informações completas do aluno
- Cursos matriculados
- Progresso detalhado
- Histórico de transações

### 20. **Gerenciamento de Pagamentos**
- Lista de todas as transações
- Status de pagamentos
- Filtros por data, status, aluno
- Detalhes das transações

### 21. **Relatórios Financeiros**
- Receita por período
- Gráficos de vendas
- Análise de pagamentos
- Exportação de dados

### 22. **Configurações do Sistema**
- Configurações gerais da plataforma
- Parâmetros do sistema
- Configurações de pagamento

---

## 🔧 Telas de Apoio/Utilitárias

### 23. **Tela de Loading/Carregamento**
- Para transações de pagamento
- Para processamento de dados
- Estados de loading gerais

### 24. **Telas de Erro**
- 404 - Página não encontrada
- 500 - Erro interno
- Erro de conectividade
- Erro de autenticação

### 25. **Tela de Confirmação**
- Confirmação de ações críticas
- Confirmação de exclusão
- Confirmação de pagamento

### 26. **Notificações/Feedback**
- Sistema de notificações
- Mensagens de sucesso/erro
- Alertas importantes

---

## 📱 Considerações Técnicas

### Responsividade
- Todas as telas devem ser responsivas
- Adaptação para mobile, tablet e desktop

### Navegação
- Menu lateral ou superior baseado no perfil
- Breadcrumbs para navegação contextual
- Navegação intuitiva entre as telas

### Estados da Aplicação
- Estados de loading
- Estados vazios (sem dados)
- Estados de erro
- Estados de sucesso

### Integração com APIs
- Consumo do BFF ou APIs diretas
- Gerenciamento de token JWT
- Tratamento de erros de API

**Total: Aproximadamente 26 telas principais + componentes de apoio**