# Bounded Contexts (BCs)

## Visão Geral
O sistema é dividido em quatro contextos delimitados principais, cada um responsável por um domínio de negócio específico. Este documento detalha as responsabilidades, modelos e interações de cada contexto.

## Contexto de Gestão de Conteúdo

### Responsabilidades
- Criação e gestão de cursos
- Organização de aulas
- Manipulação de material didático
- Gestão da estrutura do curso

### Modelos Principais
- Curso (Raiz de Agregação)
  - Propriedades: Id, Nome, Descrição, ConteúdoProgramático
  - Relacionamentos: Aulas
- Aula (Entidade)
  - Propriedades: Id, Título, Conteúdo, Material
  - Relacionamentos: Curso
- ConteúdoProgramático (Objeto de Valor)
  - Propriedades: Descrição, Objetivos

### Casos de Uso
1. Cadastro de Curso
   - Administrador cria novo curso
   - Sistema valida dados do curso
   - Curso é salvo e disponibilizado
2. Cadastro de Aula
   - Administrador adiciona aulas ao curso
   - Sistema valida dados da aula
   - Aula é associada ao curso

## Contexto de Gestão de Alunos

### Responsabilidades
- Matrícula de alunos
- Acompanhamento de progresso
- Geração de certificados
- Histórico de aprendizado

### Modelos Principais
- Aluno (Raiz de Agregação)
  - Propriedades: Id, Nome, Email
  - Relacionamentos: Matrículas, Certificados
- Matrícula (Entidade)
  - Propriedades: Id, Status, Progresso
  - Relacionamentos: Curso, Aluno
- Certificado (Entidade)
  - Propriedades: Id, DataEmissão, Curso
  - Relacionamentos: Aluno
- HistóricoAprendizado (Objeto de Valor)
  - Propriedades: Progresso, DataConclusão

### Casos de Uso
1. Matrícula do Aluno
   - Aluno seleciona curso
   - Sistema cria matrícula
   - Status definido como pendente de pagamento
2. Conclusão do Curso
   - Sistema acompanha progresso
   - Gera certificado
   - Atualiza status da matrícula

## Contexto de Faturamento

### Responsabilidades
- Processamento de pagamentos
- Geração de faturas
- Gestão de status de pagamento
- Relatórios financeiros

### Modelos Principais
- Pagamento (Raiz de Agregação)
  - Propriedades: Id, Valor, Status
  - Relacionamentos: Matrícula
- DadosCartão (Objeto de Valor)
  - Propriedades: Número, Validade, CVV
- StatusPagamento (Objeto de Valor)
  - Propriedades: Status, DataHora

### Casos de Uso
1. Processamento de Pagamento
   - Aluno fornece dados de pagamento
   - Sistema processa pagamento
   - Atualiza status da matrícula
2. Gestão de Status de Pagamento
   - Sistema acompanha status do pagamento
   - Notifica partes interessadas
   - Atualiza entidades relacionadas

## Contexto de Identidade

### Responsabilidades
- Autenticação de usuários
- Autorização
- Gestão de papéis
- Gestão de perfil de usuário

### Modelos Principais
- Usuário (Raiz de Agregação)
  - Propriedades: Id, NomeUsuário, Email
  - Relacionamentos: Papéis
- Papel (Entidade)
  - Propriedades: Id, Nome, Permissões
- PerfilUsuário (Objeto de Valor)
  - Propriedades: InformaçõesPessoais, Preferências

### Casos de Uso
1. Autenticação de Usuário
   - Usuário fornece credenciais
   - Sistema valida e emite token
2. Gestão de Papéis
   - Administrador atribui papéis
   - Sistema aplica permissões

## Mapeamento de Contextos

### Gestão de Conteúdo ↔ Gestão de Alunos
- Informações de disponibilidade de cursos
- Validação de matrícula
- Acompanhamento de progresso

### Gestão de Alunos ↔ Faturamento
- Atualizações de status de matrícula
- Requisitos de pagamento
- Gatilhos de geração de certificado

### Identidade ↔ Todos os Contextos
- Autenticação de usuário
- Controle de acesso baseado em papéis
- Aplicação de permissões

## Padrões de Integração
- Camada anti-corrupção para serviços externos