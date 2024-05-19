# BookLand
Esse é um projeto para organização de livros, ele consome a API do google books e faz webscrapping para pegar a capa do livro da amazon
Mais importante, a existência desse projeto é para o meu aprendizado de dotnet e blazor com WASM
## SETUP:
### Criar um arquivo de config: `/BookLand/config.json`
```
{
  "connectionString": "Host=POSTGRESIP; Port=5432; Database=postgres; User Id=postgres; Password=POSTGRESSENHA;"
}
```
### Rodar o arquivo `/db/db.sql` no banco de dados para ter a estrutura
### Rodar o comando ``dotnet watch run`` dentro da pasta `/BookLand`
