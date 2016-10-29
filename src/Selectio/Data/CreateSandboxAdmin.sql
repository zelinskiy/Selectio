
CREATE LOGIN [admin]   
    WITH PASSWORD = 'adminpass';  
GO  

CREATE USER [admin] 
	FOR LOGIN [admin]
	WITH DEFAULT_SCHEMA = dbo
GO

GRANT CONNECT TO [admin]
