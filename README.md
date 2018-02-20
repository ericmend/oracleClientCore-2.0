# News
Oracle Data Provider for .NET Core Official betta:
http://www.oracle.com/technetwork/topics/dotnet/downloads/odpnetcorebeta-4077982.html

# oracleClientCore-2.0
Unofficial Oracle Client for NetCore

Based on projects: [Mono](https://github.com/mono/mono) and [Oracleclientcore](https://github.com/LinqDan/oracleclientcore) from LinqDan

License
These files are licensed under the [MIT License](https://github.com/ericmend/oracleClientCore-2.0/blob/master/LICENSE)



*Portuguese Below. 

# About:
This project is a personal work to 'workaround' the problem with Oracle DataBase and **netcoreapp20** (oracle didn't certified ODP.NET Managed Driver in netcoreapp20 yet)
[odpnet-dotnet-core](http://www.oracle.com/technetwork/topics/dotnet/tech-info/odpnet-dotnet-core-sod-3628981.pdf)


### Development/Test Environment

* [Install NetCore SDK](https://www.microsoft.com/net/download/core)
* [Install VS Code](https://code.visualstudio.com/download)
* [Install OCI - Oracle Client](http://www.oracle.com/technetwork/database/features/instant-client/index-097480.html)
  * **linux** extract the files and setup the environment variable (e.g. LD_LIBRARY_PATH="/opt/oracle/instantclient"; 
  OCI_HOME="/opt/oracle/instantclient"; OCI_LIB_DIR="/opt/oracle/instantclient"; PATH=$PATH:"/opt/oracle/instantclient")
  * **windows** extract he files and setup enviroment variables.
*The database we are using a docker image: [sath89/oracle-xe-11g](https://hub.docker.com/r/sath89/oracle-xe-11g/)


### OBS:.
We did a work to run with windows/linux/macosx. Alread tested with win10 (windows) and ubuntu 17.10 (linux)

See more:

[Docker image for ASP.NET Core with Oracle Client](https://hub.docker.com/r/ericmend/aspnetcore-oracleclient/)

[ORM Dapper](https://github.com/StackExchange/Dapper)




##### PORTUGUESE ---->>>>
# Sobre:
Este projeto tem como finalidade disponibilizar uma possibiliade de uso do .NET Core usando banco de dado Oracle _(até que a Oracle consiga certificar o ODP.NET, Managed Driver no Microsoft .NET Core)_
[odpnet-dotnet-core](http://www.oracle.com/technetwork/topics/dotnet/tech-info/odpnet-dotnet-core-sod-3628981.pdf)
* OracleClient [Nuget](https://www.nuget.org/packages/dotNetCore.Data.OracleClient)

### Ambiente de Desenvolvimento/Teste

* [Instalar o SDK do .NET Core](https://www.microsoft.com/net/download/core)
* [Instalar Visual Studio Code](https://code.visualstudio.com/download)
* [Instalar OCI - Client Oracle](http://www.oracle.com/technetwork/database/features/instant-client/index-097480.html)
  * (linux) Descompactar os arquivos e declarar as variaveis de ambiente. ex: LD_LIBRARY_PATH="/opt/oracle/instantclient"; 
  OCI_HOME="/opt/oracle/instantclient"; OCI_LIB_DIR="/opt/oracle/instantclient"; PATH=$PATH:"/opt/oracle/instantclient"
  * (windows) Descompactar os arquivos e declarar as variaveis de ambiente. ex: LD_LIBRARY_PATH="C:\instantclient_12_2\"; 
  OCI_HOME="C:\instantclient_12_2\"; OCI_LIB_DIR="C:\instantclient_12_2\"; PATH=$PATH:"C:\instantclient_12_2\"
* Banco de Dados pode ser usado o docker: [sath89/oracle-xe-11g](https://hub.docker.com/r/sath89/oracle-xe-11g/)
* Disponível em [Nuget](https://www.nuget.org/packages/dotNetCore.Data.OracleClient)


### Obs:

Foi realizado um trabalho para que trabalhe em Windows/Linux/MacOsX. Testado até o momento apenas Windows (Win10) e Linux (Ubuntu 17.10).


Veja também:

[Docker para ASP.NET Core com Oracle Client](https://hub.docker.com/r/ericmend/aspnetcore-oracleclient/)

[ORM Dapper](https://github.com/StackExchange/Dapper)
