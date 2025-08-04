# Guths.Shared

Uma biblioteca compartilhada para aplicações .NET que fornece funcionalidades comuns e reutilizáveis para projetos web, APIs e sistemas distribuídos.

## 🚀 Funcionalidades

- **Sistema de Result Pattern** - Padronização de retornos com validações
- **Autenticação JWT** - Configuração e utilitários para autenticação
- **Paginação** - DTOs e helpers para paginação de dados
- **MongoDB Integration** - Generic Repository pattern e configurações
- **Storage Services** - Integração com Amazon S3
- **Web Utilities** - Controllers base, Endpoints helper, formatters e middlewares
- **Localização** - Suporte multi-idioma
- **Logging** e Observabilidade - Logger customizado, tracing e métricas
- **Resiliência HTTP** - Políticas de retry e circuit breaker
- **AWS Parameter Store** - Integração para configurações seguras

## 🔧 Instalação

```bash
dotnet add package Guths.Shared
```

## 📋 Configuração Básica

### 1. Program.cs

```csharp
using Guths.Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configuração básica (carrega automaticamente do appsettings.json)
builder.Services.AddSharedConfiguration();

var app = builder.Build();

// Configurar pipeline da aplicação
app.AddSharedConfiguration("ScalarTitle");

app.Run();
```

### 2. Configuração com Políticas de Autorização Customizadas

```csharp
using Guths.Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configuração com políticas customizadas
var sharedConfigurationOptions = new SharedConfigurationOptions
{
    ConfigureAuthorization = options =>
    {
        options.AddPolicy("SystemScope", policy =>
        {
            policy.RequireClaim("role", "system");
            policy.RequireClaim("scope", "send_email");
        });
        
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    }
};

builder.Services.AddSharedConfiguration(sharedConfigurationOptions);

var app = builder.Build();

app.AddSharedConfiguration("ScalarTitle");

app.Run();
```

### 3. appsettings.json ou AWS Parameter Store

#### Configuração Mínima Obrigatória

```json
{
  "SharedConfiguration": {
    "UseAwsParameterStore": false
  },
  "AmazonParameterStorePaths": [
    "/NomeDoProjeto",
    "/Commons",
    "/etc"
  ]
}
```

> ⚠️ **Importante**: As seções `SharedConfiguration` e `AmazonParameterStorePaths` são **obrigatórias** no `appsettings.json`. A primeira controla se o AWS Parameter Store será usado, e a segunda define os caminhos a serem buscados (necessária apenas se for usar AWS para evitar exceptions).

#### Configuração Completa

```json
{
  "SharedConfiguration": {
    "UseAwsParameterStore": true,
    "UseDefaultHttpResilience": true,
    "UseGlobalExceptionHandler": true,
    "UseCache": true,
    "UseAuth": true,
    "UseCors": true,
    "UseControllers": true,
    "UseLocalization": true,
    "UseScalar": true,
    "LoggingConfiguration": {
      "UseCustomLogger": true,
      "UseTracing": true,
      "UseMetrics": true
    }
  },
  "AmazonParameterStorePaths": [
    "/NomeDoProjeto",
    "/Commons",
    "/etc"
  ],
  "AuthSettings": {
    "Issuer": "https://meudominio.com",
    "Audience": "minha-api",
    "SecretKey": "sua-chave-secreta-aqui",
    "AccessTokenExpirationInMinutes": 30,
    "RefreshTokenExpirationInDays": 7,
    "CookieName": "AuthToken"
  },
  "DatabaseConnection": {
    "MongoDbConnectionString": "mongodb://localhost:27017",
    "MongoDbDatabaseName": "MeuDatabase"
  },
  "RedisConnection": {
    "Host": "localhost:6379",
    "User": "default",
    "Password": "sua-senha-redis"
  },
  "StorageOptions": {
    "BucketName": "meu-bucket"
  },
  "OpenTelemetry": {
    "Provider": "Grafana",
    "Grafana": {
      "ServiceName": "minha-api",
      "ServiceNamespace": "meu-namespace",
      "ApiKey": "sua-api-key-grafana",
      "TracesEndpoint": "https://traces-prod-us-central1.grafana.net/tempo",
      "MetricsEndpoint": "https://prometheus-us-central1.grafana.net/api/prom",
      "LogsEndpoint": "https://logs-prod-us-central1.grafana.net/loki"
    }
  }
}
```
> 🔒 **Recomendação de Segurança**: Dados sensíveis como senhas, chaves de API e tokens **não devem** ser armazenados no `appsettings.json` em produção ou em projetos abertos. Use AWS Parameter Store/Secrets Manager, Key Vault ou outras soluções seguras de gerenciamento de segredos.

### 4. Configurações via AWS Parameter Store

Quando `UseAwsParameterStore` está habilitado, as configurações podem ser armazenadas no AWS Systems Manager Parameter Store seguindo os padrões:

#### Autenticação
```
/{NomeDoProjeto}/AuthSettings/SecretKey
/{NomeDoProjeto}/AuthSettings/RefreshTokenExpirationInDays
/{NomeDoProjeto}/AuthSettings/Issuer
/{NomeDoProjeto}/AuthSettings/AccessTokenExpirationInMinutes
/{NomeDoProjeto}/AuthSettings/CookieName
/{NomeDoProjeto}/AuthSettings/Audience
```

#### Banco de Dados
```
/{NomeDoProjeto}/DatabaseConnection/MongoDbConnectionString
/{NomeDoProjeto}/DatabaseConnection/MongoDbDatabaseName
```

#### Cache Redis
```
/{NomeDoProjeto}/RedisConnection/Password
/{NomeDoProjeto}/RedisConnection/User
/{NomeDoProjeto}/RedisConnection/Host
```

#### Storage
```
/{NomeDoProjeto}/StorageOptions/BucketName
```

#### OpenTelemetry
```
/{NomeDoProjeto}/OpenTelemetry/Provider
/{NomeDoProjeto}/OpenTelemetry/Grafana/ApiKey
/{NomeDoProjeto}/OpenTelemetry/Grafana/TracesEndpoint
/{NomeDoProjeto}/OpenTelemetry/Grafana/ServiceNamespace
/{NomeDoProjeto}/OpenTelemetry/Grafana/MetricsEndpoint
/{NomeDoProjeto}/OpenTelemetry/Grafana/LogsEndpoint
/{NomeDoProjeto}/OpenTelemetry/Grafana/ServiceName
```

#### Configurações Compartilhadas
```
/Commons/DatabaseConnection/MongoDbConnectionString
/Commons/RedisConnection/Host
/etc/GlobalSettings/Environment
```

## ⚙️ Opções de Configuração

### SharedConfiguration

OBS: O valor padrão de todos os parâmetros abaixo é `false`.
```text
UseAwsParameterStore        -> Carrega configurações do AWS Parameter Store
UseDefaultHttpResilience    -> Habilita políticas padrão de resiliência HTTP
UseGlobalExceptionHandler   -> Middleware global de tratamento de exceções
UseCache                    -> Serviços de cache distribuído
UseAuth                     -> Autenticação JWT
UseCors                     -> Configuração de CORS
UseControllers              -> Registra controllers automaticamente
UseLocalization             -> Suporte a localização com StringLocalizer
UseScalar                   -> Documentação de API com Scalar e OpenAPI
LoggingConfiguration        -> Configurações de logging e observabilidade (Se encontrado a sessão LoggingConfiguration, pelo menos o logging será habilitado automaticamente)
```

### LoggingConfiguration

OBS: O valor padrão de todos os parâmetros abaixo é `false`.
```text
UseCustomLogger -> Logger customizado
UseTracing      -> Distributed tracing
UseMetrics      -> Métricas da aplicação
```

## 🛠️ Exemplos de Uso

### Result Pattern

```csharp
public class UserController : MyControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return CustomGetResponse(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var result = await _userService.CreateUserAsync(request);
        return CustomPostResponse(result);
    }
}
```

### Paginação

```csharp
public async Task<Result<PaginationResult<User>>> GetUsersAsync(PaginationInput pagination)
{
    var users = await _repository.GetPagedAsync(pagination);
    var totalCount = await _repository.CountAsync();
    
    return Result<PaginationResult<User>>.Success(new PaginationResult<User>
    {
        Items = users,
        Pagination = new PaginationData
        {
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalRecords = totalCount
        }
    });
}
```

### MongoDB Repository

```csharp
public class UserRepository : MongoDbRepository<User>
{
    public UserRepository(IMongoClientManager clientManager) 
        : base(clientManager, "users")
    {
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Collection
            .Find(x => x.Email == email)
            .FirstOrDefaultAsync();
    }
}
```

### Storage Service

```csharp
public class DocumentService
{
    private readonly IStorageService _storageService;
    
    public DocumentService(IStorageService storageService)
    {
        _storageService = storageService;
    }
    
    public async Task<string> UploadDocumentAsync(Stream fileStream, string fileName)
    {
        return await _storageService.UploadFileAsync(fileStream, fileName);
    }
}
```

## 🌐 Localização

### Configuração

```json
{
  "SharedConfiguration": {
    "UseLocalization": true
  }
}
```

```csharp
builder.Services.AddSharedConfiguration();
```

### Uso

```csharp
public class UserService
{
    private readonly IStringLocalizer<Global> _localizer;
    
    public UserService(IStringLocalizer<Global> localizer)
    {
        _localizer = localizer;
    }
    
    public Result<User> ValidateUser(User user)
    {
        if (string.IsNullOrEmpty(user.Email))
        {
            return Result<User>.Failure(_localizer["EmailRequired"]);
        }
        
        return Result<User>.Success(user);
    }
}
```

## 🔐 Autenticação

### Configuração Automática

```json
{
  "SharedConfiguration": {
    "UseAuth": true
  }
}
```

```csharp
var sharedConfigurationOptions = new SharedConfigurationOptions
{
    ConfigureAuthorization = options =>
    {
        options.AddPolicy("SystemScope", policy =>
        {
            policy.RequireClaim("role", "system");
            policy.RequireClaim("scope", "send_email");
        });
        
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    }
};

builder.Services.AddSharedConfiguration(sharedConfigurationOptions);
```

### Acesso ao Token

```csharp
public class MyService
{
    private readonly AuthTokenAccessor _tokenAccessor;
    
    public MyService(AuthTokenAccessor tokenAccessor)
    {
        _tokenAccessor = tokenAccessor;
    }
    
    public async Task<string> GetUserInfoAsync()
    {
        var token = _tokenAccessor.AccessToken;
        // Usar token para chamadas externas
    }
}
```

## 📊 Observabilidade

### Configuração

```json
{
  "SharedConfiguration": {
    "LoggingConfiguration": {
      "UseCustomLogger": true,
      "UseTracing": true,
      "UseMetrics": true
    }
  }
}
```

### Recursos Inclusos (OpenTelemetry)

A biblioteca utiliza **OpenTelemetry** como padrão para observabilidade, proporcionando flexibilidade total para escolher seus exporters e backends:

- **Logging Estruturado** - Logs padronizados e estruturados
- **Distributed Tracing** - Rastreamento de requisições entre serviços
- **Métricas** - Coleta automática de métricas da aplicação
- **Health Checks** - Endpoints de saúde da aplicação

## 🔄 Resiliência HTTP

### Configuração

```json
{
  "SharedConfiguration": {
    "UseDefaultHttpResilience": true
  }
}
```

### Recursos

- **Retry Policies** - Tentativas automáticas em falhas temporárias
- **Circuit Breaker** - Proteção contra cascata de falhas
- **Timeout** - Controle de tempo limite de requisições
- **Bulkhead** - Isolamento de recursos

## 📚 Recursos Automáticos

### Middleware de Usuário Autenticado
- Ativo quando `UseAuth = true`
- Popula informações do usuário no contexto automaticamente

### Exception Handler Global
- Ativo quando `UseGlobalExceptionHandler = true`
- Trata todas as exceções de forma padronizada
- Habilita suporte para ProblemException

### Formatters Customizados
- Suporte automático para `text/plain`
- Útil para webhooks e integrações

### Documentação com Scalar
- Ativo quando `UseScalar = true`
- Interface moderna para documentação de APIs

## 🤝 Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Crie um Pull Request

## 🏗️ Roadmap

- [x] Testes automatizados
- [x] Integração com AWS Systems Manager para armazenar segredos
- [x] Métricas e observabilidade
- [x] Logger customizado
- [ ] Integração com Azure Key Vault

## 📧 Contato

Para dúvidas ou sugestões, abra uma issue no repositório.

---

⭐ **Se este projeto foi útil para você, considere dar uma estrela!**