# Guths.Shared

Uma biblioteca compartilhada para aplicações .NET que fornece funcionalidades comuns e reutilizáveis para projetos web, APIs e sistemas distribuídos.

## 🚀 Funcionalidades

- **Sistema de Result Pattern** - Padronização de retornos com validações
- **Autenticação JWT** - Configuração e utilitários para autenticação
- **Paginação** - DTOs e helpers para paginação de dados
- **MongoDB Integration** - Generic Repository pattern e configurações
- **Storage Services** - Integração com Amazon S3
- **Web Utilities** - Controllers base, formatters e middlewares
- **Localização** - Suporte multi-idioma

## 🔧 Instalação

```bash
dotnet add package Guths.Shared
```

## 📋 Configuração Básica

### 1. Program.cs

```csharp
using Guths.Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Adicionar configurações compartilhadas
builder.Services.AddSharedConfiguration(builder.Configuration, new SharedConfigurationOptions
{
    UseAuth = true,
    UseCache = true,
    UseCors = true
});

var app = builder.Build();

// Configurar pipeline da aplicação
// OBS: "MeuProjeto" -> Nome do seu projeto: Vai buscar configurações no Parameter Store com base nesse nome
app.AddSharedConfiguration("MeuProjeto", new SharedConfigurationOptions
{
    UseAuth = true,
    UseCors = true
});

app.Run();
```

### 2. appsettings.json ou AWS Parameter Store

```json
{
  "AuthSettings": {
    "Issuer": "https://meudominio.com",
    "Audience": "minha-api",
    "SigningKey": "sua-chave-secreta-aqui"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "MeuDatabase"
  },
  "S3StorageSettings": {
    "AccessKey": "sua-access-key",
    "SecretKey": "sua-secret-key",
    "BucketName": "meu-bucket",
    "Region": "us-east-1"
  }
}
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

### Versionamento de API

```csharp
[ApiController]
[RouteWithVersion("users")]
public class UserV1Controller : MyControllerBase
{
    // Implementação V1
}

[ApiController]
[RouteWithVersion("users", EApiVersions.V2)]
public class UserV2Controller : MyControllerBase
{
    // Implementação V2 com breaking changes
}
```

## 🌐 Localização

### Configuração

```csharp
// Automático com AddSharedConfiguration
builder.Services.AddSharedConfiguration(builder.Configuration);
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

```csharp
builder.Services.AddSharedConfiguration(builder.Configuration, new SharedConfigurationOptions
{
    UseAuth = true,
    ConfigureAuthorization = auth =>
    {
        auth.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    }
});
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

## 📚 Recursos Adicionais

### Middleware de Usuário Autenticado

```csharp
// Automático com UseAuth = true
// Popula informações do usuário no contexto
```

### Exception Handler Global

```csharp
// Automático com AddSharedConfiguration
// Trata todas as exceções de forma padronizada
```

### Formatters Customizados

```csharp
// Suporte automático para text/plain
// Útil para webhooks e integrações
```

## 🤝 Contribuindo

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-funcionalidade`)
3. Commit suas mudanças (`git commit -am 'Adiciona nova funcionalidade'`)
4. Push para a branch (`git push origin feature/nova-funcionalidade`)
5. Crie um Pull Request

## 🏗️ Roadmap

- [ ] Testes automatizados
- [ ] Integração com AWS Systems Manager para armazenar segredos
- [ ] Métricas e observabilidade
- [ ] Logger

## 📧 Contato

Para dúvidas ou sugestões, abra uma issue no repositório.

---

⭐ **Se este projeto foi útil para você, considere dar uma estrela!**