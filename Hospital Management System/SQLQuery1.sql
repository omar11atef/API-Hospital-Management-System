DELETE from Patients

DBCC CHECKIDENT ('Patients', RESEED, 0);   