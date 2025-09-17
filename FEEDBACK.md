## Funcionalidade 30%

Avalie se o projeto atende a todos os requisitos funcionais definidos.
* Será revisado na avalição final.


## Qualidade do Código 20%

Considere clareza, organização e uso de padrões de codificação.

* Pontos positivos:
  - Uso de logger para rastreamento de operações.

### Aspire
- Como adotaram o Aspire:
  - não existe motive para usar SQLLite, visto que o Aspire irá executar SQL Server mesmo em ambiente de desenvolvimento. Não é requerimento, mas considerem abandonar o SQLLite.
	- *Resposta 1*: Havíamos conversado sobre isso com o Eduardo Pires e ele nos indicou continuar uilizando SQLite, devido ser um requisito. 
  - não vi as configurações do SQL Servier no Aspire Host. Ou adote Aspire para tudo, ou não usem Aspire. Essa mescla "híbrida" não faz sentido.
	- *Resposta 2*: este ponto também foi alinhado com o Eduardo Pires, sobre usar Aspire com as aplicações consumindo base em SQLite.
  - O mesmo para congigurações do `appSettings`, o Aspire faz a distribuição de configuração, não é necessário ter o `appSettings` para cada projeto. As configurações podem ser centralizadas no projeto de host do Aspire.
	- *Resposta 3*: entendemos que por serem projetos separados e independentes, cada um deles deve ser seu próprio arquivo de configuração, para evitar acoplamentos e cada um conter suas configurações exclusivas.

### Application
- A pasta `Endpoints` deveria estar em `WebApi`, não em `Application`. Application deve ser agonóstico ao meio da apicação, se é web, console, desktop, etc. Application sequer deve ter referencias `Microsoft.AspNetCore.*`.
	- *Resposta 4*: será ajustado.
- As rotas estão confusas nos endpoints. Em `EndpointsAula`, a rota é `/matricula/xxx`, e em `Estudante` a rota é `/certificados`. Revejam as rotas para que sejam um serviço RESTful consistente.
	- *Resposta 5*: a rota de certificados (/v1/estudante/certificados) retorna todos os certificados do aluno, por isso que não está dentro da hierarquia da matricula. 

### Domain
- Se o context é "Gestão de Alunos", o nome da entidade deveria ser "Aluno", e não "Estudante".
	- *Resposta 6*: será ajustado.
- Em `Estudante`, propriedades como coleções devem ser expostas como `IReadOnlyCollection<T>`, para evitar modificações externas.
	- *Resposta 7*: será ajustado.
- As entidades de domínio ainda não possuem validações ou regras de negócio. Considere adicionar validações básicas, como garantir que o nome do estudante não seja nulo ou vazio.
	- *Resposta 8*: será ajustado.
- Não é mais necessário ter um construtor sem parâmetros nas entidades de domínio para suporter Entity Framework. Versões recentes do EF suportam construtores parametrizados.
	- *Resposta 9*: será avaliado (remoção do construtor sem parâmetros ou alterar para ```protected``` caso seja necessário)
- Evitem agrupar interfaces em uma pasta `Interfaces`. Prefiram organizar por contexto ou funcionalidade. Em "GestãoAlunos", pode renomerar `Interfaces` para `Repositories`, e em "Faturamento", pode renomear para `Services` e hospedar `IBroker...` e `IPagamaneto...`.
	- *Resposta 10*: será ajustado.
- O nome `CartaoCreditoData` é bem confuso, tive que navegar por várias propriedades para tentar entender do que se trata. Considere renomear para `DadosDoCartaoCredito` ou algo similar.
	- *Resposta 11*: será ajustado.
- Como `ValueObjets` são imutáveis, considere usar `record` ao invés de `class`;
	- *Resposta 12*: será ajustado.

### Data
- Como adotaram o Aspire, ideal é que a connection string seja obtida via configuração do Aspire, e não via `appSettings.json`. Também não é necessario testar por `IsDevelopment()`, visto que o Aspire pode e deve gerenciar isso.
	- *Resposta 13*: verificar resposta 1.
- Não é necessário: O métodos `GetByIdAsync(Guid estudanteId)` possui nome redundante. Prefiram `GetAsync(Guid id)`, que já se entende que é por ID.
	- *Resposta 14*: entendemos que GetById deixa a intenção mais clara, sem precisar ler os parâmetros de entrada.
- Renomear `GetAulasConcluidasCountAsync()` para `CountAulasConcluidasAsync()`.
	- *Resposta 15*: será ajustado.
- Se `Certificado` é uma entidade, então deve ter seu próprio repositório. 
	- *Resposta 16*: `Certificado` não é uma raiz de agregação. O certificado é gerado a partir do estudante, por isso que está dentro do repositório do estudante. 

### Infra
- Em `GenericRepository`, não usem métodos públicos mas sim apenas `protected`, pois dá acesso irrestrito a todas as operações de banco. Prefiram métodos protegidos ou internos, e exponham apenas o necessário via repositórios específicos.
	- *Resposta 17*: A classe `GenericRepository` é utilizada diretamente (não é abstrata). Por este motivo os métodos não podem ser ajustados para `protected`.

### Geral
- Evitem siglas ou abreviações em nomes de classes e métodos. Por exemplo, a namespace `DiConfig` poderia ser renomeada para `DependencyInjectionConfiguration` para maior clareza.
	- *Resposta 18*: será ajustado.
- Não faz sentido ver os comandos apenas transportando "DTOs". Os comandos são como requests, eles são o contrato da operação e devem trazer a responsabilidade de validar o comando para eles. Da mesma forma que se espera um `response` de um `request`, se espera um `result` de um `command`. No caso do projeto de vocês, a comunicação entre uma interface (WebAPI, Console, etc) e o Application Layer é via comandos. Recomendo mover as DTOs para as WebAPIs, e fazer com que os comandos representem a intenção da operação, incluindo validações. E o mapeamento de DTO para Command ser feito na camada de interface.
	- *Resposta 19*: vamos precisar de mais detalhes acerca deste item.
- Quando se usa construtores primários, não é necessário o use de campos privados.
	- *Resposta 20*: onde que foi localizado este ponto? Qual classe?
- Prefiram user `var` ao invés de tipos explícitos.
	- *Resposta 21*: OK. 
- Remover `usings` não utilizados.
	- *Resposta 22*: será ajustado.
- Faça uso de sobrecarga de métodos ao invés de criar métodos com nomes diferentes para a mesma operação. 
	- *Resposta 23*: onde que foi localizado este ponto? Qual classe?

## Eficiência e Desempenho 20%

Avalie o desempenho e a eficiência das soluções implementadas.
* Será revisado na avalição final.
  - Propagem o `CancellationToken` em todos os métodos assíncronos.
	- *Resposta 24*: será ajustado.

## Inovação e Diferenciais 10%

Considere a criatividade e inovação na solução proposta.
* Pontos positivos:
  - Muito bacana a iniciativa de usar Aspire.

* Comentários e recomendações:
  - Com o uso de Unity of Work e um reponse handler como middleware, você criar uma forma mais sólida de gerir o estado da aplicação em apenas persistir as mudanças se tudo ocorrer bem, ou descartar as mudanças em caso de falha. Isso evita ter que chamar `SaveChangesAsync` em cada handler, e também evita ter que lidar com transações manualmente. Não é imposição, mas mais um desafio: apenas persistir se o response for `HTTP 2XX`.
	- *Resposta 25*: OK. 

## Documentação e Organização 10%

Verifique a qualidade e completude da documentação, incluindo README.md.
* Pontos positivos:
  - O README.md fornece uma visão geral clara do projeto e suas funcionalidades.
  - A documentação inclui instruções básicas para configuração e execução do projeto.
  - Boa organização dos projetos dentro da solução.
  - Excelente organização dos arquivos e pastas dentro de cada projeto.

* Comentários e recomendações:
  - Antes de submeterem para avaliação final, atualizem o README.md e repitam o processo de execução do projeto, para garantir que todas as instruções estão corretas e atualizadas.
	- *Resposta 26*: será revisado. 
  - A organização dos arquivos está clara, facilitando a navegação pelo projeto.
  - Eu esperava que a navegação entre Solution Explorer e Windows Explorer ser semelhante. Ter os projetos também separadas por pastas per contextos. Não é obrigatório, mas facilitaria a navegação.
  - A pasta de soluções "BCs" é desncessária, ou talvez ser renomada para "Contextos" ou algo similar.
  - Em `GestaoAluno/Application/Commands`, existe uma hiper granularidade de pastas. Não é um problema agora, mas já vejo sinal de dificuldades em escolhar em qual subspasta hospedar novos comandos. Considerem remover essas subpastas e manter apenas a pasta `Commands`.

## Resolução de Feedbacks 10%

Avalie a resolução dos problemas apontados na primeira avaliação de frontend
* Será revisado na avalição final.

