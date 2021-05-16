# AspNetCore_Redis_Docker
AspNetCore + Redis + Docker

Passo 1:

Execute os scripts localizados dentro da pasta AspNetCore_Redis_Docker da seguinte sequencia:

1) createTable
2) insertPaises

Passo 2:

No PowerShell do Windows:

--Criar o conteiner com o nome local-redis expondo a porta 6379 usando a imagem redis
docker run --name local-redis -p 6379:6379 -d redis

--Entra no conteiner em execucao
docker exec -it local -redis sh

--Executa a interface de linha de comando redis. Pra entrar dentro redis
redis-cli

--Verificar o status do contêiner. Para ver se o container esta em execução ou não
--docker ps

obs: Voce tem que deixar o container em execução para fazer a integração com o Aspt.Net Core

Passo 3:

1) Execute o projeto
1) Utilizaremos o postman para comparar o tempo gasto no request
2) Após executar o projeto abrir o swagger. Copie o seu http://localhost:porta + caminho do swagger => Ex: https://localhost:44349/api/Paises/redis
3) Com o postman ja aberto: Selecione GET e cole a url do item 2.
4) Aperte Send

obs: Note que a primeira requisição vai demorar mais do que a 2 
obs: O time fica localizado na parte de baixo, lado direito.

