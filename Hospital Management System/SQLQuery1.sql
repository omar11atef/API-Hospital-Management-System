DELETE from Departments

DBCC CHECKIDENT ('Departments', RESEED, 0);   