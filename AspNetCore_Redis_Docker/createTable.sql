IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'AprendendoRedis')
  BEGIN
    CREATE DATABASE AprendendoRedis

END

GO
    USE AprendendoRedis
GO

IF NOT EXISTS (SELECT TOP 1 * 
                 FROM sys.objects 
                WHERE object_id = OBJECT_ID('AprendendoRedis.Paises') 
                  AND type IN ('U'))

BEGIN

CREATE TABLE Paises(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](250) NOT NULL,
	[IsoDuasLetras] [varchar](2) NULL,
	[IsoTresLetras] [varchar](3) NULL,
	[NumeroCodigoIso] [varchar](250) NULL,	
	CONSTRAINT [PK_Paises] PRIMARY KEY CLUSTERED ([Id] ASC)
)

	PRINT('Tabela nova Paises criada em - Database: AprendendoRedis.');

END
ELSE
	PRINT('Tabela Paises já existe no - Database: AprendendoRedis.');
GO





